using Aisino.MES.Client.Common;
using Aisino.MES.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WcfClientProxyGenerator;
using WcfClientProxyGenerator.Async;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.ComponentModel;
using LC.Model.Business;
using LC.Service.Contracts.Common;
using LC.Model;

namespace Aisino.MES.Client.MainForms.Pages
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page
    {
        #region 登录成功委托
        /************************
         * 登录成功后，登录主界面可以接受成功信息
         * 主界面可以根据接收到的信息按需处理
         ************************/
        private delegate void LoginDelegate(LoginModel loginModel);
        public class LoginArgs : EventArgs
        {
            private LoginModel _loginModel;
            public LoginArgs(LoginModel loginModel)
            {
                this._loginModel = loginModel;
            }
            public LoginModel loginModel
            {
                get { return _loginModel; }
            }
        }
        public delegate void LoginHandle(Object sender, LoginArgs e);
        public event LoginHandle LoginSuccess;

        #endregion

        public LoginPage()
        {
            InitializeComponent();
#if DEBUG
            cmb_loginName.Text = "admin";
            pwdPassword.Password = "lcmanager";
#endif
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            await Login();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            AppConfigs ac = new AppConfigs();
            List<string> user_list = ac.getAppSettingsByKey("user");
            foreach (string name in user_list)
            {
                cmb_loginName.Items.Add(name);
            }
            if (cmb_loginName.Items.Count > 0)
            {
                cmb_loginName.SelectedIndex = 0;
                List<string> pass_list = ac.getAppSettingsByKey(cmb_loginName.Text);
                if (pass_list.Count > 0)
                {
                    //有密码信息，增解密并设置
                    System.Text.Encoding encoder = System.Text.Encoding.UTF8;
                    try
                    {
                        pwdPassword.Password = encoder.GetString(XXTEA.Decrypt(System.Convert.FromBase64String(pass_list[0]), encoder.GetBytes("1234567890abcdef")));
                        chb_remberPassword.IsChecked = true;
                    }
                    catch (Exception)
                    {
                        //AisinoMessageBox.Show("记录密码读取错误", "提示", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        //AisinoMessageBox.Show("记录密码读取错误", UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        strErrorMsg = "记录密码读取错误";
                        
                    }
                    if (strErrorMsg != string.Empty)
                    {
                        await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, UIResources.MsgInfo, strErrorMsg, MessageDialogStyle.Affirmative, null);
                    }
                }
            }
            cmb_loginName.Focus();
        }

        private async void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await Login();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Aisino.MES.Client.WPFCommon.WPFMessageBox.AisinoMessageBox.Show("Test Yes / No / Cancel", "UIShell - UI Framework - MessageBox v1.0.0.6 Test!", MessageBoxButton.YesNoCancel);
            //MIV.Bus.WPF.UIShell.Controls.MessageBox.Show("Test Yes / No / Cancel", "UIShell - UI Framework - MessageBox v1.0.0.6 Test!", MessageBoxButton.YesNoCancel);
        }

        private async Task Login()
        {
            string strMsg = string.Empty;
            try
            {
                //lblProgress.Content = "与通讯握手...";
                //new Thread(() =>
                //{
                //    this.Dispatcher.Invoke(new Action(() => { lblProgress.Content = "与通讯握手..."; }));
                //}
                //    ).Start();
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(new Action(() => { lblProgress.Content = "与通讯握手..."; loginProgress.IsActive = true; }));
                }
                    ).Start();
                //loginProgress.IsActive = true;
                IAsyncProxy<ICommonService> asyncProxy = await Task.Run(()=>ServiceHelper.GetCommonService());
                //lblProgress.Content = "正在验证身份...";
                new Thread(() =>
                {
                    this.Dispatcher.Invoke(new Action(() => { lblProgress.Content = "正在验证身份..."; }));
                }
                    ).Start();
                LoginModel lm = await asyncProxy.CallAsync(c => c.Login(cmb_loginName.Text.Trim(), pwdPassword.Password));

                if (lm != null && lm.UserCode != null)
                {
                    //保存当前登录对象到全局变量
                    GlobalObjects.currentLoginUser = lm;
                    //保存密码信息
                    SaveLogonName(lm.LoginName);
                    LoginSuccess(this, new LoginArgs(lm));
                }
            }
            catch (TimeoutException timeProblem)
            {
                strMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            }
            catch (FaultException<LCFault> af)
            {
                strMsg = af.Detail.Message;
            }
            catch (FaultException unknownFault)
            {
                strMsg = UIResources.UnKnowFault + unknownFault.Message;
            }
            catch (CommunicationException commProblem)
            {
                strMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            }

            if (strMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, UIResources.MsgInfo, strMsg, MessageDialogStyle.Affirmative, null);
                loginProgress.IsActive = false;
                lblProgress.Content = "";
            }
        }

        private void SaveLogonName(string loginName)
        {
            AppConfigs ac = new AppConfigs();
            //如果在appconfig中还不存在该用户，则保存该用户登陆名
            if (!ac.valueExistInAppSettings(loginName))
            {
                ac.addAppSettings("user", loginName);
                //如果选择了记住密码，则保存对应密码
                if (chb_remberPassword.IsChecked.Value)
                {
                    System.Text.Encoding encoder = System.Text.Encoding.UTF8;
                    Byte[] data = XXTEA.Encrypt(encoder.GetBytes(pwdPassword.Password), encoder.GetBytes("1234567890abcdef"));
                    string password = System.Convert.ToBase64String(data);
                    ac.addAppSettings(loginName, password);
                }
            }
            //如果该用户已经存在，则判断是否选择保存密码
            else
            {
                if (chb_remberPassword.IsChecked.Value)
                {
                    //如果保存密码，则察看该用户密码是否已经存在,如果不存在则保存密码
                    System.Text.Encoding encoder = System.Text.Encoding.UTF8;
                    Byte[] data = XXTEA.Encrypt(encoder.GetBytes(pwdPassword.Password), encoder.GetBytes("1234567890abcdef"));
                    string password = System.Convert.ToBase64String(data);
                    if (!ac.keyExistInAppSettings(loginName))
                    {
                        ac.addAppSettings(loginName, password);
                    }
                    else
                    {
                        ac.updateAppSettings(loginName, password);
                    }
                }
                else
                {
                    //如果不保存密码，则察看该用户密码是否已经存在，如果存在则删除该密码
                    if (ac.keyExistInAppSettings(loginName))
                    {
                        ac.delAppSettings(loginName);
                    }
                }
            }
        }
    }
}
