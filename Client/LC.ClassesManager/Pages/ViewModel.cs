using Aisino.MES.Client.Common;
using LC.ClassesManager.Windows;
using LC.Contracts.ClassManager;
using LC.Model.Business.ClassModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;
using WcfClientProxyGenerator.Async;

namespace LC.ClassesManager.Pages
{
    public class ViewModel
    {
        private DelegateCommand setScheduleCommand;

        public DelegateCommand SetScheduleCommand
        {
            get
            {
                return this.setScheduleCommand;
            }
            set
            {
                this.setScheduleCommand = value;
            }
        }

        private DelegateCommand cancelCommand;

        public DelegateCommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; }
        }

        private DelegateCommand checkStudentsCommand;

        public DelegateCommand CheckStudentsCommand
        {
            get { return checkStudentsCommand; }
            set { checkStudentsCommand = value; }
        }

        private DelegateCommand deleteScheduleCommand;

        public DelegateCommand DeleteScheduleCommand
        {
            get { return deleteScheduleCommand; }
            set { deleteScheduleCommand = value; }
        }


        public ViewModel()
        {
            this.SetScheduleCommand = new DelegateCommand(this.SetScheduleExecuted, CanExecuteSetSchedule);
            this.CancelCommand = new DelegateCommand(this.CancelScheduleExecuted, CanExecuteCancelSchedule);
            this.CheckStudentsCommand = new DelegateCommand(this.CheckStudentExecuted,CanExecuteCheckStudent);
            this.DeleteScheduleCommand = new DelegateCommand(this.DeleteScheduleExecuted, CanExecuteDeleteSchedule);
        }

        private void SetScheduleExecuted(object parameter)
        {
            this.SetSchedule(parameter);
        }

        private void CancelScheduleExecuted(object parameter)
        {
            this.CancelSchedule(parameter);
        }

        private void CheckStudentExecuted(object parameter)
        {
            this.CheckStudent(parameter);
        }

        private void DeleteScheduleExecuted(object parameter)
        {
            this.DeleteSchedule(parameter);
        }

        private void SetSchedule(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;
            if (appointments != null)
            {
                foreach (ScheduleAppointment appointment in appointments.OfType<ScheduleAppointment>())
                {
                    EditScheduleWindow newScheduleWindow = new EditScheduleWindow();
                    if(appointment.ScheduleId == 0)
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
                        }
                    }

                }
            }
        }

        private void CancelSchedule(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;
            if (appointments != null)
            {
                foreach (ScheduleAppointment appointment in appointments.OfType<ScheduleAppointment>())
                {
                    
                }
            }
        }

        private void CheckStudent(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;
            if (appointments != null)
            {
                foreach (ScheduleAppointment appointment in appointments.OfType<ScheduleAppointment>())
                {
                    CheckStudentWindow checkStudentWindow = new CheckStudentWindow();
                    checkStudentWindow.ClassId = appointment.ClassId;
                    checkStudentWindow.ScheduleId = appointment.ScheduleId;
                    if (checkStudentWindow.ShowDialog() == true)
                    {

                    }
                }
            }
        }

        private async void DeleteSchedule(object parameter)
        {
            if (MessageBox.Show("确认删除", "确认删除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                IEnumerable appointments = parameter as IEnumerable;
                if (appointments != null)
                {
                    foreach (ScheduleAppointment appointment in appointments.OfType<ScheduleAppointment>())
                    {
                        IAsyncProxy<IScheduleService> scheduleAsyncProxy = await Task.Run(() => ServiceHelper.GetScheduleService());
                        bool res = await scheduleAsyncProxy.CallAsync(c => c.DeleteById(appointment.ScheduleId));
                    }
                }
            }
        }

        private static bool CanExecuteSetSchedule(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;

            return appointments != null && appointments.OfType<ScheduleAppointment>().Any();
        }

        private static bool CanExecuteCancelSchedule(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;

            return appointments != null && appointments.OfType<ScheduleAppointment>().Any();
        }

        private static bool CanExecuteCheckStudent(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;

            return appointments != null && appointments.OfType<ScheduleAppointment>().Any();
        }

        private static bool CanExecuteDeleteSchedule(object parameter)
        {
            IEnumerable appointments = parameter as IEnumerable;

            return appointments != null && appointments.OfType<ScheduleAppointment>().Any();
        }

        private void CommandsInvalidateCanExecute()
        {
            this.SetScheduleCommand.InvalidateCanExecute();
            this.CheckStudentsCommand.InvalidateCanExecute();
            this.CancelCommand.InvalidateCanExecute();
            this.DeleteScheduleCommand.InvalidateCanExecute();
        }
    }
}
