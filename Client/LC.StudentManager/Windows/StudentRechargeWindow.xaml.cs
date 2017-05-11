using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.StudentManager;
using LC.Model;
using LC.Model.Business.StudentModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace LC.StudentManager.Windows
{
    /// <summary>
    /// StudentRechargeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StudentRechargeWindow : BaseWindow
    {
        private int studentId;

        public int StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }

        IAsyncProxy<IStudentService> studentAsyncProxy = null;
        StudentEditModel currentStudent; 

        public StudentRechargeWindow()
        {
            InitializeComponent();
        }

        private async void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                studentAsyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());
                currentStudent = await studentAsyncProxy.CallAsync(c => c.GetStudentById(this.StudentId));
                NameLable.Content = currentStudent.Name;
                NickNameLable.Content = currentStudent.Nickname;
                RemainBalanceLable.Content = currentStudent.RemainingBalance.ToString();

                NumRecharge.Value = currentStudent.NeedRecharge;
            }
            catch (TimeoutException timeProblem)
            {
                strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            }
            catch (FaultException<LCFault> af)
            {
                strErrorMsg = af.Detail.Message;
            }
            catch (FaultException unknownFault)
            {
                strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
            }
            catch (CommunicationException commProblem)
            {
                strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            }
            catch (Exception ex)
            {
                strErrorMsg = ex.Message;
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                bool res = await studentAsyncProxy.CallAsync(t => t.Recharge(StudentId, Convert.ToInt16(NumRecharge.Value),GlobalObjects.currentLoginUser));
                if (res == true)
                {
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "学员充值成功！");
                    this.DialogResult = true;
                }
            }
            catch (TimeoutException timeProblem)
            {
                strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            }
            catch (FaultException<LCFault> af)
            {
                strErrorMsg = af.Detail.Message;
            }
            catch (FaultException unknownFault)
            {
                strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
            }
            catch (CommunicationException commProblem)
            {
                strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            }
            catch (Exception ex)
            {
                strErrorMsg = ex.Message;
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "学员充值失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalObjects.blOpenCancelWarning == true)
            {
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "是否确认取消操作？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    this.DialogResult = false;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

    }
}
