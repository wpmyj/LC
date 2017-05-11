using Aisino.MES.Client.Common;
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

namespace Aisino.MES.Client.SysManager.Windows.SubSystemForms
{
    /// <summary>
    /// SubSystemEditForm.xaml 的交互逻辑
    /// </summary>
    public partial class SubSystemEditForm
    {
        public OperationMode OM { get; set; }
        public SubSystemDisplayModel _subSystemDisplayModel { get; set; }
        private IAsyncProxy<ISubSystemModelService> _SubSystemAyncProxy;
        private SubSystemEditModel _sysSubSystem;
        public SubSystemEditForm()
        {
            InitializeComponent();
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmbMetroType.ItemsSource = Enum.GetValues(typeof(MetroTypes));
            if (this.OM == OperationMode.AddMode)
            {
                _sysSubSystem = new SubSystemEditModel();
                this.gdMenu.DataContext = this._sysSubSystem;
            }
            if (this.OM == OperationMode.EditMode)
            {
                this.GetSysSubSystem(this._subSystemDisplayModel);
            }

        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                this._SubSystemAyncProxy = await Task.Run(() => ServiceHelper.GetSubSystemService());

                if (this.OM == OperationMode.AddMode)
                {
                    await this._SubSystemAyncProxy.CallAsync(c => c.Add(_sysSubSystem));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增子菜单成功！");
                    this.DialogResult = true;
                }
                else if (this.OM == OperationMode.EditMode)
                {
                    await this._SubSystemAyncProxy.CallAsync(c => c.Update(_sysSubSystem));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改子菜单成功！");
                    this.DialogResult = true;
                }
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "子菜单编辑失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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

        private async void GetSysSubSystem(SubSystemDisplayModel subsys)
        {
            string strErrorMsg = string.Empty;
            try
            {
                this._SubSystemAyncProxy = await Task.Run(() => ServiceHelper.GetSubSystemService());
                SubSystemEditModel subSystemEditModel = await this._SubSystemAyncProxy.CallAsync(c => c.GetSubSystemByCode(subsys.SubSystemCode));
                if (subSystemEditModel != null)
                {
                    this._sysSubSystem = subSystemEditModel;
                }
                this.gdMenu.DataContext = this._sysSubSystem;
                this.gdMenu.DataContext = subSystemEditModel;
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "修改模式绑定失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private void xColorPicker_DropDownClosed(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            txtColorName.Text = this.xColorPicker.SelectedColor.ToString();
        }

        private void xColorPickerTwo_DropDownClosed(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            txtColorNameTwo.Text = this.xColorPickerTwo.SelectedColor.ToString();
        }
    }
}
