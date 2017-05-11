using Aisino.MES.Client.Common;
using Aisino.MES.Client.Settings.Pages.SettingPages;
using LC.Model;
using MahApps.Metro.Controls;
using LC.Service.Contracts.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

namespace Aisino.MES.Client.Settings.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        BackgroundWorker bwTestConnect = new BackgroundWorker();
        HardwareConfigEnabled hardwareConfigEnabled = new HardwareConfigEnabled();
        StationConfigEnabled stationConfigEnabled = new StationConfigEnabled();

        private delegate void outputDelegate(string msg);
        private void output(string msg)
        {
            this.LTestConnect.Dispatcher.Invoke(new outputDelegate(outputAction), msg);
        }

        private void outputAction(string msg)
        {
            this.LTestConnect.Content = msg;
        }

        public SettingPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            #region 设置当前服务链接
            ServiceHostAddressText.Text = GlobalObjects.ServiceHostAddress;
            cmbCulture.Text = GlobalObjects.CurrentLanguage;
            #endregion

            #region 后台线程
            bwTestConnect.DoWork += new DoWorkEventHandler(bwTestConnect_DoWork);
            bwTestConnect.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwTestConnect_RunWorkerCompleted);
            if (!bwTestConnect.IsBusy)
            {
                bwTestConnect.RunWorkerAsync();
            }
            #endregion
        }

        #region 服务链接设置
        //获取并设置当前服务链接
        private void BtnSaveHostAddress_Click(object sender, RoutedEventArgs e)
        {
            AppConfigs ac = new AppConfigs();
            ac.updateAppSettings("ServiceHostAddress", ServiceHostAddressText.Text.Trim());
            GlobalObjects.ServiceHostAddress = ServiceHostAddressText.Text.Trim();
        }
        #endregion

        #region 数据库连接测试
        void bwTestConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!bwTestConnect.IsBusy)
            {
                bwTestConnect.RunWorkerAsync();
            }
        }

        async void bwTestConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(500);
            //绑定委托要执行的方法   
            try
            {
                IAsyncProxy<ICommonService> asyncProxy = await Task.Run(() => ServiceHelper.GetCommonService());
                DateTime datetime = asyncProxy.Client.GetSystemDateTime();
                output("服务链接成功，当前服务器时间："+datetime.ToString("yyyy/MM/dd HH:mm:ss"));
                GlobalObjects.IsConnected = true;
                //if (GlobalObjects.HardwareConfigXml == null || GlobalObjects.HardwareConfigXml == string.Empty || GlobalObjects.HardwareConfigXml == "")
                //{
                //    //如果还没有获取过硬件配置文件，则需要从数据库中根据mac地址获取，如果依然没有，则从标准配置文件获取
                //    GlobalObjects.HardwareConfigXml = DeviceHelper.LoadXMLFile();
                //}
            }
            catch (TimeoutException timeProblem)
            {
                GlobalObjects.IsConnected = false;
                output("服务链接超时");
            }
            catch (FaultException<LCFault> af)
            {
                GlobalObjects.IsConnected = false;
                output("服务链接失败");
            }
            catch (FaultException unknownFault)
            {
                GlobalObjects.IsConnected = false;
                output("服务链接失败");
            }
            catch (CommunicationException commProblem)
            {
                GlobalObjects.IsConnected = false;
                output("服务链接失败");
            }

            stationConfigEnabled.stationConfigEnabled = GlobalObjects.IsConnected;
        }
        #endregion

        #region 设置客户端语言
        public delegate void RestartHandle(Object sender);
        public event RestartHandle Restart;
        private void BtnSaveCulture_Click(object sender, RoutedEventArgs e)
        {
            AppConfigs ac = new AppConfigs();
            ac.updateAppSettings("Lang", cmbCulture.Text);
            GlobalObjects.CurrentLanguage = cmbCulture.Text;

            Restart(this);
        }
        #endregion
    }

    #region 绑定属性
    public class HardwareConfigEnabled : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _hardwareConfigEnabled;
        public bool hardwareConfigEnabled
        {
            get
            {
                return _hardwareConfigEnabled;
            }
            set
            {
                if (value != _hardwareConfigEnabled)
                {
                    _hardwareConfigEnabled = value;
                    //改变时通知
                    prochanged("hardwareConfigEnabled");
                }
            }
        }

        private void prochanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class StationConfigEnabled : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _stationConfigEnabled;
        public bool stationConfigEnabled
        {
            get
            {
                return _stationConfigEnabled;
            }
            set
            {
                if (value != _stationConfigEnabled)
                {
                    _stationConfigEnabled = value;
                    //改变时通知
                    prochanged("stationConfigEnabled");
                }
            }
        }

        private void prochanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
    #endregion
}
