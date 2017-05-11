using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.ClassesManager.Pages;
using LC.Contracts.ClassManager;
using LC.Contracts.StudentManager;
using LC.Model;
using LC.Model.Business.ClassModel;
using LC.Model.Business.StudentModel;
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

namespace LC.ClassesManager.Windows
{
    /// <summary>
    /// CheckStudentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckStudentWindow : BaseWindow
    {
        private int classId;

        public int ClassId
        {
            get { return classId; }
            set { classId = value; }
        }

        private int scheduleId;

        public int ScheduleId
        {
            get { return scheduleId; }
            set { scheduleId = value; }
        }

        private ScheduleAppointment appointment;

        public ScheduleAppointment Appointment
        {
            get { return appointment; }
            set { appointment = value; }
        }

        private List<RadTreeViewItem> checkeditems;

        public CheckStudentWindow()
        {
            InitializeComponent();
        }
        private async void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(GlobalObjects.currentLoginUser.LoginName == "admin")
            {
                btnUnCheck.Visibility = Visibility.Visible;
            }
            else
            {
                btnUnCheck.Visibility = Visibility.Hidden;
            }
            string strErrorMsg = string.Empty;
            try
            {
                checkeditems = new List<RadTreeViewItem>();
                await bindStudentList();                
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
        }

        private async Task bindStudentList()
        {
            IEqualityComparer<StudentDisplayModel> ec = new EntityComparer(); 
            IAsyncProxy<IScheduleService> scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());
            IAsyncProxy<IStudentService> studentAyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());
            ScheduleEditModel sem = await scheduleAsyncProxy.CallAsync(c => c.GetScheduleById(ScheduleId));

            lbClassName.Content = "ClassName:"+sem.ClassName+"  LessonName:"+sem.LessonName;
            lbClassTime.Content = "StartTime:" + sem.StartTime.ToString("yyyy-MM-dd HH:mm");
            lbTeacherName.Content = "Teacher:" + sem.TeacherName;

            IList<StudentDisplayModel> RM;

            if (sem.RealDate < DateTime.Now)
            {
                //如果选择的为历史，先获取列表
                RM = studentAyncProxy.Client.FindStudentsByScheduleId(ScheduleId);                
                if(RM == null || RM.Count == 0)
                {
                    //说明该课程还没有历史记录，说明没有签到过，则需要根据班级查找
                    RM = studentAyncProxy.Client.FindStudentsByClassId(ClassId);
                }
                else
                {
                    //考虑可能后期维护的时候增加漏掉的新增学员，需要合并列表
                    RM = RM.Union(studentAyncProxy.Client.FindStudentsByClassId(ClassId),ec).ToList();                    
                }
            }
            else
            {
                //绑定该课程所关联班级所有学生列表
                RM = studentAyncProxy.Client.FindStudentsByClassId(ClassId);
            }
            this.StudentTree.ItemsSource = RM;
            this.StudentTree.ExpandAll();
            //选择当前已经点到学生

            if (sem.AttendedStudentIds != null && sem.AttendedStudentIds.Count > 0)
            {
                foreach (StudentDisplayModel sdm in StudentTree.Items)
                {
                    RadTreeViewItem targetItem = StudentTree.GetItemByPath(sdm.Name);
                    if (sem.AttendedStudentIds.Contains(sdm.Id))
                    {
                        targetItem.IsChecked = true;
                    }
                }
            }

            foreach(StudentDisplayModel sdm in StudentTree.Items)
            {
                RadTreeViewItem targetItem = StudentTree.GetItemByPath(sdm.Name);
                targetItem.ToolTip = sdm.Name + "\r\n" + sdm.Nickname + "\r\n" + sdm.Momsname + "\r\n" +
                    sdm.Momsphone + "\r\n" + sdm.School + "\r\n" + sdm.Grade + "\r\n" + sdm.ExtraInfo;
            }

        }

        private void StudentTree_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem checkedItem = e.OriginalSource as RadTreeViewItem;
            if (checkedItem != null && checkedItem.Items.Count == 0)
            {
                checkedItem.IsSelected = true;
                checkeditems.Add(checkedItem);
            }
        }

        private void StudentTree_Unchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem checkedItem = e.OriginalSource as RadTreeViewItem;
            if (checkedItem != null)
            {
                checkedItem.IsSelected = false;
                checkeditems.Remove(checkedItem);
            }
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = "";
            try
            {
                List<int> checkStudentList = new List<int>();
                //foreach (StudentDisplayModel sdm in StudentTree.SelectedItems)
                //{
                //    checkStudentList.Add(sdm.Id);
                //}  
                foreach(RadTreeViewItem rtvi in checkeditems)
                {
                    checkStudentList.Add((rtvi.Item as StudentDisplayModel).Id);
                }


                IAsyncProxy<IScheduleService> scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());
                await scheduleAsyncProxy.CallAsync(c => c.CheckStudent(ScheduleId,checkStudentList,GlobalObjects.currentLoginUser));

                this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "学员上课登记成功！");
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "学员上课登记失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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

        private async void btnUnCheck_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = "";
            try
            {
                StudentDisplayModel sdm = StudentTree.SelectedItem as StudentDisplayModel;
                IAsyncProxy<IScheduleService> scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());
                await scheduleAsyncProxy.CallAsync(c => c.UnCheckStudent(ScheduleId, sdm.Id, GlobalObjects.currentLoginUser));

                this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "撤销学员上课记录成功！");
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "撤销学员上课记录失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

    }
}
