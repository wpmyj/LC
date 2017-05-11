using System;
using System.Collections.Generic;
using System.Linq;
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
using MahApps.Metro.Controls;
using Aisino.MES.Client.Common;
using Aisino.MES.Model.Entities;
using Aisino.MES.Service.Contracts.SysManager;
using System.ServiceModel;
using WcfClientProxyGenerator;
using WcfClientProxyGenerator.Async;
using Aisino.MES.Model.Business;
using Aisino.MES.Resources;
using Aisino.MES.Model;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using MahApps.Metro.Controls.Dialogs;

namespace Aisino.MES.Client.SysManager.Windows.DeptAndUser
{
    /// <summary>
    /// EditDeptWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditDeptWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private bool _isParent;

        public bool IsParent
        {
            get { return _isParent; }
            set { _isParent = value; }
        }

        private DepartmentEditModel parentSysDepartment;

        public DepartmentEditModel ParentSysDepartment
        {
            get { return parentSysDepartment; }
            set { parentSysDepartment = value; }
        }

        private DepartmentEditModel selectSysDepartment;

        public DepartmentEditModel SelectSysDepartment
        {
            get { return selectSysDepartment; }
            set { selectSysDepartment = value; }
        }

        IAsyncProxy<IDepartmentModelService> asyncProxy = null;

        public List<DepartmentDisplayModel> lstDepartments;
        public EditDeptWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                asyncProxy = await Task.Run(() => ServiceHelper.GetDepartmentService());
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化窗体失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("创建部门WCF代理失败！原因：" + strMsg, UIResources.MsgError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            if (Om == OperationMode.AddMode)
            {
                this.Title = "新增部门";
                txtDeptCode.IsEnabled = true;
                chkIsStoped.IsEnabled = false;
                if (IsParent)
                {
                    BindDepartment(null);
                }
                else
                {
                    BindDepartment(SelectSysDepartment);
                }
            }
            else
            {
                this.Title = "修改部门";
                txtDeptCode.IsEnabled = false;
                txtName.IsEnabled = false;
                chkIsStoped.IsEnabled = true;
                BindDepartment(SelectSysDepartment);
            }
        }

        private async void BindDepartment(DepartmentEditModel selectDepartment)
        {
            string strErrorMsg = string.Empty;
            try
            {
                lstDepartments = await asyncProxy.CallAsync(c => c.GetAllDisplayModel());
                if (lstDepartments != null && lstDepartments.Count > 0)
                {
                    cmbFatherDept.ItemsSource = lstDepartments;
                    cmbFatherDept.DisplayMemberPath = "Name";
                    cmbFatherDept.SelectedIndex = 0;
                }
                if (this.Om == OperationMode.EditMode)
                {
                    if (selectDepartment != null)
                    {
                        foreach (DepartmentDisplayModel deptItem in cmbFatherDept.Items)
                        {
                            if (deptItem.Code == selectDepartment.ParentCode)
                            {
                                cmbFatherDept.SelectedItem = deptItem;
                                break;
                            }
                        }

                        txtDeptCode.Text = selectDepartment.DepartmentCode == null ? "" : selectDepartment.DepartmentCode.Trim();
                        txtName.Text = selectDepartment.Name == null ? "" : selectDepartment.Name.Trim();
                        txtShowIndex.Value = selectDepartment.ShowIndex;
                        txtRemark.Text = selectDepartment.Remark == null ? "" : selectDepartment.Remark.Trim();
                        chkIsRealDept.IsChecked = selectDepartment.IsRealy;
                        chkIsStoped.IsChecked = selectDepartment.Stopped;
                    }
                }
                else if (this.Om == OperationMode.AddMode)
                {
                    if (selectDepartment != null)
                    {
                        cmbFatherDept.Text = selectDepartment.Name;
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定部门信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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
                    DepartmentEditModel newEditDepartment = new DepartmentEditModel();
                    newEditDepartment.DepartmentCode = txtDeptCode.Text.Trim();
                    newEditDepartment.Name = txtName.Text.Trim();
                    newEditDepartment.ShowIndex = Convert.ToInt16(txtShowIndex.Value.Value);
                    newEditDepartment.Remark = txtRemark.Text.Trim();
                    newEditDepartment.IsRealy = chkIsRealDept.IsChecked.HasValue ? chkIsRealDept.IsChecked.Value : false;

                    DepartmentDisplayModel selectDept = cmbFatherDept.SelectedItem as DepartmentDisplayModel;
                    if (selectDept != null)
                    {
                        newEditDepartment.ParentCode = selectDept.Code;
                    }

                    newEditDepartment = await asyncProxy.CallAsync(c => c.Add(newEditDepartment, IsParent));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增部门成功！");
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "新增部门成功！", MessageDialogStyle.Affirmative, null);
                    this.DialogResult = true;

                    #region 延时1s的弹窗
                    ////this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;
                    //this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增部门成功！");
                    #endregion
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增部门失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("新增部门失败！原因：" + strMsg, UIResources.MsgError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
            #endregion

            #region 修改
            else
            {
                SelectSysDepartment.Name = txtName.Text.Trim();
                SelectSysDepartment.ShowIndex = Convert.ToInt16(txtShowIndex.Value.Value);
                SelectSysDepartment.Remark = txtRemark.Text.Trim();
                SelectSysDepartment.IsRealy = chkIsRealDept.IsChecked.HasValue ? chkIsRealDept.IsChecked.Value : false;
                SelectSysDepartment.Stopped = chkIsStoped.IsChecked.HasValue ? chkIsStoped.IsChecked.Value : false;
                DepartmentDisplayModel selectDept = cmbFatherDept.SelectedItem as DepartmentDisplayModel;
                if (selectDept != null)
                {
                    SelectSysDepartment.ParentCode = selectDept.Code;
                }

                string strErrorMsg = string.Empty;

                try
                {
                    SelectSysDepartment = await asyncProxy.CallAsync(c => c.Update(SelectSysDepartment));
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "修改部门成功！", MessageDialogStyle.Affirmative, null);
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改部门成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改部门失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("修改部门失败！原因：" + strMsg, UIResources.MsgError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
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
