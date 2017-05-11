using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.ClassesManager.Pages;
using LC.Contracts.BaseManager;
using LC.Contracts.ClassManager;
using LC.Contracts.TeacherManager;
using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Model.Business.ClassModel;
using LC.Model.Business.TeacherModel;
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
    /// EditScheduleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditScheduleWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private ScheduleAppointment schedule;

        public ScheduleAppointment Schedule
        {
            get { return schedule; }
            set { schedule = value; }
        }

        public ScheduleEditModel AffterEditSchedule;

        public int classid;

        private IAsyncProxy<IScheduleService> scheduleAsyncProxy = null;

        public EditScheduleWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());

                await bindTeacherList();
                await bindClassroomList();
                await bindStatus();
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
                this.Title = "AddSchedule";
                txtName.IsEnabled = true;
                BindScheduleInfo();
            }
            else
            {
                this.Title = "EditSchedult"; 
                txtName.IsEnabled = false;
                BindScheduleInfo();
            }
        }

        private async Task bindTeacherList()
        {
            IAsyncProxy<ITeacherService> teacherAsyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService()); ;
            IList<TeacherModel> RM = await teacherAsyncProxy.CallAsync(c => c.GetAllTeacher());
            this.cmbAssistant.ItemsSource = RM;
            this.cmbTeacher.ItemsSource = RM;
        }

        private async Task bindClassroomList()
        {
            IAsyncProxy<IClassroomService> classroomAyncProxy = await Task.Run(() => ServiceHelper.GetClassroomService()); ;
            IList<ClassroomModel> RM = await classroomAyncProxy.CallAsync(c => c.GetAllClassroom());
            this.cmbClassroom.ItemsSource = RM;
        }

        private async Task bindStatus()
        {
            IAsyncProxy<IStatusService> _statusAyncProxy = await Task.Run(() => ServiceHelper.GetStatusService());
            IList<StatusModel> RM = await _statusAyncProxy.CallAsync(c => c.FindStatusByCat(GlobalObjects.ScheduleStatus));
            this.cmbStatus.ItemsSource = RM;
        }

        private async void BindScheduleInfo()
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (Om == OperationMode.EditMode)
                {
                    //选择了修改模式
                    this.AffterEditSchedule = await scheduleAsyncProxy.CallAsync(c => c.GetScheduleById(schedule.ScheduleId));
                    txtName.Text = AffterEditSchedule.ClassName;
                    classid = AffterEditSchedule.ClassId;
                    realDate.Text = AffterEditSchedule.StartTime.Date.ToShortDateString();
                    timeStart.SelectedTime = AffterEditSchedule.StartTime.TimeOfDay;
                    timeEnd.SelectedTime = AffterEditSchedule.EndTime.TimeOfDay;
                    cmbClassroom.Text = AffterEditSchedule.ClassroomName;
                    cmbAssistant.Text = AffterEditSchedule.AssistantName;
                    cmbTeacher.Text = AffterEditSchedule.TeacherName;
                    cmbStatus.Text = AffterEditSchedule.StatusDes;
                    txtNote.Text = AffterEditSchedule.Note;
                    txtSchemas.Text = AffterEditSchedule.LessonName;
                    this.Title = this.Title + AffterEditSchedule.ScheduleId;
                }
                else if(Om==OperationMode.CopyMode)
                {
                    //复制模式
                    this.AffterEditSchedule = await scheduleAsyncProxy.CallAsync(c => c.GetScheduleById(schedule.ScheduleId));
                    txtName.Text = AffterEditSchedule.ClassName;
                    classid = AffterEditSchedule.ClassId;
                    realDate.Text = AffterEditSchedule.StartTime.Date.ToShortDateString();
                    timeStart.SelectedTime = AffterEditSchedule.StartTime.TimeOfDay;
                    timeEnd.SelectedTime = AffterEditSchedule.EndTime.TimeOfDay;
                    cmbClassroom.Text = AffterEditSchedule.ClassroomName;
                    cmbAssistant.Text = AffterEditSchedule.AssistantName;
                    cmbTeacher.Text = AffterEditSchedule.TeacherName;
                    cmbStatus.Text = AffterEditSchedule.StatusDes;
                    txtNote.Text = AffterEditSchedule.Note;
                    txtSchemas.Text = AffterEditSchedule.LessonName;
                    this.Title = this.Title + "Copy";
                }
                else
                {
                    //新增模式
                    txtName.Text = schedule.Subject;
                    classid = schedule.ClassId;
                    realDate.Text = schedule.Start.Date.ToShortDateString();
                    timeStart.SelectedTime = schedule.Start.TimeOfDay;
                    timeEnd.SelectedTime = schedule.End.TimeOfDay;
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
            if (Om == OperationMode.AddMode || Om == OperationMode.CopyMode)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    ScheduleEditModel newScheduleModel = new ScheduleEditModel();
                    newScheduleModel.ClassId = schedule.ClassId ;
                    newScheduleModel.ClassroomId = (cmbClassroom.SelectedItem as ClassroomModel).Id;
                    newScheduleModel.EndTime = realDate.SelectedDate.Value.Date + timeEnd.SelectedValue.Value.TimeOfDay;
                    newScheduleModel.StartTime = realDate.SelectedDate.Value.Date + timeStart.SelectedValue.Value.TimeOfDay;
                    newScheduleModel.RealDate = realDate.SelectedDate.Value.Date;
                    newScheduleModel.TeacherId = (cmbTeacher.SelectedItem as TeacherModel).Id;
                    newScheduleModel.Status = (cmbStatus.SelectedItem as StatusModel).Id;
                    newScheduleModel.Note = txtNote.Text.Trim();
                    newScheduleModel.LessonName = txtSchemas.Text.Trim();
                    if(cmbAssistant.SelectedIndex != -1)
                    {
                        newScheduleModel.AssistantId = (cmbAssistant.SelectedItem as TeacherModel).Id;
                    }

                    newScheduleModel = await scheduleAsyncProxy.CallAsync(c => c.Add(newScheduleModel));
                    this.AffterEditSchedule = newScheduleModel;
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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增（复制）排课信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            //#region 复制
            //else if(Om == OperationMode.CopyMode)
            //{
            //    string strErrorMsg = string.Empty;
            //    try
            //    {
            //        ScheduleEditModel newScheduleModel = new ScheduleEditModel();
            //        newScheduleModel.ClassId = schedule.ClassId;
            //        newScheduleModel.ClassroomId = (cmbClassroom.SelectedItem as ClassroomModel).Id;
            //        newScheduleModel.EndTime = realDate.SelectedDate.Value.Date + timeEnd.SelectedValue.Value.TimeOfDay;
            //        newScheduleModel.StartTime = realDate.SelectedDate.Value.Date + timeStart.SelectedValue.Value.TimeOfDay;
            //        newScheduleModel.RealDate = realDate.SelectedDate.Value.Date;
            //        newScheduleModel.TeacherId = (cmbTeacher.SelectedItem as TeacherModel).Id;
            //        newScheduleModel.Status = (cmbStatus.SelectedItem as StatusModel).Id;
            //        if (cmbAssistant.SelectedIndex != -1)
            //        {
            //            newScheduleModel.AssistantId = (cmbAssistant.SelectedItem as TeacherModel).Id;
            //        }

            //        newScheduleModel = await scheduleAsyncProxy.CallAsync(c => c.Add(newScheduleModel));
            //        this.AffterEditSchedule = newScheduleModel;
            //        this.DialogResult = true;
            //    }
            //    catch (TimeoutException timeProblem)
            //    {
            //        strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            //    }
            //    catch (FaultException<LCFault> af)
            //    {
            //        strErrorMsg = af.Detail.Message;
            //    }
            //    catch (FaultException unknownFault)
            //    {
            //        strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
            //    }
            //    catch (CommunicationException commProblem)
            //    {
            //        strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            //    }
            //    if (strErrorMsg != string.Empty)
            //    {
            //        await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "复制排课信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            //    }
            //}
            //#endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    AffterEditSchedule.ClassroomId = (cmbClassroom.SelectedItem as ClassroomModel).Id;
                    AffterEditSchedule.ClassroomName = cmbClassroom.Text;
                    AffterEditSchedule.EndTime = realDate.SelectedDate.Value.Date + timeEnd.SelectedValue.Value.TimeOfDay;
                    AffterEditSchedule.StartTime = realDate.SelectedDate.Value.Date + timeStart.SelectedValue.Value.TimeOfDay;
                    AffterEditSchedule.RealDate = realDate.SelectedDate.Value.Date;
                    AffterEditSchedule.TeacherId = (cmbTeacher.SelectedItem as TeacherModel).Id;
                    AffterEditSchedule.TeacherName = cmbTeacher.Text;
                    AffterEditSchedule.Status = (cmbStatus.SelectedItem as StatusModel).Id;
                    AffterEditSchedule.StatusDes = cmbStatus.Text;
                    AffterEditSchedule.Note = txtNote.Text.Trim();
                    AffterEditSchedule.LessonName = txtSchemas.Text.Trim();
                    if (cmbAssistant.SelectedIndex != -1)
                    {
                        AffterEditSchedule.AssistantId = (cmbAssistant.SelectedItem as TeacherModel).Id;
                        AffterEditSchedule.AssistantName = cmbAssistant.Text;
                    }

                    await scheduleAsyncProxy.CallAsync(c => c.Update(AffterEditSchedule));

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
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改排课信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
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
