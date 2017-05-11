using Aisino.MES.Client.Common;
using Aisino.MES.Client.MainForms.Pages;
using Aisino.MES.Client.Settings.Pages;
using LC.Model.Business;
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

namespace Aisino.MES.Client.MainForms
{
    /// <summary>
    /// LoginForm.xaml 的交互逻辑
    /// </summary>
    public partial class LoginForm
    {
        private List<Page> localPages;
        private LoginPage loginPage;
        private SettingPage settingPage;
        public LoginForm()
        {
            try
            {
                InitializeComponent();
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo(GlobalObjects.CurrentLanguage);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ex");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            loginFrame.Navigate(localPages[1]);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            loginFrame.Navigate(localPages[0]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                localPages = new List<Page>();
                loginPage = new LoginPage();
                settingPage = new SettingPage();
                localPages.Add(loginPage);
                localPages.Add(settingPage);
                loginFrame.Navigate(localPages[0]);

                loginPage.LoginSuccess += loginPage_LoginSuccess;

                settingPage.Restart += settingPage_Restart;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ex");
            }
        }

        void settingPage_Restart(object sender)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        void loginPage_LoginSuccess(object sender, LoginPage.LoginArgs e)
        {
            LoginModel lm = e.loginModel;
            MainWindow mwin = new MainWindow();
            Application.Current.MainWindow = mwin;
            mwin.userCode = lm.UserCode;
            this.Close();
            mwin.Show();
        }
    }
}
