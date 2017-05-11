using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using Aisino.MES.Resources;
using LC.BaseManager.Reports;
using LC.Contracts.BaseManager;
using LC.Contracts.TeacherManager;
using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Model.Business.TeacherModel;
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

namespace LC.BaseManager.Pages
{
    /// <summary>
    /// TeacherClassRecordDetailPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConsultantRecordDetailPage : BusinessBasePage
    {
        public ConsultantRecordDetailPage()
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
                await bindConsultantList();
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

        private async Task bindConsultantList()
        {
            IAsyncProxy<IConsultantService> ConsultantAsyncProxy = await Task.Run(() => ServiceHelper.GetConsultantService()); ;
            IList<ConsultantModel> RM = await ConsultantAsyncProxy.CallAsync(c => c.GetAllConsultant());
            this.cmbConsultant.ItemsSource = RM;
        }

        private async Task GetReportResult()
        {
            DateTime startDate = dateStart.SelectedDate.Value;
            DateTime endDate = dateEnd.SelectedDate.Value;
            int consultantId = (cmbConsultant.SelectedItem as ConsultantModel).Id;
            IAsyncProxy<IConsultantService> _consultantAyncProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
            IList<ConsultantRecordDetailModel> crdml = await _consultantAyncProxy.CallAsync(c => c.FindConsultantRecordDetailByDate(startDate, endDate, consultantId));
            Telerik.Reporting.ObjectDataSource objectDataSource = new Telerik.Reporting.ObjectDataSource();
            objectDataSource.DataSource = crdml; // GetData returns a DataTable

            //Creating a new report
            Telerik.Reporting.Report report1 = new ConsultantRecordDetailReport();
            report1.ReportParameters["StartDate"].Value = startDate.ToString("yyyy-MM-dd");
            report1.ReportParameters["EndDate"].Value = endDate.ToString("yyyy-MM-dd");
            report1.ReportParameters["Consultant"].Value = cmbConsultant.Text;

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
