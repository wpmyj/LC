using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.BaseManager;
using LC.Contracts.TeacherManager;
using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Model.Business.SysManager;
using LC.Model.Business.TeacherModel;
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

namespace LC.BaseManager.Windows
{
    /// <summary>
    /// EditTeacherWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditTeacherWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private TeacherModel selectedTeacher;

        public TeacherModel SelectedTeacher
        {
            get { return selectedTeacher; }
            set { selectedTeacher = value; }
        }

        IAsyncProxy<ITeacherService> teacherAsyncProxy = null;

        public EditTeacherWindow()
        {
            InitializeComponent();
        }

        private async void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                teacherAsyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService());

                await bindStatusList();
                await bindSysUserList();
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
                this.Title = "AddTeacher";
                txtName.IsEnabled = true;
                BindTeacherInfo();
            }
            else
            {
                this.Title = "EditTeacher";
                txtName.IsEnabled = false;
                BindTeacherInfo();
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
                    TeacherModel newTeacherModel = new TeacherModel();
                    newTeacherModel.Name = txtName.Text.Trim();
                    newTeacherModel.Mobile = txtMobile.Text.Trim();
                    newTeacherModel.StatusId = (cmbStatus.SelectedItem as StatusModel).Id;
                    newTeacherModel.UserCode = cmbUser.Text;

                    newTeacherModel = await teacherAsyncProxy.CallAsync(c => c.Add(newTeacherModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增教师信息成功！");
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
                    selectedTeacher.Name = txtName.Text.Trim();
                    selectedTeacher.Mobile = txtMobile.Text.Trim();
                    selectedTeacher.StatusId = (cmbStatus.SelectedItem as StatusModel).Id;
                    selectedTeacher.UserCode = cmbUser.Text;

                    selectedTeacher = await teacherAsyncProxy.CallAsync(c => c.Update(selectedTeacher));

                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改教室信息成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改教室信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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

        private async Task bindStatusList()
        {
            IAsyncProxy<IStatusService> _statusAyncProxy = await Task.Run(() => ServiceHelper.GetStatusService());
            IList<StatusModel> RM = await _statusAyncProxy.CallAsync(c => c.FindStatusByCat(GlobalObjects.TeacherStatus));
            this.cmbStatus.ItemsSource = RM;
        }

        private async Task bindSysUserList()
        {
            IAsyncProxy<IUserModelService> sysuserAsyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
            IList<UserDisplayModel> RM = await sysuserAsyncProxy.CallAsync(c => c.GetAllUser());
            this.cmbUser.ItemsSource = RM;
        }

        private async void BindTeacherInfo()
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (selectedTeacher != null)
                {
                    txtName.Text = selectedTeacher.Name;
                    txtMobile.Text = selectedTeacher.Mobile;
                    cmbStatus.Text = selectedTeacher.Status;
                    cmbUser.Text = selectedTeacher.UserCode;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定教室信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
    }
}
