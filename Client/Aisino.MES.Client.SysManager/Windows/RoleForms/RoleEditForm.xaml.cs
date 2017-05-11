using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Resources;
using LC.Model;
using LC.Model.Business.SysManager;
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

namespace Aisino.MES.Client.SysManager.Windows.RoleForms
{
    /// <summary>
    /// RoleEditForm.xaml 的交互逻辑
    /// </summary>
    public partial class RoleEditForm
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        public RoleDisplayModel _roleDisplayModel;

        IAsyncProxy<IRoleModelService> roleAyncProxy;

        public RoleEditForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Om == OperationMode.AddMode)
            {
                this.Title = "添加角色";
            }
            else if (this.Om == OperationMode.EditMode)
            {
                this.Title = "修改角色";
                this.roleCode.Text = _roleDisplayModel.Code;
                this.roleName.Text = _roleDisplayModel.Name;
                this.roleStopped.IsChecked = _roleDisplayModel.Stopped;
                this.roleRemark.Text = _roleDisplayModel.Remark;
                this.roleCode.IsEnabled = false;
                this.roleName.IsEnabled = false;
            }
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                RoleEditModel _sysRole = new RoleEditModel();
                _sysRole.RoleCode = this.roleCode.Text.Trim();
                _sysRole.Name = this.roleName.Text.Trim();
                _sysRole.Stopped = this.roleStopped.IsChecked;
                _sysRole.Remark = this.roleRemark.Text.Trim();
                this.roleEdit(_sysRole);
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "操作角色信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strMsg);
            }
        }

        private async void btnCancle_Click(object sender, RoutedEventArgs e)
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

        private async void roleEdit(RoleEditModel sysRole)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (Om == OperationMode.AddMode)
                {
                    roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
                    await roleAyncProxy.CallAsync(d => d.Add(sysRole));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增角色成功！");
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "新增角色成功！", MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("新增角色成功！", UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    this.DialogResult = true;
                }
                else if (Om == OperationMode.EditMode)
                {
                    roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
                    await roleAyncProxy.CallAsync(d => d.Update(sysRole, null, null));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改角色成功！");
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "修改角色成功！", MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("修改角色成功！", UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    this.DialogResult = true;
                }
                this.Close();
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "操作角色信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }
    }
}
