using Aisino.MES.Client.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aisino.MES.Client.Settings.Pages.SettingPages
{
    /// <summary>
    /// ReportServerSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReportServerSettingPage : Page
    {
        public ReportServerSettingPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //加载报表服务设置
            ReportPathText.Text = ClientHelper.ReportServerCof.ReportPath;
        }

        private void BtnSaveReportServerConfig_Click(object sender, RoutedEventArgs e)
        {
            ClientHelper.ReportServerCof.ReportPath = ReportPathText.Text.Trim();
            ClientHelper.SetReportServerConfig();
        }
    }
}
