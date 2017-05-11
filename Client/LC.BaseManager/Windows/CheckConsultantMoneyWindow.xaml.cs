using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.BaseManager;
using LC.Contracts.TeacherManager;
using LC.Model.Business.BaseModel;
using LC.Model.Business.TeacherModel;
using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace LC.BaseManager.Windows
{
    /// <summary>
    /// CheckMoneyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckConsultantMoneyWindow : BaseWindow
    {
        private int consultantId;

        public int ConsultantId
        {
            get { return consultantId; }
            set { consultantId = value; }
        }

        private string consultantName;

        public string ConsultantName
        {
            get { return consultantName; }
            set { consultantName = value; }
        }

        public CheckConsultantMoneyWindow()
        {
            InitializeComponent();
        }

        private async void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.GetMoney();
        }

        private async Task GetMoney()
        {
            IAsyncProxy<IConsultantService> consultantServiceProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
            IList<ConsultantCheckModel> tcmList = await consultantServiceProxy.CallAsync(t => t.GetCheckMonthMoney(this.consultantId));
            this.gvMoney.ItemsSource = tcmList;
        }
        private async void CheckControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConsultantCheckModel selectCheckMoney = gvMoney.SelectedItem as ConsultantCheckModel;

            IAsyncProxy<IConsultantService> consultantServiceProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
            bool res = await consultantServiceProxy.CallAsync(t => t.CheckMonthMoney(this.consultantId, selectCheckMoney.month, selectCheckMoney.money, GlobalObjects.currentLoginUser));
            if (res == true)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "会籍顾问" + this.ConsultantName + "的" + selectCheckMoney.month + "月费用结算成功！", MessageDialogStyle.Affirmative, null);
            }
            await this.GetMoney();
        }
    }
}
