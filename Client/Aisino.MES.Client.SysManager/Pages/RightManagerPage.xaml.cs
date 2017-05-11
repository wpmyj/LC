using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
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
using WcfClientProxyGenerator;
using WcfClientProxyGenerator.Async;
using MahApps.Metro.Controls;
using System.Collections;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Resources;
using Aisino.MES.Client.SysManager.Windows.RightManagerWin;
using System.Xml;
using MahApps.Metro.Controls.Dialogs;
using Telerik.Windows.Controls;
using System.Windows.Automation;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using LC.Model;


namespace Aisino.MES.Client.SysManager.Pages
{
    /// <summary>
    /// RightManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class RightManagerPage : BusinessBasePage
    {
        private RightDisplayModel mySelectedElement;
        public RightManagerPage()
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

        private async void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            await addRightList();
            addRightTree();
        }
        /// <summary>
        /// 加载权限列表
        /// </summary>
        private async Task addRightList()
        {
            string strErrorMsg = string.Empty;
            try
            {
                IAsyncProxy<IRightModelService> asyncProxy = await Task.Run(() => ServiceHelper.GetRightService());
                IList<RightDisplayModel> RM = await asyncProxy.CallAsync(c => c.GetAllRights());
                this.rightList.ItemsSource = RM;
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "加载权限列表失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }
        /// <summary>
        /// 加载菜单列表
        /// </summary>
        private async void addRightTree()
        {
            string strErrorMsg = string.Empty;
            try
            {
                IAsyncProxy<IMenuModelService> asyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                MenuDisplayModel rootMenu = await asyncProxy.CallAsync(c => c.GetRootMenu());
                RadTreeViewItem menuHeader = new RadTreeViewItem();
                menuHeader.Header = rootMenu.Name;
                menuHeader.IsExpanded = true;
                this.rightmenuTree.Items.Add(menuHeader);
                EachTree(rootMenu, menuHeader);
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "加载菜单列表失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
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

        #region MyRegion
        /// <summary>
        /// 点击添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddRight_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditRightForm RightAdd = new EditRightForm();
                RightAdd.Om = OperationMode.AddMode;
                if (RightAdd.ShowDialog() == true)
                {
                    await this.addRightList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加权限失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }

        /// <summary>
        ///点击修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateRight_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.mySelectedElement==null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要修改的权限！", MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("请选择权限名称！", "提示", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
                    return;
                }
                EditRightForm RightAdd = new EditRightForm();
                RightAdd.Om = OperationMode.EditMode;
                RightAdd._sysCode = this.mySelectedElement.Code;
                if (RightAdd.ShowDialog() == true)
                {
                    await this.addRightList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "修改权限失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        }


        /// <summary>
        /// 点击删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleRight_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.mySelectedElement==null)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的权限！", MessageDialogStyle.AffirmativeAndNegative, null);
                    //AisinoMessageBox.Show("请选择要删除的权限", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    return;
                }
                IAsyncProxy<IRightModelService> asyncProxyRight = await Task.Run(() => ServiceHelper.GetRightService());
                //删除提示
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确认删除所选权限吗？", MessageDialogStyle.AffirmativeAndNegative, null);
                //MessageBoxResult result = AisinoMessageBox.Show("确定删除该权限吗", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
                if (result == MessageDialogResult.Affirmative)
                {
                    await asyncProxyRight.CallAsync(c => c.DeleteByCode(this.mySelectedElement.Code));
                    await this.addRightList();
                    this.mySelectedElement = null;
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除权限成功！", MessageDialogStyle.Affirmative, null);
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "修改权限失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }
        } 
        #endregion


        #region 选中
        private async void Label_MouseLeftButtonDown(object sender, SelectionChangedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                this.mySelectedElement = (RightDisplayModel)rightList.SelectedItem;
                if (mySelectedElement == null)
                {
                    return;
                }
                (this.rightmenuTree.Items[0] as RadTreeViewItem).CheckState = ToggleState.Off;
                this.menuBindToRight(mySelectedElement.Code);
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
                IAsyncProxy<IRightModelService> asyncProxy = await Task.Run(() => ServiceHelper.GetRightService());
                RightEditModel rightEditModel = await asyncProxy.CallAsync(d => d.GetRightByCode(rightCode));
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

        /// <summary>
        /// 点击保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveRight_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                List<String> IsCheckedList = new List<string>();
                this.ISChecked((RadTreeViewItem)this.rightmenuTree.Items[0], IsCheckedList);
                IAsyncProxy<IRightModelService> asyncProxyRight = await Task.Run(() => ServiceHelper.GetRightService());
                RightEditModel rightEditModel = await asyncProxyRight.CallAsync(d => d.GetRightByCode(this.mySelectedElement.Code));
                await asyncProxyRight.CallAsync(d => d.SetRightMenus(rightEditModel, IsCheckedList));
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
                if (menu.CheckState!=ToggleState.Off)
                {
                    checkList.Add((menu.Tag as MenuDisplayModel).Code);
                }
                this.ISChecked(menu, checkList);
            }
        }
        #endregion
    }
}
