using Aisino.MES.Client.Common;
using Aisino.MES.Client.SysManager.Windows.ModuleForms;
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
    /// ModuleManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleManagerPage : BusinessBasePage
    {
        private ModuleDisplayModel _moduleDisplayModel;
        private FunctionEditModel _sysFunction;
        private IAsyncProxy<IModuleModelService> _moduleAyncProxy;
        public ModuleManagerPage()
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

        private void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            bindModuleList();
        }
        private async void bindModuleList()
        {
            _moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
            IList<ModuleDisplayModel> RM = await _moduleAyncProxy.CallAsync(c => c.GetAllModules());
            this.moduleList.ItemsSource = RM;
        }
        private void AddModule_Click(object sender, RoutedEventArgs e)
        {
            ModuleEditForm moduleEditForm = new ModuleEditForm();
            moduleEditForm.Om = OperationMode.AddMode;
            if (moduleEditForm.ShowDialog() == true)
            {
                this.bindModuleList();
            }
        }

        private async void UpdateModule_Click(object sender, RoutedEventArgs e)
        {
            if (this._moduleDisplayModel == null || this.moduleList.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择模块请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.moduleList.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个模块，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            ModuleEditForm moduleEditForm = new ModuleEditForm();
            moduleEditForm.Om = OperationMode.EditMode;
            moduleEditForm._moduleDisplayModel = this._moduleDisplayModel;
            if (moduleEditForm.ShowDialog() == true)
            {
                this.bindModuleList();
            }
        }

        private async void DeleModule_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this._moduleDisplayModel == null || this.moduleList.SelectedItems.Count == 0)
                {
                    await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择模块请选择！", MessageDialogStyle.Affirmative, null);
                    return;
                }
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确认删除所选的模块吗！", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    
                    _moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
                    foreach (ModuleDisplayModel item in this.moduleList.SelectedItems)
                    {
                        await this._moduleAyncProxy.CallAsync(c => c.DeleteByCode(item.Code));
                    }
                    this._moduleDisplayModel = null;
                    this.bindModuleList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "删除模块失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void AddFun_Click(object sender, RoutedEventArgs e)
        {
            if (this.moduleList.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择模块请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.moduleList.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个模块，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            FunctionEditForm functionEditForm = new FunctionEditForm();
            functionEditForm.Om = OperationMode.AddMode;
            functionEditForm._moduleDisplayModel = this._moduleDisplayModel;
            functionEditForm._sysFunction = this._sysFunction;
            if (functionEditForm.ShowDialog() == true)
            {
                await bindModuleCode();
            }
        }

        private async void UpdateFun_Click(object sender, RoutedEventArgs e)
        {
            if (this.moduleList.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择模块请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.moduleList.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个模块，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this._sysFunction == null || this.moduleCode.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择方法请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.moduleCode.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个方法，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            FunctionEditForm functionEditForm = new FunctionEditForm();
            functionEditForm.Om = OperationMode.EditMode;
            functionEditForm._moduleDisplayModel = this._moduleDisplayModel;
            functionEditForm._sysFunction = this._sysFunction;
            if (functionEditForm.ShowDialog() == true)
            {
                await bindModuleCode();
            }
        }

        private async void DeleFun_Click(object sender, RoutedEventArgs e)
        {
            if (this.moduleList.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择模块请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.moduleList.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个模块，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this._sysFunction == null || this.moduleCode.SelectedItems.Count == 0)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "没有选择方法请选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            if (this.moduleCode.SelectedItems.Count > 1)
            {
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "不能选择多个方法，请重新选择！", MessageDialogStyle.Affirmative, null);
                return;
            }
            MessageDialogResult result=await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgInfo, "确认删除所选的方法吗！", MessageDialogStyle.AffirmativeAndNegative, null);
            if (result == MessageDialogResult.Affirmative)
            {
                IAsyncProxy<IFunctionModelService> _functionAyncProxy = await Task.Run(() => ServiceHelper.GetFunctionService());
                await _functionAyncProxy.CallAsync(c => c.DeleteByCode(_sysFunction.FunctionCode));
                this._sysFunction = null;
                await bindModuleCode();
            }
        }

        private async void moduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.moduleList.SelectedItems.Count == 0)
                {
                    return;
                }
                this._moduleDisplayModel = (ModuleDisplayModel)this.moduleList.SelectedItem;
                ModuleDisplayModel(_moduleDisplayModel);
                await bindModuleCode();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "选取数据失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
        private async Task bindModuleCode()
        {
            string strErrorMsg = string.Empty;
            try
            {
                this._moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
                ModuleEditModel moduleEditModel = await this._moduleAyncProxy.CallAsync(f => f.GetModuleByCode(this._moduleDisplayModel.Code));
                this.moduleCode.ItemsSource = moduleEditModel.SysFunctions;
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "绑定数据失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
        private void ModuleDisplayModel(ModuleDisplayModel module)
        {
            this.labCode.Content = module.Code;
            this.labName.Content = module.Name;
            this.cbxStatus.IsChecked = module.Stopped;
            this.txtRemark.Text = module.Remark;
        }

        private void moduleCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.moduleCode.SelectedItems.Count == 0)
            {
                return;
            }
            this._sysFunction = (FunctionEditModel)this.moduleCode.SelectedItem;
        }
    }
}
