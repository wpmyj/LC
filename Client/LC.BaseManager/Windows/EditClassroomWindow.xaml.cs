using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.BaseManager;
using LC.Model;
using LC.Model.Business.BaseModel;
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

namespace LC.BaseManager.Windows
{
    /// <summary>
    /// EditClassroomWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditClassroomWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private ClassroomModel selectClassroom;

        public ClassroomModel SelectClassroom
        {
            get { return selectClassroom; }
            set { selectClassroom = value; }
        }

        private CenterModel parentCenter;

        public CenterModel ParentCenter
        {
            get { return parentCenter; }
            set { parentCenter = value; }
        }

        private IAsyncProxy<IClassroomService> classroomAyncProxy;
        public EditClassroomWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                classroomAyncProxy = await Task.Run(() => ServiceHelper.GetClassroomService());
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
                this.Title = "AddClassroom";
                txtName.IsEnabled = true;
                UpperLimit.IsEnabled = true;
                BindClassroomInfo(null);
            }
            else
            {
                this.Title = "EditClassroom";
                txtName.IsEnabled = false;
                UpperLimit.IsEnabled = true;
                BindClassroomInfo(selectClassroom);
            }
        }
        private async void BindClassroomInfo(ClassroomModel selectClassroom)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (selectClassroom != null)
                {
                    txtName.Text = selectClassroom.Name;
                    UpperLimit.Value = selectClassroom.UpperLimit;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定用户信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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
                    ClassroomModel newClassroomModel = new ClassroomModel();
                    newClassroomModel.Name = txtName.Text.Trim();
                    newClassroomModel.UpperLimit = Convert.ToInt16(UpperLimit.Value);
                    newClassroomModel.CenterId = parentCenter.Id;

                    newClassroomModel = await classroomAyncProxy.CallAsync(c => c.Add(newClassroomModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增教室中心成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增教室信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    selectClassroom.Name = txtName.Text;
                    selectClassroom.UpperLimit = Convert.ToInt16(UpperLimit.Value);

                    selectClassroom = await classroomAyncProxy.CallAsync(c => c.Update(selectClassroom));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改教室成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改教程信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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
