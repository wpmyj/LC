using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Resources;
using LC.BaseManager.Windows;
using LC.Contracts.BaseManager;
using LC.Model;
using LC.Model.Business.BaseModel;
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
    /// ConsultantManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConsultantManagerPage : BusinessBasePage
    {
        public ConsultantManagerPage()
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
            await bindConsultantList();
        }

        private async Task bindConsultantList()
        {
            IAsyncProxy<IConsultantService> _consultantAyncProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
            IList<ConsultantModel> RM = await _consultantAyncProxy.CallAsync(c => c.GetAllConsultant());
            this.gvConsultant.ItemsSource = RM;
        }

        private async void gvConsultant_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditConsultantWindow newConsultantWindow = new EditConsultantWindow();
                newConsultantWindow.Om = OperationMode.AddMode;

                if (newConsultantWindow.ShowDialog() == true)
                {
                    await bindConsultantList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加会籍顾问信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void UpdateConsultant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConsultantModel selectConsultant = gvConsultant.SelectedItem as ConsultantModel;

            if (selectConsultant != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    EditConsultantWindow newConsultantWindow = new EditConsultantWindow();
                    newConsultantWindow.SelectConsultant = gvConsultant.SelectedItem as ConsultantModel;
                    newConsultantWindow.Om = OperationMode.EditMode;

                    if (newConsultantWindow.ShowDialog() == true)
                    {
                        await bindConsultantList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新会籍顾问信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }

        private async void DeleteConsultant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (gvConsultant.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的会籍顾问！", MessageDialogStyle.Affirmative, null);
                return;
            }

            MessageDialogResult delResult = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该会籍顾问信息吗？", MessageDialogStyle.AffirmativeAndNegative, null);

            if (delResult == MessageDialogResult.Affirmative)
            {
                ConsultantModel selectConsultant = gvConsultant.SelectedItem as ConsultantModel;

                if (selectConsultant != null)
                {
                    string strErrorMsg = string.Empty;
                    try
                    {
                        IAsyncProxy<IConsultantService> consultantServiceProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
                        if (selectConsultant.Id != 0)
                        {
                            //删除已经存在于数据库的数据，对于没有存于数据库的，则事件处理完成时都会刷新列表，故不用处理
                            bool blIsSuccess = await consultantServiceProxy.CallAsync(c => c.DeleteById(selectConsultant.Id));
                            if (blIsSuccess == true)
                            {
                                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除会籍顾问信息成功！", MessageDialogStyle.Affirmative, null);
                            }
                        }

                        await bindConsultantList();
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
            if (this.gvConsultant.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要结算的会籍顾问！", MessageDialogStyle.Affirmative, null);
                return;
            }

            ConsultantModel selectConsultant = gvConsultant.SelectedItem as ConsultantModel;

            if (selectConsultant != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    CheckConsultantMoneyWindow cmw = new CheckConsultantMoneyWindow();
                    cmw.ConsultantId = selectConsultant.Id;
                    cmw.ConsultantName = selectConsultant.Name;
                    cmw.ShowDialog();
                    await bindConsultantList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "该会籍顾问费用结算失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }
    }
}
