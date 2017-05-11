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
    /// FunctionEditForm.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionEditForm
    {
        public ModuleDisplayModel _moduleDisplayModel;
        public FunctionEditModel _sysFunction;
        private OperationMode _om;
        private IAsyncProxy<IFunctionModelService> _functionAyncProxy;
        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }
        public FunctionEditForm()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmbFunctionType.ItemsSource = Enum.GetValues(typeof(FunctionType));
            if (Om == OperationMode.AddMode)
            {
                this.FunMain.Title = "添加方法";
                this.cmbFunctionType.SelectedIndex = 0;
            }
            if (Om == OperationMode.EditMode)
            {
                this.FunMain.Title = "修改方法";
                this.txtOperationCode.Text = this._sysFunction.OperationCode;
                this.txtOperationName.Text = this._sysFunction.OperationName;
                this.txtAssembly.Text = this._sysFunction.Assembly;
                this.txtClassName.Text = this._sysFunction.ClassName;
                this.txtFunctionCode.Text = this._sysFunction.FunctionCode;
                this.txtFunctionName.Text = this._sysFunction.Name;
                this.txtParams.Text = this._sysFunction.Params;
                this.txtRemark.Text = this._sysFunction.Remark;
                this.cmbFunctionType.Text = this._sysFunction.Type.ToString();
                this.txtFunctionCode.IsEnabled = false;
                this.txtFunctionName.IsEnabled = false;
            }
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                FunctionEditModel sysFunction = new FunctionEditModel();
                sysFunction.ModuleCode = this._moduleDisplayModel.Code;
                sysFunction.OperationCode = this.txtOperationCode.Text;
                sysFunction.OperationName = this.txtOperationName.Text;
                sysFunction.Assembly = this.txtAssembly.Text;
                sysFunction.ClassName = this.txtClassName.Text;
                sysFunction.FunctionCode = this.txtFunctionCode.Text;
                sysFunction.Name = this.txtFunctionName.Text;
                sysFunction.Params = this.txtParams.Text;
                sysFunction.Remark = this.txtRemark.Text;
                FunctionType functionType = (FunctionType)Enum.Parse(typeof(FunctionType), this.cmbFunctionType.Text);
                sysFunction.Type = functionType;
                _functionAyncProxy = await Task.Run(()=>ServiceHelper.GetFunctionService());
                if (Om==OperationMode.AddMode)
                {
                    await _functionAyncProxy.CallAsync(c => c.Add(sysFunction));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增方法成功！");
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "新增方法成功！", MessageDialogStyle.Affirmative, null); ;
                    this.DialogResult = true;
                }
               if (Om == OperationMode.EditMode)
                {
                    await _functionAyncProxy.CallAsync(c => c.Update(sysFunction));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改方法成功！");
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "修改方法成功！", MessageDialogStyle.Affirmative, null);
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
