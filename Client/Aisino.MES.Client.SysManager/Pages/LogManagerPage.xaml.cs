using Aisino.MES.Client.Common;
using Aisino.MES.Client.SysManager.Windows.MenuForms;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Model;
using Aisino.MES.Model.Business;
using Aisino.MES.Model.Business.SysManager;
using Aisino.MES.Model.PageModel;
using Aisino.MES.Resources;
using Aisino.MES.Service.Contracts.Common;
using Aisino.MES.Service.Contracts.SysManager;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Pages
{
    /// <summary>
    /// LogManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogManagerPage
    {
        string userName = string.Empty;
        IAsyncProxy<ILogService> logAsyncProxy = null;
        IAsyncProxy<IUserModelService> userAsyncProxy = null;
        IAsyncProxy<IModuleModelService> moduleAsyncProxy = null;
        AisinoPageIList<LogDisplayModel> aplLogs;

        AisinoPagingCriteria _paging;

        //查询条件
        string userCode = string.Empty;
        string moduleCode = string.Empty;
        DateTime? dtStart = null;
        DateTime? dtEnd = null;

        public LogManagerPage()
        {
            InitializeComponent();
        }

        private async void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            _paging = new AisinoPagingCriteria();
            InitPageInfo();
            logAsyncProxy = await Task.Run(() => ServiceHelper.GetLogService());
            userAsyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
            moduleAsyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
            userName = GlobalObjects.currentLoginUser.LoginName;
            rdpLog.PageIndex = 0;
            BindGridLog();
            BindUser();
            BindModule();
        }

        //初始化分页信息
        private void InitPageInfo()
        {
            _paging.SortBy = "LogTime";
            _paging.SortDirection = "desc";
            _paging.PageSize = rdpLog.PageSize = 10;            
            _paging.PageNumber = 1;     
        }

        private async void BindGridLog()
        {
            aplLogs = await logAsyncProxy.CallAsync(c => c.FindLogByUserAndModuleAndDateWithPage(userCode, moduleCode, dtStart, dtEnd, _paging));
            
            if (aplLogs != null && aplLogs.totalCount > 0)
            {
                gvLog.ItemsSource = aplLogs.Entities;
                rdpLog.ItemCount = aplLogs.totalCount;
                
            }
            else
            {
                gvLog.ItemsSource = null;
                rdpLog.ItemCount = 0;
                rdpLog.Source = null;
            }
        }

        private async void BindUser()
        {
            IList<UserDisplayModel> lstUsers = await userAsyncProxy.CallAsync(c => c.GetAllUser());
            cmbUser.ItemsSource = lstUsers;
            cmbUser.DisplayMemberPath = "Name";
            cmbUser.SelectedValuePath = "Code";
        }

        private async void BindModule()
        {
            IList<ModuleDisplayModel> lstModules = await moduleAsyncProxy.CallAsync(c => c.GetAllModules());
            
            cmbModule.ItemsSource = lstModules;
            cmbModule.DisplayMemberPath = "Name";
            cmbModule.SelectedValuePath = "Code";

            //ComboBoxItem item = new ComboBoxItem();
            //item.Content = "-请选择-";
            //cmbModule.Items.Add(item);
        }

        private void SearchControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (cmbUser.SelectedIndex != -1)
            {
                userCode = cmbUser.SelectedValue.ToString();
            }
            if (cmbModule.SelectedIndex != -1)
            {
                moduleCode = cmbModule.SelectedValue.ToString();
            }
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue && dpStartDate.SelectedDate == dpEndDate.SelectedDate)
            {
                dtStart = dtEnd = dpStartDate.SelectedDate.Value;
            }
            else
            {
                if (dpStartDate.SelectedDate.HasValue)
                {
                    dtStart = dpStartDate.SelectedDate.Value;
                }
                if (dpEndDate.SelectedDate.HasValue)
                {
                    dtEnd = dpEndDate.SelectedDate.Value;
                }
            }
            rdpLog.PageIndex = 0;
            BindGridLog();
        }

        private async void DeleteControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                List<int> idList = new List<int>();
                for (int i = 0; i < gvLog.SelectedItems.Count; i++)
                {
                    LogDisplayModel logDisplayModel = (LogDisplayModel)gvLog.SelectedItems[i];
                    idList.Add(logDisplayModel.Id);
                }
                if (idList.Count > 0)
                {
                    bool isDel = await logAsyncProxy.CallAsync(c => c.DeleteByIdList(idList));
                    BindGridLog();
                }
                else
                {
                    //await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "删除选中日志失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
                #region
                //for (int rowCounter = 0; rowCounter < gvLog.Items.Count; rowCounter++)
                //{
                //    object o = gvLog.Items[rowCounter];
                //    GridViewRow grdLogRow = (GridViewRow)gvLog.ItemContainerGenerator.ContainerFromItem(o);
                //    if (grdLogRow != null)
                //    {
                //        CheckBox chkLogChecked = grdLogRow.Cells[0].FindChildByType<CheckBox>();
                //        if (chkLogChecked.IsChecked == true)
                //        {
                //            //grdLogRow.Cells[1]
                //            LogDisplayModel logDisplayModel = (LogDisplayModel)gvLog.Items[rowCounter];
                //            idList.Add(logDisplayModel.Id);
                //        }
                //    }
                //}
                //if (idList.Count > 0)
                //{
                //    bool isDel = await asyncProxy.CallAsync(c => c.DeleteByIdList(idList));
                //}
                #endregion
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
                //await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "删除选中日志失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private void chkAllChecked_Click(object sender, RoutedEventArgs e)
        {
            //if (chkAllChecked.IsChecked == true)
            //{
            //    for (int rowCounter = 0; rowCounter < gvLog.Items.Count; rowCounter++)
            //    {
            //        object o = gvLog.Items[rowCounter];
            //        GridViewRow grdLogRow = (GridViewRow)gvLog.ItemContainerGenerator.ContainerFromItem(o);
            //        if (grdLogRow != null)
            //        {
            //            CheckBox chkLogChecked = grdLogRow.Cells[0].FindChildByType<CheckBox>();
            //            chkLogChecked.IsChecked = true;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int rowCounter = 0; rowCounter < gvLog.Items.Count; rowCounter++)
            //    {
            //        object o = gvLog.Items[rowCounter];
            //        GridViewRow grdLogRow = (GridViewRow)gvLog.ItemContainerGenerator.ContainerFromItem(o);
            //        if (grdLogRow != null)
            //        {
            //            CheckBox chkLogChecked = grdLogRow.Cells[0].FindChildByType<CheckBox>();
            //            chkLogChecked.IsChecked = false;
            //        }
            //    }
            //}
            //for (int rowCounter = 0; rowCounter < gvLog.Items.Count; rowCounter++)
            //{
            //    object o = gvLog.Items[rowCounter];
            //    GridViewRow grdLogRow = (GridViewRow)gvLog.ItemContainerGenerator.ContainerFromItem(o);
            //    if (grdLogRow != null)
            //    {
            //        CheckBox chkLogChecked = grdLogRow.Cells[0].FindChildByType<CheckBox>();
            //        chkLogChecked.IsChecked = chkAllChecked.IsChecked.HasValue ? chkAllChecked.IsChecked.Value : false;
            //    }
            //}
        }

        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (this.ActualWidth > 100)
            //{
            //    dockPanel.Width = this.ActualWidth;
            //    dockPanel.Height = this.ActualHeight;
            //}
        }

        private void rdpLog_PageIndexChanging(object sender, PageIndexChangingEventArgs e)
        {
            _paging.PageNumber = e.NewPageIndex + 1;
            BindGridLog();
        }
    }
}
