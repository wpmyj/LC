using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Resources;
using LC.Contracts.TeacherManager;
using LC.Model;
using LC.Model.Business.TeacherModel;
using LC.BaseManager.Reports;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;
using LC.TeacherManager.Reports;

namespace LC.BaseManager.Pages
{
    /// <summary>
    /// TeacherClassRecordDetailPage.xaml 的交互逻辑
    /// </summary>
    public partial class TeacherClassRecordDetailPage : BusinessBasePage
    {
        public TeacherClassRecordDetailPage()
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
            string strErrorMsg = string.Empty;
            try
            {
                await bindTeacherList();
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
                await DialogManager.ShowMessageAsync(this.GetMainWindow(), UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }
        }

        private async Task bindTeacherList()
        {
            IAsyncProxy<ITeacherService> teacherAsyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService()); ;
            IList<TeacherModel> RM = await teacherAsyncProxy.CallAsync(c => c.GetAllTeacher());
            this.cmbTeacher.ItemsSource = RM;
        }

        private async Task GetReportResult()
        {
            DateTime startDate = dateStart.SelectedDate.Value;
            DateTime endDate = dateEnd.SelectedDate.Value;
            int teacherId = (cmbTeacher.SelectedItem as TeacherModel).Id;
            IAsyncProxy<ITeacherService> _teacherAyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
            IList<TeacherClassRecordDetailModel> tcrdml = await _teacherAyncProxy.CallAsync(c => c.FindTeacherClassRecordDetailByDate(startDate, endDate, teacherId));
            Telerik.Reporting.ObjectDataSource objectDataSource = new Telerik.Reporting.ObjectDataSource();
            objectDataSource.DataSource = tcrdml; // GetData returns a DataTable

            //Creating a new report
            Telerik.Reporting.Report report1 = new TeacherClassRecordDetailReport();
            report1.ReportParameters["StartDate"].Value = startDate.ToString("yyyy-MM-dd");
            report1.ReportParameters["EndDate"].Value = endDate.ToString("yyyy-MM-dd");
            report1.ReportParameters["Teacher"].Value = cmbTeacher.Text;

            // Assigning the ObjectDataSource component to the DataSource property of the report.
            report1.DataSource = objectDataSource;

            // Use the InstanceReportSource to pass the report to the viewer for displaying
            Telerik.Reporting.InstanceReportSource reportSource = new Telerik.Reporting.InstanceReportSource();
            reportSource.ReportDocument = report1;

            // Assigning the report to the report viewer.
            ReportViewer1.ReportSource = reportSource;

            // Calling the RefreshReport method in case this is a WinForms application.
            ReportViewer1.RefreshReport();

        }

        private async void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            await GetReportResult();
        }
    }
}
