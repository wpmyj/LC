using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
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
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace LC.BaseManager.Windows
{
    /// <summary>
    /// EditCenterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCenterWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private CenterModel selectCenter;

        public CenterModel SelectCenter
        {
            get { return selectCenter; }
            set { selectCenter = value; }
        }

        IAsyncProxy<ICenterService> centerAsyncProxy = null;

        public EditCenterWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                centerAsyncProxy = await Task.Run(() => ServiceHelper.GetCenterService());

                IAsyncProxy<IConsultantService> consultantAyncProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
                IList<ConsultantModel> consultantLists = await consultantAyncProxy.CallAsync(c => c.GetAllConsultant());
                this.cmbConsultant.ItemsSource = consultantLists;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }

            if (Om == OperationMode.AddMode)
            {
                this.Title = "AddCenter";
                txtName.IsEnabled = true;
                txtMobile.IsEnabled = true;
                BindCenterInfo(null);
            }
            else
            {
                this.Title = "EditCenter";
                txtName.IsEnabled = false;
                txtMobile.IsEnabled = true;
                cmbConsultant.Text = selectCenter.ConsultantName;
                BindCenterInfo(SelectCenter);
            }
        }

        private async void BindCenterInfo(CenterModel selectCenter)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (selectCenter != null)
                {
                    txtName.Text = selectCenter.Name;
                    txtMobile.Text = selectCenter.Phone;
                    txtAddress.Text = selectCenter.Address;
                    if(selectCenter.ConsultantId != 0)
                    {
                        //绑定主要管理的会籍顾问
                        cmbConsultant.SelectedValue = selectCenter.Id;
                    }
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定用户信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            #region 新增
            if (Om == OperationMode.AddMode)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    CenterModel newCenterModel = new CenterModel();
                    newCenterModel.Name = txtName.Text.Trim();
                    newCenterModel.Address = txtAddress.Text.Trim();
                    newCenterModel.Phone = txtMobile.Text.Trim();
                    if(cmbConsultant.SelectedItem != null)
                    {
                        newCenterModel.ConsultantId = ((ConsultantModel)this.cmbConsultant.SelectedItem).Id;
                    }
                    else
                    {
                        newCenterModel.ConsultantId = 0;
                    }

                    newCenterModel = await centerAsyncProxy.CallAsync(c => c.Add(newCenterModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增教学中心成功！");
                    this.DialogResult = true;
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
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增中心信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    selectCenter.Name = txtName.Text;
                    selectCenter.Phone = txtMobile.Text;
                    selectCenter.Address = txtAddress.Text;
                    if (cmbConsultant.SelectedItem != null)
                    {
                        selectCenter.ConsultantId = ((ConsultantModel)this.cmbConsultant.SelectedItem).Id;
                    }
                    else
                    {
                        selectCenter.ConsultantId = 0;
                    }
                    selectCenter = await centerAsyncProxy.CallAsync(c => c.Update(selectCenter));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改教学中心成功！");
                    this.DialogResult = true;
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
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改中心信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }
            #endregion
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
