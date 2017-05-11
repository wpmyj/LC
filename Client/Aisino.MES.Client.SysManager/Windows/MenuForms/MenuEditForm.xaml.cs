using Aisino.MES.Client.Common;
using Aisino.MES.Resources;
using LC.Model;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
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

namespace Aisino.MES.Client.SysManager.Windows.MenuForms
{
    /// <summary>
    /// MenuEditForm.xaml 的交互逻辑
    /// </summary>
    public partial class MenuEditForm
    {

        public MenuDisplayModel _menuDisplayModel;
        private MenuEditModel _sysmenu;
        private IAsyncProxy<IMenuModelService> _menuAyncProxy;
        private IAsyncProxy<IModuleModelService> _moduleAyncProxy;
        private OperationMode _om;
        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }
        public MenuEditForm()
        {
            InitializeComponent();
        }
        private void MenuMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmbMenuType.ItemsSource = Enum.GetValues(typeof(MenuType));
            this.menuConvert();
            if (Om == OperationMode.AddMode)
            {
                this.MenuMain.Title = "添加菜单";
                this.cmbMenuType.SelectedIndex = 0;
                this.txtShowIndex.Value = 0;
            }
            if (Om == OperationMode.EditMode)
            {
                this.MenuMain.Title = "修改菜单";
                this.txtName.IsEnabled = false;
                this.txtMenuCode.IsEnabled = false;
            }
        }
        private async void menuConvert()
        {
            string strErrorMsg = string.Empty;
            try
            {
                _menuAyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                _sysmenu = await _menuAyncProxy.CallAsync(c => c.GetMenuByCode(this._menuDisplayModel.Code));
                MenuDisplayModel rootMenuDisplay = await _menuAyncProxy.CallAsync(c => c.GetRootMenu());

                _moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
                IList<ModuleDisplayModel> moduleLists = await _moduleAyncProxy.CallAsync(c => c.GetAllModules());
                this.cmbModule.ItemsSource = moduleLists;

                List<MenuDisplayModel> menuList = new List<MenuDisplayModel>();
                menuList.Add(rootMenuDisplay);
                this.menuCmb(rootMenuDisplay, menuList);
                this.cmbParent.ItemsSource = menuList;
                if (Om == OperationMode.AddMode)
                {
                    this.cmbParent.Text = this._menuDisplayModel.Name;
                }
                if (Om == OperationMode.EditMode)
                {
                    if (this._sysmenu != null && this._sysmenu.ParentCode != null)
                    {
                        MenuEditModel rootMenuParent = await _menuAyncProxy.CallAsync(c => c.GetMenuByCode(this._sysmenu.ParentCode));
                        this.cmbParent.Text = rootMenuParent.DisplayName;
                        this.cmbMenuType.Text = this._sysmenu.Type.ToString();
                        this.txtMenuCode.Text = this._sysmenu.MenuCode;
                        this.txtName.Text = this._sysmenu.Name;
                        this.txtDisplayName.Text = this._sysmenu.DisplayName;
                        this.txtMenuRemark.Text = this._sysmenu.Remark;
                        this.txtShowIndex.Value = this._sysmenu.ShowIndex;
                        if (_sysmenu.ParentModule != null && _sysmenu.ParentFunction != null)
                        {
                            _moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
                            ModuleEditModel moduleEditModel = await _moduleAyncProxy.CallAsync(c => c.GetModuleByCode(_sysmenu.ParentModule.ModuleCode));
                            this.cmbModule.Text = _sysmenu.ParentModule.Name;
                            this.cmbFunction.ItemsSource = moduleEditModel.SysFunctions;
                            this.cmbFunction.Text = _sysmenu.ParentFunction.Name;
                        }
                        if (_sysmenu.Layer < 2)
                        {
                            this.cmbModule.IsEnabled = false;
                            this.cmbFunction.IsEnabled = false;
                        }
                    }
                    else
                    {
                        this.cmbModule.IsEnabled = false;
                        this.cmbFunction.IsEnabled = false;
                        this.cmbParent.IsEnabled = false;
                        this.txtMenuCode.Text = this._menuDisplayModel.Code;
                        this.txtName.Text = this._menuDisplayModel.Name;
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
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "异常原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }
        }
        private void menuCmb(MenuDisplayModel menuDisplayModel, List<MenuDisplayModel> menuList)
        {
            if (menuDisplayModel.SubMenus != null || menuDisplayModel.SubMenus.Count > 0)
            {
                foreach (var item in menuDisplayModel.SubMenus)
                {
                    menuList.Add(item);
                    menuCmb(item, menuList);
                }
            }
        }
        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                MenuEditModel menuEditModel = new MenuEditModel();
                menuEditModel.MenuCode = this.txtMenuCode.Text;
                menuEditModel.Name = this.txtName.Text;
                menuEditModel.DisplayName = this.txtDisplayName.Text;
                menuEditModel.Remark = this.txtMenuRemark.Text;
                menuEditModel.ShowIndex = (int)this.txtShowIndex.Value;
                if (this.cmbModule.SelectedItem != null)
                {
                    menuEditModel.ModuleCode = ((ModuleDisplayModel)this.cmbModule.SelectedItem).Code;

                }
                menuEditModel.ParentCode = ((MenuDisplayModel)this.cmbParent.SelectedItem).Code;
                if (this.cmbFunction.SelectedItem != null)
                {
                    menuEditModel.FunctionCode = ((FunctionEditModel)this.cmbFunction.SelectedItem).FunctionCode;
                }
                MenuType menuType = (MenuType)Enum.Parse(typeof(MenuType), this.cmbMenuType.Text);
                menuEditModel.Type = menuType;
                if (Om == OperationMode.AddMode)
                {
                    _menuAyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                    await _menuAyncProxy.CallAsync(c => c.Add(menuEditModel, false));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "新增菜单成功！");
                    this.DialogResult = true;
                    this.Close();
                }
                if (Om == OperationMode.EditMode)
                {
                    _menuAyncProxy = await Task.Run(() => ServiceHelper.GetMenuService());
                    await _menuAyncProxy.CallAsync(c => c.Update(menuEditModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "编辑菜单成功！");
                    this.DialogResult = true;
                    this.Close();
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
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "保存数据失败原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
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

        private async void cmbModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (this.cmbModule.SelectedItem == null)
                {
                    return;
                }
                _moduleAyncProxy = await Task.Run(() => ServiceHelper.GetModuleService());
                ModuleEditModel moduleEditModel = await _moduleAyncProxy.CallAsync(c => c.GetModuleByCode(((ModuleDisplayModel)this.cmbModule.SelectedItem).Code));
                this.cmbFunction.ItemsSource = moduleEditModel.SysFunctions;
                if (this.Om == OperationMode.EditMode &&this. _sysmenu.ParentFunction!=null)
                {
                    this.cmbFunction.Text = _sysmenu.ParentFunction.Name;
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
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "失败原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }
        }
    }
}
