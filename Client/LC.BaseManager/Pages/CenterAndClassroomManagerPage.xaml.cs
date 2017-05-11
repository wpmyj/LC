using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using LC.Contracts.BaseManager;
using LC.Model.Business.BaseModel;
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

using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Aisino.MES.Resources;
using LC.Model;
using System.ServiceModel;
using MahApps.Metro.Controls.Dialogs;
using LC.BaseManager.Windows;

namespace LC.BaseManager.Pages
{
    /// <summary>
    /// CenterAndClassroomManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class CenterAndClassroomManagerPage : BusinessBasePage
    {
        private CenterModel _selectedCenter;
        public CenterAndClassroomManagerPage()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(RadGridViewCommands).TypeHandle);
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

        private void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Add.IsEnabled = add;
            //SetRight();
            bindCenterList();
        }

        private async void bindCenterList()
        {
            IAsyncProxy<ICenterService> _centerAyncProxy = await Task.Run(() => ServiceHelper.GetCenterService());
            IList<CenterModel> RM = await _centerAyncProxy.CallAsync(c => c.GetAllCenter());
            this.centerList.ItemsSource = RM;
        }

        private async void gvClassRoomDetail_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            if(_selectedCenter == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "请选择需要添加教室的中心", MessageDialogStyle.Affirmative, null);
                return;
            }
            try
            {
                EditClassroomWindow newClassroomWindow = new EditClassroomWindow();
                newClassroomWindow.ParentCenter = _selectedCenter;
                newClassroomWindow.Om = OperationMode.AddMode;

                if (newClassroomWindow.ShowDialog() == true)
                {
                    await bindClassroom();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加教室信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        

        private async Task bindClassroom()
        {
            string strErrorMsg = string.Empty;
            try
            {
                IAsyncProxy<IClassroomService> classroomAyncProxy = await Task.Run(() => ServiceHelper.GetClassroomService());
                IList<ClassroomModel> classroomModels = await classroomAyncProxy.CallAsync(f => f.FindClassroomByCenter(this._selectedCenter.Id));
                this.gvClassRoomDetail.ItemsSource = classroomModels;
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "绑定数据失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnDeleteDetail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (gvClassRoomDetail.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的教室！", MessageDialogStyle.Affirmative, null);
                return;
            }

            MessageDialogResult delResult = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该教室信息吗？", MessageDialogStyle.AffirmativeAndNegative, null);

            if (delResult == MessageDialogResult.Affirmative)
            {
                ClassroomModel selectClassroom = gvClassRoomDetail.SelectedItem as ClassroomModel;

                if (selectClassroom != null)
                {
                    string strErrorMsg = string.Empty;
                    try
                    {
                        IAsyncProxy<IClassroomService> classroomServiceProxy = await Task.Run(() => ServiceHelper.GetClassroomService());
                        if (selectClassroom.Id != 0)
                        {
                            //删除已经存在于数据库的数据，对于没有存于数据库的，则事件处理完成时都会刷新列表，故不用处理
                            bool blIsSuccess = await classroomServiceProxy.CallAsync(c => c.DeleteById(selectClassroom.Id));
                            if (blIsSuccess == true)
                            {
                                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除教室信息成功！", MessageDialogStyle.Affirmative, null);
                            }
                        }

                        IList<ClassroomModel> classroomModels = await classroomServiceProxy.CallAsync(f => f.FindClassroomByCenter(this._selectedCenter.Id));
                        this.gvClassRoomDetail.ItemsSource = classroomModels;
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
                        await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除教室信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    }
                }
            }
        }

        private async void UpdateClassroom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClassroomModel selectClassroom = gvClassRoomDetail.SelectedItem as ClassroomModel;

            if (selectClassroom != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    EditClassroomWindow newClassroomWindow = new EditClassroomWindow();
                    newClassroomWindow.SelectClassroom = gvClassRoomDetail.SelectedItem as ClassroomModel;
                    newClassroomWindow.Om = OperationMode.EditMode;

                    if (newClassroomWindow.ShowDialog() == true)
                    {
                        await bindClassroom();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新教室信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }

        #region 中心操作
        private void AddModule_Click(object sender, RoutedEventArgs e)
        {
            EditCenterWindow editCenterWindow = new EditCenterWindow();
            editCenterWindow.Om = OperationMode.AddMode;
            if (editCenterWindow.ShowDialog() == true)
            {
                this.bindCenterList();
            }
        }

        private async void UpdateModule_Click(object sender, RoutedEventArgs e)
        {
            if (this._selectedCenter == null || this.centerList.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择中心请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.centerList.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个中心，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            EditCenterWindow editCenterWindow = new EditCenterWindow();
            editCenterWindow.Om = OperationMode.EditMode;
            editCenterWindow.SelectCenter = this._selectedCenter;
            if (editCenterWindow.ShowDialog() == true)
            {
                this.bindCenterList();
            }
        }

        private async void DeleModule_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this._selectedCenter == null || this.centerList.SelectedItems.Count == 0)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择中心请选择！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确认删除所选的中心吗！", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    IAsyncProxy<ICenterService> _centerAyncProxy = await Task.Run(() => ServiceHelper.GetCenterService());
                    foreach (CenterModel item in this.centerList.SelectedItems)
                    {
                        await _centerAyncProxy.CallAsync(c => c.DeleteById(item.Id));
                    }
                    this._selectedCenter = null;
                    this.bindCenterList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除中心失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void centerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.centerList.SelectedItems.Count == 0)
                {
                    return;
                }
                this._selectedCenter = (CenterModel)this.centerList.SelectedItem;
                await bindClassroom();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "选取数据失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
        #endregion

        
    }
}
