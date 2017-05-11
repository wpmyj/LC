using Aisino.MES.Client.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Aisino.MES.Client.MainForms
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ClientHelper.GetServiceHostInfo();
            ClientHelper.GetReportServerConfig();
            ClientHelper.GetClientLan();

            //58.215.196.58:8088
        }
    }
}
