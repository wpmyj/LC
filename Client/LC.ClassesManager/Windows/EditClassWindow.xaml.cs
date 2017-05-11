using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
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
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace LC.ClassesManager.Windows
{
    /// <summary>
    /// EditClassWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditClassWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private int classId;

        public int ClassId
        {
            get { return classId; }
            set { classId = value; }
        }


        private int classTypeId;

        public int ClassTypeId
        {
            get { return classTypeId; }
            set { classTypeId = value; }
        }

        private ClassTypeModel classTypeModel;

        IAsyncProxy<IClassTypeService> classTypeAsyncProxy = null;
        IAsyncProxy<IClassesService> classesAsyncProxy = null;
        ClassEditModel classEditModel = null;

        public EditClassWindow()
        {
            InitializeComponent();

            classTypeId = 0;
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            ClassEditModel classEditModel = new ClassEditModel();
            try
            {
                classTypeAsyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
                classesAsyncProxy = await Task.Run(() => ServiceHelper.GetClassService());

                IList<ClassTypeModel> classTypeLists = await classTypeAsyncProxy.CallAsync(c => c.GetAllClassType());
                this.cmbClassType.ItemsSource = classTypeLists;

                if(this.classTypeId != 0)
                {
                    classTypeModel = await classTypeAsyncProxy.CallAsync(c => c.GetClassTypeById(classTypeId));
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }

            if (Om == OperationMode.AddMode)
            {
                this.Title = "新增班级";
                txtName.IsEnabled = true;
                BindClassInfo();
            }
            else
            {
                this.Title = "修改班级";
                //txtName.IsEnabled = false;
                BindClassInfo();
            }
        }

        private async void BindClassInfo()
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (classId != 0)
                {
                    //选择了修改模式
                    classEditModel = await classesAsyncProxy.CallAsync(c => c.GetClassById(this.classId));
                    txtName.Text = classEditModel.Name;
                    cmbClassType.Text = classEditModel.TypeName;
                    dateStart.Text = classEditModel.StartDate.ToString("MM/dd/yyyy");
                    dateEnd.Text = classEditModel.EndDate.ToString("MM/dd/yyyy");
                    numLastCount.Value = classEditModel.LastCount;
                    cbIsActive.IsChecked = classEditModel.IsActive;
                }
                else
                {
                    //新增模式
                    if(cmbClassType.Items.Count > 0)
                    {
                        cmbClassType.Text = classTypeModel.Name;
                        numLastCount.Value = classTypeModel.TotalLessons;
                        cbIsActive.IsChecked = true;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定班级信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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
                    ClassEditModel newClassModel = new ClassEditModel();
                    newClassModel.Name = txtName.Text.Trim();
                    newClassModel.TypeId = (cmbClassType.SelectedItem as ClassTypeModel).Id;
                    newClassModel.LastCount = Convert.ToInt16(numLastCount.Value);
                    newClassModel.StartDate = dateStart.DisplayDate;
                    newClassModel.EndDate = dateEnd.DisplayDate;
                    newClassModel.IsActive = cbIsActive.IsChecked.Value;

                    newClassModel = await classesAsyncProxy.CallAsync(c => c.Add(newClassModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增班级成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    classEditModel.Name = txtName.Text.Trim();
                    classEditModel.TypeId = (cmbClassType.SelectedItem as ClassTypeModel).Id;
                    classEditModel.LastCount = Convert.ToInt16(numLastCount.Value);
                    classEditModel.StartDate = dateStart.DisplayDate;
                    classEditModel.EndDate = dateEnd.DisplayDate;
                    classEditModel.IsActive = cbIsActive.IsChecked.Value; 

                    classEditModel = await classesAsyncProxy.CallAsync(c => c.Update(classEditModel));

                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改班级成功！");
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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
