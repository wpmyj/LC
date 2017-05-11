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
    /// SetRoleRightForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetRoleRightForm
    {
        private RoleDisplayModel _roleDisplayModel;
        private IAsyncProxy<IRightModelService> rightAyncProxy;
        private IAsyncProxy<IRoleModelService> roleAyncProxy;

        public RoleDisplayModel RoleDisplayModel
        {
            get { return _roleDisplayModel; }
            set { _roleDisplayModel = value; }
        }

        public SetRoleRightForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnAllToRight.Content = ">>";
            this.btnPartToRight.Content = ">";
            this.btnAllToLeft.Content = "<<";
            this.btnPartToLeft.Content = "<";
            bindRightList();
        }

        private async void bindRightList()
        {
            rightAyncProxy = await Task.Run(() => ServiceHelper.GetRightService());
            roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
            IList<RightDisplayModel> allRight = await rightAyncProxy.CallAsync(u => u.GetAllRights());
            RoleEditModel roleEditModel = await roleAyncProxy.CallAsync(c => c.GetRoleByCode(this._roleDisplayModel.Code));
            for (int i = allRight.Count - 1; i >= 0; i--)
            {
                bool flag = true;
                RightDisplayModel rightDisplayModel = allRight[i];
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = rightDisplayModel.Name;
                listViewItem.Tag = rightDisplayModel;
                foreach (RightEditModel sysRight in roleEditModel.SysRights)
                {
                    if (rightDisplayModel.Code == sysRight.RightCode)
                    {
                        this.backRight.Items.Add(listViewItem);
                        flag = false;
                        break;
                    }

                }
                if (flag)
                {
                    this.forceRight.Items.Add(listViewItem);
                }
            }
        }

        private void btnAllToRight_Click(object sender, RoutedEventArgs e)
        {
            if (this.forceRight.Items.Count == 0)
            {
                return;
            }
            List<ListViewItem> listViewItemS = new List<ListViewItem>();
            foreach (ListViewItem item in this.forceRight.Items)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = item.Content;
                listViewItem.Tag = item.Tag;
                listViewItemS.Add(listViewItem);
            }
            this.forceRight.Items.Clear();
            foreach (var item in listViewItemS)
            {
                this.backRight.Items.Add(item);
            }
        }
        
        private void btnPartToRight_Click(object sender, RoutedEventArgs e)
        {
            if (this.forceRight.Items.Count == 0 || this.forceRight.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Content = ((ListViewItem)this.forceRight.SelectedItem).Content;
            listViewItem.Tag = ((ListViewItem)this.forceRight.SelectedItem).Tag;
            this.backRight.Items.Add(listViewItem);
            this.forceRight.Items.Remove((ListViewItem)this.forceRight.SelectedItem);
        }

        private void btnAllToLeft_Click(object sender, RoutedEventArgs e)
        {
            if (this.backRight.Items.Count == 0)
            {
                return;
            }
            List<ListViewItem> listViewItemS = new List<ListViewItem>();
            foreach (ListViewItem item in this.backRight.Items)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = item.Content;
                listViewItem.Tag = item.Tag;
                listViewItemS.Add(listViewItem);
            }
            this.backRight.Items.Clear();
            foreach (var item in listViewItemS)
            {
                this.forceRight.Items.Add(item);
            }
        }

        private void btnPartToLeft_Click(object sender, RoutedEventArgs e)
        {
            if (this.backRight.Items.Count == 0 || this.backRight.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Content = ((ListViewItem)this.backRight.SelectedItem).Content;
            listViewItem.Tag = ((ListBoxItem)this.backRight.SelectedItem).Tag;
            this.forceRight.Items.Add(listViewItem);
            this.backRight.Items.Remove((ListViewItem)this.backRight.SelectedItem);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            roleAyncProxy = await Task.Run(() => ServiceHelper.GetRoleService());
            rightAyncProxy = await Task.Run(() => ServiceHelper.GetRightService());
            RoleEditModel roleEditModel = await roleAyncProxy.CallAsync(c => c.GetRoleByCode(this._roleDisplayModel.Code));
            List<string> userCodeList = new List<string>();
            List<string> rightCodeList = new List<string>();
            foreach (ListViewItem item in this.backRight.Items)
            {
                rightCodeList.Add(((RightDisplayModel)(item.Tag)).Code);
            }
            foreach (UserEditModel sysUser in roleEditModel.SysUsers)
            {
                userCodeList.Add(sysUser.UserCode);
            }
            await roleAyncProxy.CallAsync(p => p.Update(roleEditModel, userCodeList, rightCodeList));
            this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增角色成功！");
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
