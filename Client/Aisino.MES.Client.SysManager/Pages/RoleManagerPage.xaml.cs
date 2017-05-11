using Aisino.MES.Client.Common;
using Aisino.MES.Client.SysManager.Windows.RoleForms;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Resources;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Pages
{
    /// <summary>
    /// RoleManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class RoleManagerPage : BusinessBasePage
    {
        private RoleDisplayModel _roleDisplayModel;
        IAsyncProxy<IRoleModelService> roleAyncProxy;
        public RoleManagerPage()
        {
            InitializeComponent();
        }

        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth > 100)
            {
                dockPanel.Width = this.ActualWidth;
                dockPanel.Height = this.ActualHeight;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bindRoleList();
        }

        private async void bindRoleList()
        {
            roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
            IList<RoleDisplayModel> listRoles = await roleAyncProxy.CallAsync(c => c.GetAllRoles());
            this.listRole.ItemsSource = listRoles;
        }

        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            RoleEditForm RoleAdd = new RoleEditForm();
            RoleAdd.Om = OperationMode.AddMode;
            if (RoleAdd.ShowDialog() == true)
            {
                bindRoleList();
            }
        }

        private async void btnUpdateRole_Click(object sender, RoutedEventArgs e)
        {
            if (this.listRole.SelectedItems.Count == 0 || this._roleDisplayModel == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要修改的角色！", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("没有选择角色，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            if (this.listRole.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个需要修改的角色", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("不能选择多个角色请重新选择，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            RoleEditForm roleEditForm = new RoleEditForm();
            roleEditForm.Om = OperationMode.EditMode;
            roleEditForm._roleDisplayModel = this._roleDisplayModel;
            if (roleEditForm.ShowDialog() == true)
            {
                bindRoleList();
            }
        }

        private async void btnDeleteRole_Click(object sender, RoutedEventArgs e)
        {
            if (this.listRole.SelectedItems.Count == 0 || this._roleDisplayModel == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的角色！", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("没有选择角色，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该角色吗？", MessageDialogStyle.AffirmativeAndNegative, null);
            //MessageBoxResult result = AisinoMessageBox.Show("确定删除该权限吗", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
            if (result == MessageDialogResult.Affirmative)
            {
                roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
                foreach (RoleDisplayModel roleDisplayModel in this.listRole.SelectedItems)
                {
                    await roleAyncProxy.CallAsync(d => d.DeleteByCode(roleDisplayModel.Code));
                }
                this._roleDisplayModel = null;
                bindRoleList();
            }
        }

        private async void btnAllUser_Click(object sender, RoutedEventArgs e)
        {
            if (this.listRole.SelectedItems.Count == 0 || this._roleDisplayModel == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要配置所含用户的角色！", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("没有选择角色，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            if (this.listRole.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个需要配置所含用户的角色！", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("不能选择多个角色请重新选择，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            SetRoleUserForm setRoleUserForm = new SetRoleUserForm();
            setRoleUserForm.RoleDisplayModel = this._roleDisplayModel;
            setRoleUserForm.ShowDialog();
            UserRight();
        }

        private async void btnAllRight_Click(object sender, RoutedEventArgs e)
        {
            if (this.listRole.SelectedItems.Count == 0 || this._roleDisplayModel == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要配置所含权限的角色！", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("没有选择角色，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            if (this.listRole.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个需要配置所含权限的角色！", MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("不能选择多个角色请重新选择，请选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }
            SetRoleRightForm setrightUserForm = new SetRoleRightForm();
            setrightUserForm.RoleDisplayModel = this._roleDisplayModel;
            setrightUserForm.ShowDialog();
            UserRight();
        }

        private async void listRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listRole.SelectedItems.Count == 0)
            {
                return;
            }
            _roleDisplayModel = (RoleDisplayModel)this.listRole.SelectedItem;
            roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
            UserRight();
        }
        private async void UserRight()
        {
            RoleEditModel roleEditModel = await roleAyncProxy.CallAsync(c => c.GetRoleByCode(_roleDisplayModel.Code));
            this.listRight.ItemsSource = roleEditModel.SysRights;
            this.listUser.ItemsSource = roleEditModel.SysUsers;
        }

    }
}
