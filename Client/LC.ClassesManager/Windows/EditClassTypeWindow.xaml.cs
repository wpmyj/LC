using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.ClassManager;
using LC.Model;
using LC.Model.Business.ClassModel;
using LC.Service.Contracts.SysManager;
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

namespace LC.ClassesManager.Windows
{
    /// <summary>
    /// EditClassType.xaml 的交互逻辑
    /// </summary>
    public partial class EditClassTypeWindow:BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private ClassTypeModel selectClassType;

        public ClassTypeModel SelectClassType
        {
            get { return selectClassType; }
            set { selectClassType = value; }
        }

        IAsyncProxy<IClassTypeService> classTypeAsyncProxy = null;
        public EditClassTypeWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                classTypeAsyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
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

            if (Om == OperationMode.AddMode)
            {
                this.Title = "新增班级类型";
                txtName.IsEnabled = true;
                BindClassTypeInfo(null);
            }
            else
            {
                this.Title = "修改班级类型";
                txtName.IsEnabled = false;
                IAsyncProxy<IUserModelService> userAsyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
                if (userAsyncProxy.CallAsync(u=>u.CheckUserRole(GlobalObjects.currentLoginUser.UserCode,"consultant")).Result)
                {
                    numPrice.IsEnabled = false;
                }
                BindClassTypeInfo(SelectClassType);
            }
        }

        private async void BindClassTypeInfo(ClassTypeModel selectClassType)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (selectClassType != null)
                {
                    txtName.Text = selectClassType.Name;
                    numTotal.Value = selectClassType.TotalLessons;
                    numPrice.Value = selectClassType.UnitPrice;
                    numStuLimit.Value = selectClassType.StudentLimit;
                    numTeacherRate.Value = selectClassType.TeacherRate;
                    numAssistantRate.Value = selectClassType.AssistantRate;
                    numConsultantRate.Value = selectClassType.ConsultantRate;
                    txtRemark.Text = selectClassType.Des;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定班级类型失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            #region 新增
            if (Om == OperationMode.AddMode)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    ClassTypeModel newClassTypeModel = new ClassTypeModel();
                    newClassTypeModel.Name = txtName.Text.Trim();
                    newClassTypeModel.TotalLessons = Convert.ToInt16(numTotal.Value);
                    newClassTypeModel.UnitPrice = Convert.ToInt16(numPrice.Value);
                    newClassTypeModel.StudentLimit = Convert.ToInt16(numStuLimit.Value);
                    newClassTypeModel.TeacherRate = Convert.ToDouble(numTeacherRate.Value);
                    newClassTypeModel.AssistantRate = Convert.ToDouble(numAssistantRate.Value);
                    newClassTypeModel.ConsultantRate = Convert.ToDouble(numConsultantRate.Value);
                    newClassTypeModel.Des = txtRemark.Text.Trim();
                    newClassTypeModel.IsActive = true;

                    newClassTypeModel = await classTypeAsyncProxy.CallAsync(c => c.Add(newClassTypeModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增班级类型成功！");
                    this.DialogResult = true;
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
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增班级类型失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    selectClassType.TotalLessons = Convert.ToInt16(numTotal.Value);
                    selectClassType.UnitPrice = Convert.ToInt16(numPrice.Value);
                    selectClassType.StudentLimit = Convert.ToInt16(numStuLimit.Value);
                    selectClassType.TeacherRate = Convert.ToDouble(numTeacherRate.Value);
                    selectClassType.AssistantRate = Convert.ToDouble(numAssistantRate.Value);
                    selectClassType.ConsultantRate = Convert.ToDouble(numConsultantRate.Value);
                    selectClassType.Des = txtRemark.Text.Trim();
                    selectClassType.IsActive = true;

                    selectClassType = await classTypeAsyncProxy.CallAsync(c => c.Update(selectClassType));
                    
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改班级类型成功！");
                    this.DialogResult = true;
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
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改班级类型失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
            #endregion
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
