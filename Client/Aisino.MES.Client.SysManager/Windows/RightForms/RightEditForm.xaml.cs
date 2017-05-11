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
using WcfClientProxyGenerator;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Windows.RightManagerWin
{
    /// <summary>
    /// AddWin.xaml 的交互逻辑
    /// </summary>
    public partial class EditRightForm 
    {
        private OperationMode _om;
       
        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }
        public RightEditModel _sysRight { get; set; }
        public string _sysCode { get; set; }
        public EditRightForm()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Om == OperationMode.AddMode)
            {
                this.rightMain.Title = "添加权限";
            }
            if (Om == OperationMode.EditMode)
            {
                this.rightMain.Title = "修改权限";
                getRightEditModel(this._sysCode);
            }
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this._sysRight = new RightEditModel();
            this._sysRight.RightCode = this.rightCode.Text;
            this._sysRight.Name = this.rightName.Text;
            this._sysRight.Remark = this.rightRemark.Text;
            this._sysRight.Stopped = this.rightStopped.IsChecked;
            if (Om == OperationMode.AddMode)
            {
                this.rightAdd(this._sysRight);
            }
            if (Om == OperationMode.EditMode)
            {
                rightUpdate(this._sysRight);
            }
        }
        private async void rightAdd(RightEditModel sysRight)
        {
            string strErrorMsg = string.Empty;
            try
            {
                IAsyncProxy<IRightModelService> asyncProxyRight = await Task.Run(() => ServiceHelper.GetRightService());
                await asyncProxyRight.CallAsync(d => d.Add(sysRight));
                //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "添加权限成功！", MessageDialogStyle.Affirmative, null);
                this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "添加权限成功！");
                this.DialogResult = true;
                this.Close();
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "添加权限失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strErrorMsg);
            }

        }

        private async void rightUpdate(RightEditModel sysRight)
        {
            string strErrorMsg = string.Empty;
            try
            {
                IAsyncProxy<IRightModelService> asyncProxyRight = await Task.Run(() => ServiceHelper.GetRightService());
                await asyncProxyRight.CallAsync(d => d.Update(sysRight));
                //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "修改权限成功！", MessageDialogStyle.Affirmative, null);
                this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改权限成功！");
                this.DialogResult = true;
                this.Close();
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改权限失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strMsg);
            }
        }

        private async void getRightEditModel(string sysCode)
        {
            string strErrorMsg = string.Empty;
            try
            {
                IAsyncProxy<IRightModelService> asyncProxyRight = await Task.Run(() => ServiceHelper.GetRightService());
                RightEditModel _rightEditModel = await asyncProxyRight.CallAsync(c => c.GetRightByCode(_sysCode));
                this.rightCode.Text = _rightEditModel.RightCode;
                this.rightCode.IsEnabled = false;
                this.rightName.Text = _rightEditModel.Name;
                this.rightName.IsEnabled = false;
                this.rightRemark.Text = _rightEditModel.Remark;
                this.rightStopped.IsChecked = _rightEditModel.Stopped;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "获取权限修改实例失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show(strMsg);
            }
        }
        private async void btnCancle_Click(object sender, RoutedEventArgs e)
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
