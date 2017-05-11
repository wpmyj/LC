using Aisino.MES.Client.Common;
using Aisino.MES.Client.SysManager.Windows.DeptAndUser;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Resources;
using LC.Model;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Telerik.Windows.Controls;
using WcfClientProxyGenerator;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Pages
{
    /// <summary>
    /// DeptAndUserManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class DeptAndUserManagerPage : BusinessBasePage
    {
        TreeViewItem trviRootDept = null;

        IAsyncProxy<IUserModelService> userAsyncProxy;


        public DeptAndUserManagerPage()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(RadGridViewCommands).TypeHandle);
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await BindSysUserList();
        }

        private async Task BindSysUserList()
        {
            string strErrorMsg = string.Empty;
            try
            {
                this.userAsyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
                IList<UserDisplayModel> lstUsers = await userAsyncProxy.CallAsync(c => c.GetAllUser());
                if (lstUsers != null && lstUsers.Count > 0)
                {
                    gvUsers.ItemsSource = lstUsers;
                }
                else
                {
                    gvUsers.ItemsSource = null;
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "用户信息绑定失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private void Exit_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            OnClosePage(new PageCloseArgs(this.GetType().Name));
        }

        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth > 100)
            {
                dockPanel.Width = this.ActualWidth;
                dockPanel.Height = this.ActualHeight;
            }
        }


        private async void btnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditUserWindow editUserWindow = new EditUserWindow();
                editUserWindow.Om = OperationMode.EditMode;
                UserDisplayModel userDisplayModel = gvUsers.SelectedItem as UserDisplayModel;
                if (userDisplayModel != null)
                {
                    editUserWindow.SelectSysUser = await userAsyncProxy.CallAsync(c => c.GetUserByCode(userDisplayModel.Code));
                }
                if (editUserWindow.ShowDialog() == true)
                {
                    await BindSysUserList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "修改用户失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该用户吗？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    UserDisplayModel userDisplayModel = gvUsers.SelectedItem as UserDisplayModel;
                    if (userDisplayModel != null)
                    {
                        bool blIsSuccess = await userAsyncProxy.CallAsync(c => c.DeleteByCode(userDisplayModel.Code));
                        if (blIsSuccess == true)
                        {
                            await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除用户成功！", MessageDialogStyle.Affirmative, null);
                            await BindSysUserList();
                        }
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
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除用户失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void gvUsers_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditUserWindow newUserWindow = new EditUserWindow();
                newUserWindow.Om = OperationMode.AddMode;

                if (newUserWindow.ShowDialog() == true)
                {
                    await BindSysUserList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加用户失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private void btnUpdateUser_Click(object sender, MouseButtonEventArgs e)
        {

        }

    }

}
