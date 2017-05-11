using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.TeacherManager;
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
    public partial class CheckMoneyWindow : BaseWindow
    {
        private int teacherId;

        public int TeacherId
        {
            get { return teacherId; }
            set { teacherId = value; }
        }

        private string teacherName;

        public string TeacherName
        {
          get { return teacherName; }
          set { teacherName = value; }
        }

        public CheckMoneyWindow()
        {
            InitializeComponent();
        }
        private async void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.GetMoney();
        }

        private async Task GetMoney()
        {
            IAsyncProxy<ITeacherService> teacherServiceProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
            IList<TeacherCheckModel> tcmList = await teacherServiceProxy.CallAsync(t => t.GetCheckMonthMoney(this.teacherId));
            this.gvMoney.ItemsSource = tcmList;
        }
        private async void CheckControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TeacherCheckModel selectCheckMoney = gvMoney.SelectedItem as TeacherCheckModel;

            IAsyncProxy<ITeacherService> teacherServiceProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
            bool res = await teacherServiceProxy.CallAsync(t => t.CheckMonthMoney(this.teacherId, selectCheckMoney.month, selectCheckMoney.money, GlobalObjects.currentLoginUser));
            if (res == true)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "教师"+this.TeacherName+"的"+selectCheckMoney.month+"月费用结算成功！", MessageDialogStyle.Affirmative, null);
            }
        }
    }
}
