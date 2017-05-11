using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Resources;
using LC.Model;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.SysManager.Windows.DeptAndUser
{
    /// <summary>
    /// EditUserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private UserEditModel selectSysUser;

        public UserEditModel SelectSysUser
        {
            get { return selectSysUser; }
            set { selectSysUser = value; }
        }


        IAsyncProxy<IUserModelService> userAsyncProxy = null;

        public EditUserWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                userAsyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                //AisinoMessageBox.Show("创建部门WCF代理失败！原因：" + strMsg, UIResources.MsgError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            if (Om == OperationMode.AddMode)
            {
                this.Title = "新增用户";
                txtUserCode.IsEnabled = true;
                chkIsStoped.IsEnabled = false;
                cmbSex.SelectedIndex = 0;
                BindUserInfo(null);
            }
            else
            {
                this.Title = "修改用户";
                txtUserCode.IsEnabled = false;
                chkIsStoped.IsEnabled = true;
                BindUserInfo(SelectSysUser);
            }
        }

        private async void BindUserInfo(UserEditModel selectUser)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (selectUser != null)
                {
                    txtUserCode.Text = selectUser.UserCode == null ? "" : selectUser.UserCode.Trim();
                    txtName.Text = selectUser.Name == null ? "" : selectUser.Name.Trim();
                    txtLoginName.Text = selectUser.LoginName == null ? "" : selectUser.LoginName.Trim();
                    dtBirthday.Text = selectUser.Birthday == null ? DateTime.Now.ToString() : selectUser.Birthday.ToString();
                    cmbSex.SelectedIndex = (int)(selectUser.Sex) - 1;  //此处前台最好能根据UserSex取值
                    txtMobile.Text = selectUser.Mobile == null ? "" : selectUser.Mobile.Trim();
                    txtOfficialPhone.Text = selectUser.OfficialPhone == null ? "" : selectUser.OfficialPhone.Trim();
                    txtEmail.Text = selectUser.Email == null ? "" : selectUser.Email.Trim().Trim();
                    txtPosition.Text = selectUser.Position == null ? "" : selectUser.Position.Trim();
                    txtRemark.Text = selectUser.Remark == null ? "" : selectUser.Remark.Trim();
                    chkNeedChangePassword.IsChecked = selectUser.NeedChangePassword;
                    chkIsLeader.IsChecked = selectUser.IsLeader;
                    chkIsStoped.IsChecked = selectUser.Stopped;
                    GetBytesByImageData(this.image, selectUser.Picture);
                    if (!selectUser.NeedChangePassword)
                    {
                        this.txtPassWord.IsEnabled = false;
                    }
                    this.txtPassWord.Password = selectUser.Password;
                }
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "绑定用户信息失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            #region 新增
            if (Om == OperationMode.AddMode)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    UserEditModel newUserEditModel = new UserEditModel();
                    //SysUser newSysUser = new SysUser();
                    newUserEditModel.UserCode = txtUserCode.Text.Trim();
                    newUserEditModel.Name = txtName.Text.Trim();
                    newUserEditModel.LoginName = txtLoginName.Text.Trim();
                    newUserEditModel.Password = "123456";
                    newUserEditModel.Sex = (UserSex)(cmbSex.SelectedIndex + 1);
                    //newSysUser.Picture
                    //newUserEditModel.Picture = this.GetBytesByImagePath(this.image.Tag.ToString());
                    newUserEditModel.Password = txtPassWord.Password.Trim();
                    newUserEditModel.Mobile = txtMobile.Text.Trim();
                    newUserEditModel.OfficialPhone = txtOfficialPhone.Text.Trim();
                    newUserEditModel.Email = txtEmail.Text.Trim();
                    newUserEditModel.Position = txtPosition.Text.Trim();
                    newUserEditModel.Remark = txtRemark.Text.Trim();
                    newUserEditModel.NeedChangePassword = chkNeedChangePassword.IsChecked.HasValue ? chkNeedChangePassword.IsChecked.Value : false;
                    newUserEditModel.IsLeader = chkIsLeader.IsChecked.HasValue ? chkIsLeader.IsChecked.Value : false;
                    newUserEditModel.IsOnline = false;
                    newUserEditModel.Stopped = false;


                    newUserEditModel = await userAsyncProxy.CallAsync(c => c.Add(newUserEditModel));
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "新增用户成功！", MessageDialogStyle.Affirmative, null);
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增用户成功！");
                    this.DialogResult = true;
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
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增用户失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("新增用户失败！原因：" + strMsg, UIResources.MsgError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }

            #endregion

            #region 修改
            else
            {
                string strErrorMsg = string.Empty;
                try
                {
                    SelectSysUser.Name = txtName.Text.Trim();
                    SelectSysUser.LoginName = txtLoginName.Text.Trim();
                    SelectSysUser.Sex = (UserSex)(cmbSex.SelectedIndex + 1);
                    //SelectSysUser.User.Picture
                    //SelectSysUser.Picture=this.GetBytesByImagePic(this.image, SelectSysUser.Picture);
                    SelectSysUser.Mobile = txtRemark.Text.Trim();
                    SelectSysUser.OfficialPhone = txtOfficialPhone.Text.Trim();
                    SelectSysUser.Email = txtEmail.Text.Trim();
                    SelectSysUser.Position = txtPosition.Text.Trim();
                    SelectSysUser.Remark = txtRemark.Text.Trim();
                    SelectSysUser.NeedChangePassword = chkNeedChangePassword.IsChecked.HasValue ? chkNeedChangePassword.IsChecked.Value : false;
                    SelectSysUser.IsLeader = chkIsLeader.IsChecked.HasValue ? chkIsLeader.IsChecked.Value : false;
                    SelectSysUser.Stopped = false;
                    SelectSysUser.Password = txtPassWord.Password.Trim();

                    SelectSysUser = await userAsyncProxy.CallAsync(c => c.Update(SelectSysUser));
                    //MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "修改用户成功！", MessageDialogStyle.Affirmative, null);
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改用户成功！");
                    this.DialogResult = true;
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
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改用户失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                    //AisinoMessageBox.Show("修改用户失败！原因：" + strMsg, UIResources.MsgError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
            #endregion
        }

        private  byte[] GetBytesByImagePath(object strFile)
        {
            string strErrorMsg = string.Empty;
            byte[] photo = null;
            try
            {
                if (strFile==null || strFile.ToString() == "")
                {
                    return null;
                }
                using (FileStream fs = new FileStream(strFile.ToString(), FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        photo = br.ReadBytes((int)fs.Length);
                        br.Close();
                    }
                }
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                //await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "选择图片转换！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
            return photo;
        }

        private byte[] GetBytesByImagePic(Image image, byte[] data)
        {
            if (image.Tag==null)
            {
                return data;
            }
           return this.GetBytesByImagePath(image.Tag);
        }
        private void GetBytesByImageData(Image image,byte[] data)
        {
            try
            {
                if (data == null)
                    return;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
                {
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(ms.ToArray());
                    bi.EndInit();
                    image.Source = bi;
                    ms.Close();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalObjects.blOpenCancelWarning == true)
            {
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "是否确认取消操作？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    this.DialogResult = false;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        int? _integerGreater10Property;
        public int? IntegerGreater10Property
        {
            get { return this._integerGreater10Property; }
            set
            {
                if (Equals(value, _integerGreater10Property))
                {
                    return;
                }

                _integerGreater10Property = value;
                RaisePropertyChanged("IntegerGreater10Property");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "IntegerGreater10Property" && this.IntegerGreater10Property < 10)
                {
                    return "Number is not greater than 10!";
                }

                //if (columnName == "DatePickerDate" && this.DatePickerDate == null)
                //{
                //    return "No date given!";
                //}

                return null;
            }
        }

        static void TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            SetTextLength(txtBox, txtBox.Text.Length);
        }

        private static void SetTextLength(DependencyObject obj, int value)
        {
            //obj.SetValue(TextLengthProperty, value);
            //obj.SetValue(HasTextProperty, value >= 1);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "所有图片|*.BMP;*.JPG;*.GIF;*.PNG;*.TIF";
                if (file.ShowDialog() == true)
                {
                    if (file.FileNames.Count() < 1)
                    {
                        return;
                    }
                    BitmapImage bp = new BitmapImage(new Uri(file.FileNames[0]));
                    this.image.Source = bp;
                    this.image.Tag = file.FileNames[0];
                }
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
                //AisinoMessageBox.Show(ex.Message, UIResources.MsgInfo, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "选择图片失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
    }
}
