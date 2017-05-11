using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Resources;
using LC.ClassesManager.Windows;
using LC.Contracts.ClassManager;
using LC.Model;
using LC.Model.Business.ClassModel;
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

namespace LC.ClassesManager.Pages
{
    /// <summary>
    /// ClassTypeAndClassManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClassTypeAndClassManagerPage : BusinessBasePage
    {
        private ClassTypeModel selectClassType = null;
        public ClassTypeAndClassManagerPage()
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
            await bindClassTypeList();
        }

        private async Task bindClassTypeList()
        {
            IAsyncProxy<IClassTypeService> _classtypeAyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
            IList<ClassTypeModel> RM = await _classtypeAyncProxy.CallAsync(c => c.GetAllClassType());
            this.gvClassTypes.ItemsSource = RM;
        }

        private async Task bindClassList()
        {
            IAsyncProxy<IClassesService> classesAsyncProxy = await Task.Run(() => ServiceHelper.GetClassService());
            IList<ClassDisplayModel> RM = await classesAsyncProxy.CallAsync(c => c.FindClassByClassType(selectClassType.Id));
            this.gvClasses.ItemsSource = RM;
        }

        private async Task bindSchemasList()
        {
            IAsyncProxy<IClassTypeService> _classtypeAyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
            IList<SchemasEditModel> RM = await _classtypeAyncProxy.CallAsync(c => c.FindSchemasByClassType(selectClassType.Id));
            this.gvSchemas.ItemsSource = RM;
        }

        #region classmanager
        private async void UpdateClass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClassDisplayModel selectClass = this.gvClasses.SelectedItem as ClassDisplayModel;

            if (selectClass != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    EditClassWindow newClassWindow = new EditClassWindow();
                    newClassWindow.ClassId = selectClass.Id;
                    newClassWindow.Om = OperationMode.EditMode;

                    if (newClassWindow.ShowDialog() == true)
                    {
                        await bindClassList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }

        private async void DeleteClass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (gvClasses.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的班级！", MessageDialogStyle.Affirmative, null);
                return;
            }

            MessageDialogResult delResult = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该班级吗？", MessageDialogStyle.AffirmativeAndNegative, null);

            if (delResult == MessageDialogResult.Affirmative)
            {
                ClassDisplayModel selectClass = this.gvClasses.SelectedItem as ClassDisplayModel;

                if (selectClass != null)
                {
                    string strErrorMsg = string.Empty;
                    try
                    {
                        IAsyncProxy<IClassesService> classAyncProxy = await Task.Run(() => ServiceHelper.GetClassService());
                        if (selectClass.Id != 0)
                        {
                            //删除已经存在于数据库的数据，对于没有存于数据库的，则事件处理完成时都会刷新列表，故不用处理
                            bool blIsSuccess = await classAyncProxy.CallAsync(c => c.DeleteById(selectClass.Id));
                            if (blIsSuccess == true)
                            {
                                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除班级成功！", MessageDialogStyle.Affirmative, null);
                            }
                        }

                        await bindClassList();
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
                        await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    }
                }
            }
        }

        private async void gvClasses_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            if(gvClassTypes.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要添加班级的班级类型！", MessageDialogStyle.Affirmative, null);
                return;
            }
            try
            {
                EditClassWindow newClassWindow = new EditClassWindow();
                newClassWindow.Om = OperationMode.AddMode;
                newClassWindow.ClassTypeId = this.selectClassType.Id;

                if (newClassWindow.ShowDialog() == true)
                {
                    await bindClassList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }


        #endregion

        #region classtype
        private async void gvClassTypes_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                EditClassTypeWindow newClassTypeWindow = new EditClassTypeWindow();
                newClassTypeWindow.Om = OperationMode.AddMode;

                if (newClassTypeWindow.ShowDialog() == true)
                {
                    await bindClassTypeList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加班级类型失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void UpdateClassType_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClassTypeModel selectClassType = this.gvClassTypes.SelectedItem as ClassTypeModel;

            if (selectClassType != null)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    EditClassTypeWindow newClassTypeWindow = new EditClassTypeWindow();
                    newClassTypeWindow.SelectClassType = selectClassType;
                    newClassTypeWindow.Om = OperationMode.EditMode;

                    if (newClassTypeWindow.ShowDialog() == true)
                    {
                        await bindClassTypeList();
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
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "更新班级类型失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
        }

        private async void DeleteClassType_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (gvClassTypes.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要删除的班级类型！", MessageDialogStyle.Affirmative, null);
                return;
            }

            MessageDialogResult delResult = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确定删除该班级类型吗？", MessageDialogStyle.AffirmativeAndNegative, null);

            if (delResult == MessageDialogResult.Affirmative)
            {
                ClassTypeModel selectClassType = this.gvClassTypes.SelectedItem as ClassTypeModel;

                if (selectClassType != null)
                {
                    string strErrorMsg = string.Empty;
                    try
                    {
                        IAsyncProxy<IClassTypeService> _classtypeAyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
                        if (selectClassType.Id != 0)
                        {
                            //删除已经存在于数据库的数据，对于没有存于数据库的，则事件处理完成时都会刷新列表，故不用处理
                            bool blIsSuccess = await _classtypeAyncProxy.CallAsync(c => c.DeleteById(selectClassType.Id));
                            if (blIsSuccess == true)
                            {
                                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "删除班级类型成功！", MessageDialogStyle.Affirmative, null);
                            }
                        }

                        await bindClassTypeList();
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
                        await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除班级类型失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    }
                }
            }
        }

        private async void gvClassTypes_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            if (gvClassTypes.SelectedItem != null)
            {
                this.selectClassType = gvClassTypes.SelectedItem as ClassTypeModel;
                await bindClassList();
                await bindSchemasList();
            }
        }
        #endregion

        private async void gvSchemas_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            string strErrorMsg = string.Empty;
            if (gvClassTypes.SelectedItem == null)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "请选择需要添加schemas的班级类型！", MessageDialogStyle.Affirmative, null);
                return;
            }
            try
            {
                EditSchemasWindow newSchemasWindow = new EditSchemasWindow();
                newSchemasWindow.Om = OperationMode.AddMode;
                newSchemasWindow.ClassTypeId = this.selectClassType.Id;

                if (newSchemasWindow.ShowDialog() == true)
                {
                    await bindSchemasList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "添加班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private void DeleteSchemas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void UpdateSchemas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        
    }
}
