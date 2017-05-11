using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Resources;
using LC.Contracts.ClassManager;
using LC.Contracts.TeacherManager;
using LC.Model;
using LC.Model.Business.ClassModel;
using LC.Model.Business.TeacherModel;
using LC.BaseManager.Windows;
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
using WcfClientProxyGenerator.Async;

namespace LC.BaseManager.Pages
{
    /// <summary>
    /// TeacherManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class TeacherManagerPage : BusinessBasePage
    {
        public TeacherManagerPage()
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

        private async void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            await bindTeacherList();
        }

        private async Task bindTeacherList()
        {
            IAsyncProxy<ITeacherService> _teacherAyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
            IList<TeacherModel> RM = await _teacherAyncProxy.CallAsync(c => c.GetAllTeacher());
            this.gvTeacher.ItemsSource = RM;
        }

        private async void gvTeacher_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditTeacherWindow newTeacherWindow = new EditTeacherWindow();
                newTeacherWindow.Om = OperationMode.AddMode;

                if (newTeacherWindow.ShowDialog() == true)
                {
                    await bindTeacherList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加教师信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void UpdateTeacher_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TeacherModel selectTeacher = this.gvTeacher.SelectedItem as TeacherModel;

            if (selectTeacher != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    EditTeacherWindow newTeacherWindow = new EditTeacherWindow();
                    newTeacherWindow.SelectedTeacher = selectTeacher;
                    newTeacherWindow.Om = OperationMode.EditMode;

                    if (newTeacherWindow.ShowDialog() == true)
                    {
                        await bindTeacherList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新教师信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }

        private async void DeleteTeacher_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.gvTeacher.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的教师！", MessageDialogStyle.Affirmative, null);
                return;
            }

            MessageDialogResult delResult = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该教师信息吗？", MessageDialogStyle.AffirmativeAndNegative, null);

            if (delResult == MessageDialogResult.Affirmative)
            {
                TeacherModel selectTeacher = gvTeacher.SelectedItem as TeacherModel;

                if (selectTeacher != null)
                {
                    string strErrorMsg = string.Empty;
                    try
                    {
                        IAsyncProxy<ITeacherService> teacherServiceProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
                        if (selectTeacher.Id != 0)
                        {
                            //删除已经存在于数据库的数据，对于没有存于数据库的，则事件处理完成时都会刷新列表，故不用处理
                            bool blIsSuccess = await teacherServiceProxy.CallAsync(c => c.DeleteById(selectTeacher.Id));
                            if (blIsSuccess == true)
                            {
                                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除教师信息成功！", MessageDialogStyle.Affirmative, null);
                            }
                        }

                        await bindTeacherList();
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
                        await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除会籍顾问信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    }
                }
            }
        }

        private async void MoneyControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.gvTeacher.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要结算的教师！", MessageDialogStyle.Affirmative, null);
                return;
            }

            TeacherModel selectTeacher = gvTeacher.SelectedItem as TeacherModel;

            if (selectTeacher != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    CheckMoneyWindow cmw = new CheckMoneyWindow();
                    cmw.TeacherId = selectTeacher.Id;
                    cmw.TeacherName = selectTeacher.Name;
                    cmw.ShowDialog();
                    await bindTeacherList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "该教师费用结算失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }            
        }

        private async void gvTeacher_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            gvRecord.ItemsSource = null;
            gvRecord.Items.Clear();
            if (gvTeacher.SelectedItem != null)
            {
                TeacherModel tm = gvTeacher.SelectedItem as TeacherModel;
                IAsyncProxy<IClassesService> classesAsyncProxy = await Task.Run(() => ServiceHelper.GetClassService());
                IList<ClassRecordDisplayModel> RM = await classesAsyncProxy.CallAsync(c => c.FindClassRecordByTeacher(tm.Id));
                gvRecord.ItemsSource = RM;
            }
        }
    }
}
