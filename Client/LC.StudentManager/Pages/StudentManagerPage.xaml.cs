using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Resources;
using LC.Contracts.StudentManager;
using LC.Model;
using LC.Model.Business.StudentModel;
using LC.StudentManager.Windows;
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

namespace LC.StudentManager.Pages
{
    /// <summary>
    /// StudentManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class StudentManagerPage : BusinessBasePage
    {
        public StudentManagerPage()
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

        private async Task bindStudentList()
        {
            IAsyncProxy<IStudentService> studentAyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());
            IList<StudentDisplayModel> RM = await studentAyncProxy.CallAsync(c => c.GetAll());
            this.gvStudent.ItemsSource = RM;
        }

        private async void gvStudent_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditStudentWindow newStudentWindow = new EditStudentWindow();
                newStudentWindow.Om = OperationMode.AddMode;

                if (newStudentWindow.ShowDialog() == true)
                {
                    await bindStudentList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加学员信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void UpdateConsultant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StudentDisplayModel selectStudent = this.gvStudent.SelectedItem as StudentDisplayModel;

            if (selectStudent != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    EditStudentWindow newStudentWindow = new EditStudentWindow();
                    newStudentWindow.StudentId = selectStudent.Id;
                    newStudentWindow.Om = OperationMode.EditMode;

                    if (newStudentWindow.ShowDialog() == true)
                    {
                        await bindStudentList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新学员失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }

        private async void DeleteConsultant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.gvStudent.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的学员！", MessageDialogStyle.Affirmative, null);
                return;
            }

            MessageDialogResult delResult = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该学员吗？", MessageDialogStyle.AffirmativeAndNegative, null);

            if (delResult == MessageDialogResult.Affirmative)
            {
                StudentDisplayModel selectStudent = this.gvStudent.SelectedItem as StudentDisplayModel;

                if (selectStudent != null)
                {
                    string strErrorMsg = string.Empty;
                    try
                    {
                        IAsyncProxy<IStudentService> studentAyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());
                        if (selectStudent.Id != 0)
                        {
                            //删除已经存在于数据库的数据，对于没有存于数据库的，则事件处理完成时都会刷新列表，故不用处理
                            bool blIsSuccess = await studentAyncProxy.CallAsync(c => c.DeleteById(selectStudent.Id));
                            if (blIsSuccess == true)
                            {
                                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除学员成功！", MessageDialogStyle.Affirmative, null);
                            }
                        }

                        await bindStudentList();
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
                        await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除学员失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    }
                }
            }

        }

        private async void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            await bindStudentList();
        }

        private async void MoneyControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StudentDisplayModel selectStudent = this.gvStudent.SelectedItem as StudentDisplayModel;

            if (selectStudent != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    StudentRechargeWindow srw = new StudentRechargeWindow();
                    srw.StudentId = selectStudent.Id;
                    if (srw.ShowDialog() == true)
                    {
                        await bindStudentList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新学员失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }
    }
}
