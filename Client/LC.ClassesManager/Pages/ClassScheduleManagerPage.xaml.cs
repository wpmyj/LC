using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using LC.ClassesManager.Windows;
using LC.Contracts.ClassManager;
using LC.Contracts.TeacherManager;
using LC.Model.Business.ClassModel;
using LC.Model.Business.TeacherModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;
using Telerik.Windows.DragDrop.Behaviors;
using WcfClientProxyGenerator.Async;

namespace LC.ClassesManager.Pages
{
    /// <summary>
    /// ClassScheduleManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClassScheduleManagerPage : BusinessBasePage
    {
        private DateTime currentDate;
        public ClassScheduleManagerPage()
        {
            InitializeComponent();

        }
        
        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth > 100)
            {
                dockPanel.Width = this.ActualWidth;
                dockPanel.Height = this.ActualHeight;
            }
        }

        private async void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            currentDate = new DateTime(1900,1,1);
            await bindClassTypeList();
            await bindSchedule();
        }

        private async Task bindClassTypeList()
        {
            IAsyncProxy<IClassesService> _classAyncProxy = await Task.Run(() => ServiceHelper.GetClassService());
            IList<ClassDisplayModel> RM = await _classAyncProxy.CallAsync(c => c.GetAllActiveClasses());
            this.gvClasses.ItemsSource = RM.ToList();
        }

        private async Task bindSchedule()
        {
            IAsyncProxy<ITeacherService> teacherAsyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
            TeacherModel tm = await teacherAsyncProxy.CallAsync(t => t.FindTeacherByUserCode(GlobalObjects.currentLoginUser.UserCode));
            
            this.currentDate = classSchedule.CurrentDate;
            IAsyncProxy<IScheduleService> scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());
            IList<ScheduleDisplayModel> RM = null;

            if(tm.Id == 0)
            {
                RM = await scheduleAsyncProxy.CallAsync(c => c.FindScheduleByMonth(classSchedule.CurrentDate.ToString("yyyyMM")));
            }
            else
            {
                RM = await scheduleAsyncProxy.CallAsync(c => c.FindScheduleByMonthAndTeacher(classSchedule.CurrentDate.ToString("yyyyMM"),tm.Id)); 
            }

            if (RM != null)
            {
                ObservableCollection<ScheduleAppointment> appointments = new ObservableCollection<ScheduleAppointment>();
                foreach (ScheduleDisplayModel sdm in RM)
                {
                    ScheduleAppointment sa = new ScheduleAppointment();
                    sa.End = sdm.EndTime;
                    sa.ScheduleId = sdm.Id;
                    sa.Start = sdm.StartTime;
                    sa.TeacherName = sdm.TeacherName;
                    sa.AssistantName = sdm.AssistantName;
                    sa.ClassroomName = sdm.ClassroomName;
                    sa.ClassId = sdm.ClassId;
                    sa.Subject = sdm.ClassName + "\r\n" + sdm.SchemasText;
                    
                    if (sdm.Status == "Normal")
                    {
                        //正常上课状态
                        if (sdm.ClassName.Contains(sdm.TeacherName))
                        {
                            //教师名字与原班级中所含教师名字一致，则显示任为绿色
                            sa.Category = new Category("Green Category", new SolidColorBrush(Colors.Green));
                        }
                        else
                        {
                            //教师名字与原班级名称中所含教师名字不一致
                            //说明代替上课，则显示蓝色
                            sa.Category = new Category("Blue Category", new SolidColorBrush(Colors.Blue));
                        }
                    }
                    else
                    {
                        sa.Category = new Category("Red Category", new SolidColorBrush(Colors.Red));
                    }
                    appointments.Add(sa);
                }

                this.classSchedule.AppointmentsSource = appointments;
            }
        }

        private void classSchedule_ShowDialog(object sender, ShowDialogEventArgs e)
        {
            if (e.DialogViewModel is AppointmentDialogViewModel)
                e.Cancel = true;
        }

        private async void btnSetSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (classSchedule.SelectedAppointment != null)
            {
                ScheduleAppointment appointment = classSchedule.SelectedAppointment as ScheduleAppointment;
                EditScheduleWindow newScheduleWindow = new EditScheduleWindow();
                if (appointment.ScheduleId == 0)
                {
                    newScheduleWindow.Om = OperationMode.AddMode;
                    newScheduleWindow.Schedule = appointment;

                    if (newScheduleWindow.ShowDialog() == true)
                    {
                        ScheduleEditModel affterEdit = newScheduleWindow.AffterEditSchedule;
                        RadScheduleViewCommands.BeginEditAppointment.Execute(appointment, null);
                        appointment.ScheduleId = affterEdit.ScheduleId;
                        appointment.Start = affterEdit.StartTime;
                        appointment.End = affterEdit.EndTime;
                        if (affterEdit.StatusDes == "Normal")
                        {
                            appointment.Category = new Category("Green Category", new SolidColorBrush(Colors.Green));
                        }
                        else
                        {
                            appointment.Category = new Category("Red Category", new SolidColorBrush(Colors.Red));
                        }

                        RadScheduleViewCommands.CommitEditAppointment.Execute(appointment, null);

                        await bindSchedule();
                    }
                }
                else
                {
                    newScheduleWindow.Om = OperationMode.EditMode;
                    newScheduleWindow.Schedule = appointment;

                    if (newScheduleWindow.ShowDialog() == true)
                    {
                        ScheduleEditModel affterEdit = newScheduleWindow.AffterEditSchedule;
                        RadScheduleViewCommands.BeginEditAppointment.Execute(appointment, null);

                        appointment.End = affterEdit.EndTime;
                        appointment.ScheduleId = affterEdit.ScheduleId;
                        appointment.Start = affterEdit.StartTime;
                        appointment.Subject = affterEdit.ClassName;
                        appointment.TeacherName = affterEdit.TeacherName;
                        appointment.AssistantName = affterEdit.AssistantName;
                        appointment.ClassroomName = affterEdit.ClassroomName;
                        if (affterEdit.StatusDes == "Normal")
                        {
                            appointment.Category = new Category("Green Category", new SolidColorBrush(Colors.Green));
                        }
                        else
                        {
                            appointment.Category = new Category("Red Category", new SolidColorBrush(Colors.Red));
                        }

                        RadScheduleViewCommands.CommitEditAppointment.Execute(appointment, null);

                        await bindSchedule();
                    }
                }
            }
        }

        private void btnCheckStudent_Click(object sender, RoutedEventArgs e)
        {
            if (classSchedule.SelectedAppointment != null)
            {
                ScheduleAppointment appointment = classSchedule.SelectedAppointment as ScheduleAppointment;
                CheckStudentWindow checkStudentWindow = new CheckStudentWindow();
                checkStudentWindow.ClassId = appointment.ClassId;
                checkStudentWindow.ScheduleId = appointment.ScheduleId;
                checkStudentWindow.Appointment = appointment;
                if (checkStudentWindow.ShowDialog() == true)
                {

                }
            }
        }

        private async void btnDeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认删除", "确认删除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (classSchedule.SelectedAppointment != null)
                {
                    ScheduleAppointment appointment = classSchedule.SelectedAppointment as ScheduleAppointment;
                    IAsyncProxy<IScheduleService> scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());
                    bool res = await scheduleAsyncProxy.CallAsync(c => c.DeleteById(appointment.ScheduleId));
                    await bindSchedule();
                }
            }
        }

        private async void classSchedule_VisibleRangeChanged(object sender, EventArgs e)
        {
            await bindSchedule();
        }

        private async void btnCopySchedule_Click(object sender, RoutedEventArgs e)
        {
            if (classSchedule.SelectedAppointment != null)
            {
                ScheduleAppointment appointment = classSchedule.SelectedAppointment as ScheduleAppointment;
                EditScheduleWindow newScheduleWindow = new EditScheduleWindow();
                newScheduleWindow.Om = OperationMode.CopyMode;
                newScheduleWindow.Schedule = appointment;

                if (newScheduleWindow.ShowDialog() == true)
                {
                    ScheduleAppointment sa = new ScheduleAppointment();
                    ScheduleEditModel affterEdit = newScheduleWindow.AffterEditSchedule;
                    RadScheduleViewCommands.BeginEditAppointment.Execute(sa, null);
                    sa.ScheduleId = affterEdit.ScheduleId;
                    sa.Start = affterEdit.StartTime;
                    sa.End = affterEdit.EndTime;
                    if (affterEdit.StatusDes == "Normal")
                    {
                        sa.Category = new Category("Green Category", new SolidColorBrush(Colors.Green));
                    }
                    else
                    {
                        sa.Category = new Category("Red Category", new SolidColorBrush(Colors.Red));
                    }

                    RadScheduleViewCommands.CommitEditAppointment.Execute(sa, null);
                }

                await bindSchedule();

            }
        }
    }
}
