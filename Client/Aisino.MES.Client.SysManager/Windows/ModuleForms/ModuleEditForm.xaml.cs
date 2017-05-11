using Aisino.MES.Client.Common;
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
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Windows.ModuleForms
{
    /// <summary>
    /// ModuleEditForm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleEditForm
    {
        private OperationMode _om;
        IAsyncProxy<IModuleModelService> _moduleAyncProxy;
        public ModuleDisplayModel _moduleDisplayModel;
        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }
        public ModuleEditForm()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Om == OperationMode.AddMode)
            {
                this.moduleMain.Title = "添加模块";
            }
            if (Om == OperationMode.EditMode)
            {
                this.moduleMain.Title = "修改模块";
                this.moduleCode.Text = this._moduleDisplayModel.Code;
                this.moduleName.Text = this._moduleDisplayModel.Name;
                this.moduleStopped.IsChecked = this._moduleDisplayModel.Stopped;
                this.moduleRemark.Text = this._moduleDisplayModel.Remark;
                this.moduleCode.IsEnabled = false;
                this.moduleName.IsEnabled = false;
            }
        }
        private async void getModuleEditModel(ModuleEditModel sysModule)
        {
            string strErrorMsg = string.Empty;
            try
            {
                //ModuleEditModel moduleEditModel = new ModuleEditModel();
                //moduleEditModel.Module = sysModule;
                if (Om==OperationMode.AddMode)
                {
                    _moduleAyncProxy =await Task.Run(()=>ServiceHelper.GetModuleService());
                    await _moduleAyncProxy.CallAsync(c => c.Add(sysModule));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增模块成功！");
                    //AisinoMessageBox.Show("新增模块成功！", UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    this.DialogResult = true;
                }
                else if (Om == OperationMode.EditMode)
                {
                    _moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
                    await _moduleAyncProxy.CallAsync(c => c.Update(sysModule));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改模块成功！");
                    //AisinoMessageBox.Show("修改模块成功！", UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    this.DialogResult = true;
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
                AisinoMessageBox.Show(strErrorMsg);
            }
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                ModuleEditModel sysModule = new ModuleEditModel();
                sysModule.ModuleCode = this.moduleCode.Text;
                sysModule.Name = this.moduleName.Text;
                sysModule.Stopped = this.moduleStopped.IsChecked.Value;
                sysModule.Remark = this.moduleRemark.Text;
                getModuleEditModel(sysModule);
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
                AisinoMessageBox.Show(strErrorMsg);
            }

        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalObjects.blOpenCancelWarning == true)
            {
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "是否确认取消操作？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    this.DialogResult = false;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }
    }
}
