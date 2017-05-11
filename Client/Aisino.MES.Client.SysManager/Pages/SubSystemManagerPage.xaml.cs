using Aisino.MES.Client.Common;
using Aisino.MES.Client.SysManager.Windows.SubSystemForms;
using Aisino.MES.Client.WPFCommon.BasePages;
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
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Pages
{
    /// <summary>
    /// SubSystemManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class SubSystemManagerPage : BusinessBasePage
    {
        private IAsyncProxy<ISubSystemModelService> _SubSystemAyncProxy;
        private SubSystemDisplayModel mySelectedElement;
        public SubSystemManagerPage()
        {
            InitializeComponent();
        }

        private void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            subSystemModelBind();
            subSystemMenuBind();
        }

        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth > 100)
            {
                dockPanel.Width = this.ActualWidth;
                dockPanel.Height = this.ActualHeight;
            }
        }

        #region 子菜单 增·删·改
        private async void addSubSys_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                SubSystemEditForm subSystemEditForm = new SubSystemEditForm();
                subSystemEditForm.OM = OperationMode.AddMode;
                if (subSystemEditForm.ShowDialog() == true)
                {
                    this.subSystemModelBind();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "子菜单修改失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void UpdateSubSys_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.mySelectedElement == null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要修改的子菜单！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                SubSystemEditForm subSystemEditForm = new SubSystemEditForm();
                subSystemEditForm.OM = OperationMode.EditMode;
                subSystemEditForm._subSystemDisplayModel = this.mySelectedElement;
                if (subSystemEditForm.ShowDialog() == true)
                {
                    this.subSystemModelBind();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "子菜单修改失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void DeleSubSys_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.mySelectedElement == null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的子菜单！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确认删除所选子菜单吗？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    this._SubSystemAyncProxy = await Task.Run(() => ServiceHelper.GetSubSystemService());
                    await this._SubSystemAyncProxy.CallAsync(c => c.DeleteByCode(this.mySelectedElement.SubSystemCode));
                    this.mySelectedElement = null;
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除子菜单成功！", MessageDialogStyle.Affirmative, null);
                    this.subSystemModelBind();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "子菜单删除失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
        #endregion

        #region 子菜单模块
        private async void subSystemModelBind()
        {
            this._SubSystemAyncProxy = await Task.Run(() => ServiceHelper.GetSubSystemService());
            IList<SubSystemDisplayModel> subSystemDisplayModelList = await this._SubSystemAyncProxy.CallAsync(g => g.GetAllSubMenu());
            this.ltSubSystem.ItemsSource = subSystemDisplayModelList;
        }
        #endregion

        #region 绑定菜单树
        private async void subSystemMenuBind()
        {
            IAsyncProxy<IMenuModelService> asyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
            MenuDisplayModel rootMenu = await asyncProxy.CallAsync(c => c.GetRootMenu());
            RadTreeViewItem menuHeader = new RadTreeViewItem();
            menuHeader.Header = rootMenu.Name;
            menuHeader.IsExpanded = true;
            this.rightmenuTree.Items.Add(menuHeader);
            EachTree(rootMenu, menuHeader);
        }



        /// <summary>
        ///构造菜单树
        /// </summary>
        /// <param name="mytree"></param>
        /// <param name="menuDisplayModel"></param>
        private async void EachTree(MenuDisplayModel rootMenu, RadTreeViewItem menuHeader)
        {
            string strErrorMsg = string.Empty;
            try
            {
                foreach (MenuDisplayModel menuItem in rootMenu.SubMenus)
                {
                    RadTreeViewItem treeViewItem = new RadTreeViewItem();
                    treeViewItem.Tag = menuItem;
                    treeViewItem.IsExpanded = true;
                    treeViewItem.Header = menuItem.Name;
                    menuHeader.Items.Add(treeViewItem);
                    if (menuItem.SubMenus != null && menuItem.SubMenus.Count > 0)
                    {
                        treeViewItem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F8F8"));
                        EachTree(menuItem, treeViewItem);
                    }
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "构造菜单树失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }

        #endregion

        #region 选中
        private async void ltSubSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                this.mySelectedElement = (SubSystemDisplayModel)this.ltSubSystem.SelectedItem;
                if (mySelectedElement == null)
                {
                    return;
                }
                (this.rightmenuTree.Items[0] as RadTreeViewItem).CheckState = ToggleState.Off;
                this.menuBindToRight(mySelectedElement.SubSystemCode);
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "选中失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }

        private async void menuBindToRight(string rightCode)
        {
            string strErrorMsg = string.Empty;
            try
            {
                List<MenuEditModel> rightSelectedList = new List<MenuEditModel>();
                IAsyncProxy<ISubSystemModelService> asyncProxy = await Task.Run(() => ServiceHelper.GetSubSystemService());
                SubSystemEditModel rightEditModel = await asyncProxy.CallAsync(d => d.GetSubSystemByCode(rightCode));
                if (rightEditModel != null)
                {
                    if (rightEditModel.SysMenus != null && rightEditModel.SysMenus.Count != 0)
                    {
                        foreach (var item in rightEditModel.SysMenus)
                        {
                            rightSelectedList.Add(item);
                        }
                    }
                }

                this.ShowChecked((RadTreeViewItem)this.rightmenuTree.Items[0], rightSelectedList);
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "菜单树失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }

        private void ShowChecked(RadTreeViewItem menuDisplayModel, List<MenuEditModel> rightSelectedList)
        {
            foreach (RadTreeViewItem menu in menuDisplayModel.Items)
            {
                foreach (var item in rightSelectedList)
                {
                    if (((MenuDisplayModel)menu.Tag).Code == item.MenuCode)
                    {
                        if (item.SubMenus != null && item.SubMenus.Count > 0)
                        {
                            menu.CheckState = ToggleState.Off;
                        }
                        else
                        {
                            menu.CheckState = ToggleState.On;
                        }
                    }
                }
                this.ShowChecked(menu, rightSelectedList);
            }
        }

        private async void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                List<String> IsCheckedList = new List<string>();
                this.ISChecked((RadTreeViewItem)this.rightmenuTree.Items[0], IsCheckedList);
                IAsyncProxy<ISubSystemModelService> asyncProxy = await Task.Run(() => ServiceHelper.GetSubSystemService());
                SubSystemEditModel rightEditModel = await asyncProxy.CallAsync(d => d.GetSubSystemByCode(this.mySelectedElement.SubSystemCode));
                await asyncProxy.CallAsync(d => d.SetSubSystemMenu(rightEditModel, IsCheckedList));
                //AisinoMessageBox.Show("修改权限菜单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "修改权限菜单成功！", MessageDialogStyle.Affirmative, null);
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "修改权限菜单失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }

        private void ISChecked(RadTreeViewItem menuDisplayModel, List<string> checkList)
        {
            foreach (RadTreeViewItem menu in menuDisplayModel.Items)
            {
                if (menu.CheckState != ToggleState.Off)
                {
                    checkList.Add((menu.Tag as MenuDisplayModel).Code);
                }
                this.ISChecked(menu, checkList);
            }
        }
        #endregion
    }

}
