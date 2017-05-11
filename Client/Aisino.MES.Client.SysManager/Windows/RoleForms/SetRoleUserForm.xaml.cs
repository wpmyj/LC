using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Resources;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
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

namespace Aisino.MES.Client.SysManager.Windows.RoleForms
{
    /// <summary>
    /// SetRoleUserForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetRoleUserForm
    {
        private RoleDisplayModel _roleDisplayModel;

        public RoleDisplayModel RoleDisplayModel
        {
            get { return _roleDisplayModel; }
            set { _roleDisplayModel = value; }
        }

        private IAsyncProxy<IUserModelService> userAyncProxy;

        private IAsyncProxy<IRoleModelService> roleAyncProxy;
        public SetRoleUserForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnAllToRight.Content = ">>";
            this.btnPartToRight.Content = ">";
            this.btnAllToLeft.Content = "<<";
            this.btnPartToLeft.Content = "<";
            bindUserList();
        }

        private async void bindUserList()
        {
            userAyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
            roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
            IList<UserDisplayModel> allUser = await userAyncProxy.CallAsync(u => u.GetAllUser());
            RoleEditModel roleEditModel = await roleAyncProxy.CallAsync(c => c.GetRoleByCode(this._roleDisplayModel.Code));
            for (int i = allUser.Count - 1; i >= 0; i--)
            {
                bool flag = true;
                UserDisplayModel userDisplayModel = allUser[i];
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = userDisplayModel.Name;
                listViewItem.Tag = userDisplayModel;
                foreach (UserEditModel sysUser in roleEditModel.SysUsers)
                {
                    if (userDisplayModel.Code == sysUser.UserCode)
                    {
                        this.backUser.Items.Add(listViewItem);
                        flag = false;
                        break;
                    }

                }
                if (flag)
                {
                    this.forceUser.Items.Add(listViewItem);
                }
            }
        }

        private void allToRight_Click(object sender, RoutedEventArgs e)
        {
            if (this.forceUser.Items.Count == 0)
            {
                return;
            }
            List<ListViewItem> listViewItemS = new List<ListViewItem>();
            foreach (ListViewItem item in this.forceUser.Items)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = item.Content;
                listViewItem.Tag = item.Tag;
                listViewItemS.Add(listViewItem);
            }
            this.forceUser.Items.Clear();
            foreach (var item in listViewItemS)
            {
                this.backUser.Items.Add(item);
            }
        }

        private void partToRight_Click(object sender, RoutedEventArgs e)
        {
            if (this.forceUser.Items.Count == 0 || this.forceUser.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Content = ((ListViewItem)this.forceUser.SelectedItem).Content;
            listViewItem.Tag = ((ListViewItem)this.forceUser.SelectedItem).Tag;
            this.backUser.Items.Add(listViewItem);
            this.forceUser.Items.Remove((ListViewItem)this.forceUser.SelectedItem);
        }

        private void allToLeft_Click(object sender, RoutedEventArgs e)
        {
            if (this.backUser.Items.Count == 0)
            {
                return;
            }
            List<ListViewItem> listViewItemS = new List<ListViewItem>();
            foreach (ListViewItem item in this.backUser.Items)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = item.Content;
                listViewItem.Tag = item.Tag;
                listViewItemS.Add(listViewItem);
            }
            this.backUser.Items.Clear();
            foreach (var item in listViewItemS)
            {
                this.forceUser.Items.Add(item);
            }
        }

        private void partToLeft_Click(object sender, RoutedEventArgs e)
        {
            if (this.backUser.Items.Count == 0 || this.backUser.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Content = ((ListViewItem)this.backUser.SelectedItem).Content;
            listViewItem.Tag = ((ListViewItem)this.backUser.SelectedItem).Tag;
            this.forceUser.Items.Add(listViewItem);
            this.backUser.Items.Remove((ListViewItem)this.backUser.SelectedItem);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
            userAyncProxy = await Task.Run(() => ServiceHelper.GetUserService());
            RoleEditModel roleEditModel = await roleAyncProxy.CallAsync(c => c.GetRoleByCode(this._roleDisplayModel.Code));
            List<string> userCodeList = new List<string>();
            List<string> rightCodeList = new List<string>();
            foreach (ListViewItem item in this.backUser.Items)
            {
                userCodeList.Add(((UserDisplayModel)(item.Tag)).Code);
            }
            foreach (RightEditModel sysRight in roleEditModel.SysRights)
            {
                rightCodeList.Add(sysRight.RightCode);
            }
            await roleAyncProxy.CallAsync(p => p.Update(roleEditModel, userCodeList, rightCodeList));
            this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增用户成功！");
            this.Close();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "是否确认取消操作？", MessageDialogStyle.AffirmativeAndNegative, null);
            //MessageBoxResult result = AisinoMessageBox.Show("数据还没有保存！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK);
            if (result == MessageDialogResult.Affirmative)
            {
                this.Close();
            }
        }
    }
}
