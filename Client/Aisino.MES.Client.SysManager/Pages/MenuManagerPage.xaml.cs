using Aisino.MES.Client.Common;
using Aisino.MES.Client.SysManager.Windows.MenuForms;
using Aisino.MES.Client.WPFCommon.BasePages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Pages
{
    /// <summary>
    /// MenuManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class MenuManagerPage : BusinessBasePage
    {
        private MenuDisplayModel _menuDisplayModel;
        private IAsyncProxy<IMenuModelService> _menuAyncProxy;
        TreeViewItem _menuHeader;
        public MenuManagerPage()
        {
            InitializeComponent();
        }
        protected override void SetRight()
        {

        }


        protected override void OnClosePage(PageCloseArgs e)
        {
            base.OnClosePage(e);
        }

        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth > 100)
            {
                dockPanel.Width = this.ActualWidth;
                dockPanel.Height = this.ActualHeight;
            }
        }

        private void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            bindMenuList();
        }
        private async void bindMenuList()
        {
            string strErrorMsg = string.Empty;
            try
            {
                this.trvMenu.Items.Clear();
                this._menuAyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                MenuDisplayModel rootMenu = await _menuAyncProxy.CallAsync(c => c.GetRootMenu());
                _menuHeader = new TreeViewItem();
                _menuHeader.Header = rootMenu.Name;
                _menuHeader.IsExpanded = true;
                _menuHeader.Tag = rootMenu;
                this.trvMenu.Items.Add(_menuHeader);
                this.buildMenuTree(rootMenu, _menuHeader);
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "绑定菜单失败，原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
        private void buildMenuTree(MenuDisplayModel rootMenu, TreeViewItem menuHeader)
        {
            foreach (MenuDisplayModel menuItem in rootMenu.SubMenus)
            {
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.IsExpanded = true;
                treeViewItem.Tag = menuItem;
                treeViewItem.Header = menuItem.Name;
                menuHeader.Items.Add(treeViewItem);
                if (menuItem.SubMenus != null && menuItem.SubMenus.Count > 0)
                {
                    buildMenuTree(menuItem, treeViewItem);
                }
            }
        }
        private async void btnAddMenu_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.trvMenu.SelectedItem == null || this._menuDisplayModel == null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择菜单请选择！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                MenuEditForm menuEditForm = new MenuEditForm();
                menuEditForm.Om = OperationMode.AddMode;
                menuEditForm._menuDisplayModel = this._menuDisplayModel;
                if (menuEditForm.ShowDialog() == true)
                {
                    bindMenuList();
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "新增菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "新增菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnUpdateMenu_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.trvMenu.SelectedItem == null || this._menuDisplayModel == null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择菜单请选择！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                MenuEditForm menuEditForm = new MenuEditForm();
                menuEditForm.Om = OperationMode.EditMode;
                menuEditForm._menuDisplayModel = this._menuDisplayModel;
                if (menuEditForm.ShowDialog() == true)
                {

                    bindMenuList();
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "修改菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnDeleMenu_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.trvMenu.SelectedItem == null || this._menuDisplayModel == null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择菜单请选择！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确认删除所选菜单吗？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    _menuAyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                    await _menuAyncProxy.CallAsync(c => c.DeleteByCode(this._menuDisplayModel.Code));
                    this._menuDisplayModel = null;
                    bindMenuList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void trvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.trvMenu.SelectedItem == null)
                {
                    return;
                }
                this._menuDisplayModel = (MenuDisplayModel)((TreeViewItem)this.trvMenu.SelectedItem).Tag;
                if (_menuDisplayModel == null)
                {
                    return;
                }
                IAsyncProxy<IMenuModelService> _menuAyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                MenuEditModel _sysmenu = await _menuAyncProxy.CallAsync(c => c.GetMenuByCode(this._menuDisplayModel.Code));

                this.labMenuCode.Content = _sysmenu.MenuCode;
                this.labName.Content = _sysmenu.Name;
                this.labDisplayName.Content = _sysmenu.DisplayName;
                this.labShowIndex.Content = _sysmenu.ShowIndex;
                this.labMenuParent.Content = _sysmenu.ParentCode;
                this.labMenuType.Content = _sysmenu.Type;
                this.txtMenuRemark.Text = _sysmenu.Remark;
                this.labModule.Content = _sysmenu.ModuleCode;
                this.labFunction.Content = _sysmenu.FunctionCode;
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "选择菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

    }
}
