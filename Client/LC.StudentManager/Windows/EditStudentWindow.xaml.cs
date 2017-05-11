using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.BaseManager;
using LC.Contracts.ClassManager;
using LC.Contracts.StudentManager;
using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Model.Business.ClassModel;
using LC.Model.Business.StudentModel;
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
using Telerik.Windows.Controls;
using WcfClientProxyGenerator.Async;

namespace LC.StudentManager.Windows
{
    /// <summary>
    /// StudentEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditStudentWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private int studentId;

        public int StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }

        IAsyncProxy<IStudentService> studentAsyncProxy = null;

        StudentEditModel studentEditModel = null;

        public EditStudentWindow()
        {
            InitializeComponent();
        }


        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string name = ClassTypeTree.Name;
            if(cmbConsultant.SelectedItem == null)
            {
                this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "请选择该学员会籍顾问！");
                return;
            }
            if(cmbStatus.SelectedItem == null)
            {
                this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "请选择该学员状态！");
                return;
            }
            #region 新增
            if (Om == OperationMode.AddMode)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    StudentEditModel newStudentModel = new StudentEditModel();
                    newStudentModel.Name = txtName.Text.Trim();
                    newStudentModel.Address = txtAddress.Text.Trim();
                    newStudentModel.Birthdate = dateBirth.DisplayDate;
                    newStudentModel.ConsultantId = (cmbConsultant.SelectedItem as ConsultantModel).Id;
                    newStudentModel.Dadsname = txtDadsName.Text.Trim();
                    newStudentModel.Dadsphone = txtDadsPhone.Text.Trim();
                    newStudentModel.Email = txtEmail.Text.Trim();
                    newStudentModel.Grade = txtGrade.Text.Trim();
                    newStudentModel.Momsname = txtMomsName.Text.Trim();
                    newStudentModel.Momsphone = txtMomsPhone.Text.Trim();
                    newStudentModel.Nickname = txtNickName.Text.Trim();
                    newStudentModel.OriginalClass = txtOriginalClass.Text.Trim();
                    newStudentModel.RelationShip = txtRelationShip.Text.Trim();
                    newStudentModel.School = txtSchool.Text.Trim();
                    newStudentModel.StatusId = (cmbStatus.SelectedItem as StatusModel).Id;
                    newStudentModel.ClassesId = new List<int>();
                    newStudentModel.ConsultantRate = Convert.ToDecimal(numConsRate.Value);
                    foreach (ClassDisplayModel cdm in ClassTypeTree.SelectedItems)
                    {
                        newStudentModel.ClassesId.Add(cdm.Id);
                    }                        

                    newStudentModel = await studentAsyncProxy.CallAsync(c => c.Add(newStudentModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增学员成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增学员失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    studentEditModel.Address = txtAddress.Text.Trim();
                    studentEditModel.Birthdate = dateBirth.DisplayDate;
                    studentEditModel.ConsultantId = (cmbConsultant.SelectedItem as ConsultantModel).Id;
                    studentEditModel.Dadsname = txtDadsName.Text.Trim();
                    studentEditModel.Dadsphone = txtDadsPhone.Text.Trim();
                    studentEditModel.Email = txtEmail.Text.Trim();
                    studentEditModel.Grade = txtGrade.Text.Trim();
                    studentEditModel.Momsname = txtMomsName.Text.Trim();
                    studentEditModel.Momsphone = txtMomsPhone.Text.Trim();
                    studentEditModel.Nickname = txtNickName.Text.Trim();
                    studentEditModel.OriginalClass = txtOriginalClass.Text.Trim();
                    studentEditModel.RelationShip = txtRelationShip.Text.Trim();
                    studentEditModel.School = txtSchool.Text.Trim();
                    studentEditModel.StatusId = (cmbStatus.SelectedItem as StatusModel).Id;
                    studentEditModel.Name = txtName.Text.Trim();
                    studentEditModel.ConsultantRate = Convert.ToDecimal(numConsRate.Value);
                    studentEditModel.ClassesId = new List<int>();

                    foreach (ClassDisplayModel cdm in ClassTypeTree.SelectedItems)
                    {
                        studentEditModel.ClassesId.Add(cdm.Id);
                    }  

                    studentEditModel = await studentAsyncProxy.CallAsync(c => c.Update(studentEditModel));

                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改学员成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改学员失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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

        private async void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                studentAsyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());

                await bindConsultantList();
                await bindStatusList();
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }

            if (Om == OperationMode.AddMode)
            {
                this.Title = "AddStudent";
                //txtName.IsEnabled = true;
                BindStudentInfo();
            }
            else
            {
                this.Title = "EditStudent";
                IAsyncProxy<IUserModelService> userAsyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
                if (userAsyncProxy.CallAsync(u => u.CheckUserRole(GlobalObjects.currentLoginUser.UserCode, "consultant")).Result)
                {
                    numConsRate.IsEnabled = false;
                }
                BindStudentInfo();
            }
        }

        private async Task bindConsultantList()
        {
            IAsyncProxy<IConsultantService> _consultantAyncProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
            IList<ConsultantModel> RM = await _consultantAyncProxy.CallAsync(c => c.GetAllConsultant());
            this.cmbConsultant.ItemsSource = RM;
        }

        private async Task bindStatusList()
        {
            IAsyncProxy<IStatusService> _statusAyncProxy = await Task.Run(() => ServiceHelper.GetStatusService());
            IList<StatusModel> RM = await _statusAyncProxy.CallAsync(c => c.FindStatusByCat(GlobalObjects.StudentStatus));
            this.cmbStatus.ItemsSource = RM;
        }

        private async Task bindClassTypeList()
        {
            IAsyncProxy<IClassTypeService> _classtypeAyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
            IList<ClassTypeModel> RM = await _classtypeAyncProxy.CallAsync(c => c.GetAllClassTypeWithClasses());
            this.ClassTypeTree.ItemsSource = RM;

            this.ClassTypeTree.ExpandAll();
        }

        private async void BindStudentInfo()
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (studentId != 0)
                {
                    //选择了修改模式
                    studentEditModel = await studentAsyncProxy.CallAsync(c => c.GetStudentById(this.studentId));
                    txtName.Text = studentEditModel.Name;
                    txtNickName.Text = studentEditModel.Nickname;
                    dateBirth.Text = studentEditModel.Birthdate.ToString("MM/dd/yyyy");
                    txtEmail.Text = studentEditModel.Email;
                    txtMomsName.Text = studentEditModel.Momsname;
                    txtMomsPhone.Text = studentEditModel.Momsphone;
                    txtDadsName.Text = studentEditModel.Dadsname;
                    txtDadsPhone.Text = studentEditModel.Dadsphone;
                    txtAddress.Text = studentEditModel.Address;
                    txtSchool.Text = studentEditModel.School;
                    txtGrade.Text = studentEditModel.Grade;
                    txtOriginalClass.Text = studentEditModel.OriginalClass;
                    txtRelationShip.Text = studentEditModel.RelationShip;
                    cmbConsultant.Text = studentEditModel.ConsultantName;
                    cmbStatus.Text = studentEditModel.StatusText;
                    numConsRate.Value = Convert.ToDouble(studentEditModel.ConsultantRate);

                    foreach(string classpath in studentEditModel.ClassPath)
                    {
                        RadTreeViewItem targetItem = this.ClassTypeTree.GetItemByPath(classpath, "|");
                        targetItem.IsChecked = true;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定学员信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private void ClassTypeTree_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem checkedItem = e.OriginalSource as RadTreeViewItem;
            if (checkedItem != null && checkedItem.Items.Count==0)
            {
                checkedItem.IsSelected = true;
            }
        }

        private void ClassTypeTree_Unchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem checkedItem = e.OriginalSource as RadTreeViewItem;
            if (checkedItem != null)
            {
                checkedItem.IsSelected = false;
            }
        }
    }
}
