using MahApps.Metro.Controls;
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
using Telerik.Windows.Controls;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using Aisino.MES.Client.Common;
using System.Reflection;
using Aisino.MES.Client.WPFCommon.BasePages;
using System.ComponentModel;
using WcfClientProxyGenerator.Async;
using Aisino.MES.Client.MainForms.Pages;
using MahApps.Metro.Controls.Dialogs;
using Aisino.MES.Resources;
using LC.Model.Business.SysManager;

namespace Aisino.MES.Client.MainForms
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        #region 属性
        private string _userCode;

        public string userCode
        {
            get { return _userCode; }
            set { _userCode = value; }
        }

        private IList<MainMenuModel> _lstUserMenuDisplayModel;

        public IList<MainMenuModel> lstUserMenuDisplayModel
        {
            get { return _lstUserMenuDisplayModel; }
            set { _lstUserMenuDisplayModel = value; }
        }

        private List<string> _openPageName;

        #endregion

        #region 窗体函数
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Visibility = System.Windows.Visibility.Visible;
            //隐藏操作界面，删除原操作界面所有部件
            mainView.Items.Clear();
            mainPanel.Visibility = System.Windows.Visibility.Hidden;

            MainPage mainPage = new MainPage();
            mainFrame.Navigate(mainPage);

            mainPage.SelectSubSystem += mainPage_SelectSubSystem;

        }

        void mainPage_SelectSubSystem(object sender, MainPage.SubSystemMenuArgs e)
        {
            this.lstUserMenuDisplayModel = e.lstMenuDisplayModel;
            if (lstUserMenuDisplayModel != null && lstUserMenuDisplayModel.Count > 0)
            {
                BuildMenu(lstUserMenuDisplayModel);
            }

            ToggleFlyout(0);
            _openPageName = new List<string>();

            SetMainPage(false);
        }


        //每个页面的关闭按钮事件处理
        private void CloseToggleButton_Click(object sender, RoutedEventArgs e)
        {
            RadButton rb = sender as RadButton;
            RadTileViewItem rtvi = rb.TemplatedParent as RadTileViewItem;
            int pageIndex = _openPageName.IndexOf(rtvi.Name);
            _openPageName.RemoveAt(pageIndex);
            mainView.Items.RemoveAt(pageIndex);
            if (_openPageName.Count == 1)
            {
                mainView.ColumnsCount = 1;
            }
        }

        #endregion

        #region 界面布局
        private void settingsFlyout_ClosingFinished(object sender, RoutedEventArgs e)
        {
            //临时处理，收缩底层填充画布，使操作区平铺
            tempCanvas.Width = 0;
        }

        //展开消息
        private void ShowMessage(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(1);
        }

        //展开菜单
        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            //含有菜单子项的时候才能展开菜单栏
            if(sp_menu.Children.Count > 0)
                ToggleFlyout(0);
        }

        //处理flyout
        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;

            if (index == 0)
            {
                if (flyout.IsOpen)
                {
                    tempCanvas.Width = 205;
                }
                else
                {
                    tempCanvas.Width = 0;
                }
            }
        }

        private void CloseFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = false;

            if (index == 0)
            {
                if (flyout.IsOpen)
                {
                    tempCanvas.Width = 205;
                }
                else
                {
                    tempCanvas.Width = 0;
                }
            }
        }
        #endregion

        #region 菜单处理
        //装载菜单
        private void BuildMenu(IList<MainMenuModel> mainMenus)
        {
            foreach (MainMenuModel mainMenu in mainMenus.Where(m => m.Layer == 1))
            {
                RadExpander parentRadExpander = new RadExpander();
                parentRadExpander.Tag = mainMenu;
                parentRadExpander.Header = mainMenu.DisplayName;
                parentRadExpander.Style = (Style)Resources["radExpanderHeader"];
                StackPanel sp_subMenu = new StackPanel();

                foreach (MainMenuModel subMenu in mainMenus.Where(m => m.ParentCode == mainMenu.Code))
                {
                    RadButton rb = new RadButton();
                    rb.Style = (Style)Resources["menuButton"];
                    rb.Tag = subMenu;
                    rb.Content = subMenu.DisplayName;
                    rb.Click += Menu_Click;
                    sp_subMenu.Children.Add(rb); 
                }

                parentRadExpander.Content = sp_subMenu;
                sp_menu.Children.Add(parentRadExpander);
            }
            //sampleRadCarousel.ItemsSource = mainMenus.Where(m => m.Layer == 1);
            //sampleRadCarousel.SelectedItem = mainMenus.Where(m => m.Layer == 1).First();
        }

        //菜单点击事件
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainMenuModel main = (sender as RadButton).Tag as MainMenuModel;
                if (main.ClassName != "")
                {
                    string assemblyName = main.Assembly; //所属程序集
                    string className = main.ClassName;//类名
                    Assembly ass = Assembly.Load(assemblyName);
                    Type pageType = ass.GetType(className);
                    if (_openPageName.Any(s => s == pageType.Name))
                    {
                        //如果已经存在，则不需要新建，直接激活
                        int pageIndex = _openPageName.IndexOf(pageType.Name);
                        mainView.MaximizedItem = mainView.Items[pageIndex];
                    }
                    else
                    {
                        Frame f = new Frame();
                        f.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
                        BusinessBasePage businessPage = (BusinessBasePage)Activator.CreateInstance(pageType);
                        f.Name = pageType.Name;
                        f.Content = businessPage.Title;
                        f.Navigate(businessPage);
                        RadTileViewItem rtvi = new RadTileViewItem();
                        rtvi.SizeChanged += rtvi_SizeChanged;
                        rtvi.Name = pageType.Name;
                        TileItemTitle tit = new TileItemTitle();
                        tit.name = businessPage.Title;
                        tit.Fontsize = 18;
                        rtvi.DataContext = tit;
                        rtvi.Content = f;

                        mainView.Items.Add(rtvi);
                        mainView.MaximizedItem = mainView.Items[mainView.Items.Count - 1];
                        _openPageName.Add(pageType.Name);

                        //只有超过一个界面打开的情况下才需要分两列显示
                        if (_openPageName.Count > 1)
                        {
                            mainView.ColumnsCount = 2;
                        }
                        else
                        {
                            mainView.ColumnsCount = 1;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, UIResources.MsgInfo, ex.ToString(), MessageDialogStyle.Affirmative, null);
            }
        }

        void rtvi_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RadTileViewItem rtvi = sender as RadTileViewItem;
            TileItemTitle tit = rtvi.DataContext as TileItemTitle;
            if (rtvi.ActualWidth <= 100)
            {
                tit.Fontsize = 8;
                (rtvi.Content as Frame).IsEnabled = false;
            }
            else
            {
                tit.Fontsize = 18;
                (rtvi.Content as Frame).IsEnabled = true;
            }
        }
        #endregion

        #region 传递函数(无用)
        //声明委托 发布者
        public delegate void OpenMenuEventHandler(object sender, DataEventArg e);
        //定义事件
        public event OpenMenuEventHandler OpenMenu;

        //定义事件参数，须继承EventArgs
        public class DataEventArg : EventArgs
        {
            private string title;

            public string Title
            {
                get { return title; }
            }
            public DataEventArg(string newTitle)
            {
                title = newTitle;
            }
        }
        //引发事件
        public void RaiseEvent(string newTitle)
        {
            DataEventArg e = new DataEventArg(newTitle);
            if (OpenMenu != null)
            {
                OpenMenu(this, e);
            }
        }
        #endregion

        private void btn_logout_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            Application.Current.MainWindow = loginForm;
            this.Close();
            loginForm.Show();
        }

        private void btn_returnHome_Click(object sender, RoutedEventArgs e)
        {
            SetMainPage(true);
        }

        private void SetMainPage(bool isMainPage)
        {
            if(isMainPage)
            {
                mainFrame.Visibility = System.Windows.Visibility.Visible;
                //隐藏操作界面，删除原操作界面所有部件
                mainView.Items.Clear();
                mainPanel.Visibility = System.Windows.Visibility.Hidden;

                //清除原有菜单
                sp_menu.Children.Clear();

                CloseFlyout(0);                
            }
            else
            {
                //隐藏主界面
                mainFrame.Visibility = System.Windows.Visibility.Hidden;
                //显示子系统界面
                mainPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void PageTitle_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject dobj = e.OriginalSource as DependencyObject;
            RadTileViewItem tileItem = FindParent<RadTileViewItem>(dobj);
            if (tileItem.TileState == TileViewItemState.Maximized)
            {
                tileItem.TileState = TileViewItemState.Minimized;
            }
            else
            {
                tileItem.TileState = TileViewItemState.Maximized;
            }
        }

        public static T FindParent<T>(DependencyObject i_dp) where T : DependencyObject
        {
            DependencyObject dobj = (DependencyObject)VisualTreeHelper.GetParent(i_dp);
            if (dobj != null)
            {
                if (dobj is T)
                {
                    return (T)dobj;
                }
                else
                {
                    dobj = FindParent<T>(dobj);
                    if (dobj != null && dobj is T)
                    {
                        return (T)dobj;
                    }
                }
            }
            return null;
        }
    }



    public class TileItemTitle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    //改变时通知
                    prochanged("name");
                }
            }
        }

        private int _fontsize;

        public int Fontsize
        {
            get { return _fontsize; }
            set
            {
                if (value != _fontsize)
                {
                    _fontsize = value;
                    //改变时通知
                    prochanged("Fontsize");
                }
            }
        }

        private void prochanged(string info)
        {
            if (PropertyChanged != null)
            {
                //是不是很奇怪，这个事件发起后，处理函数在哪里？
                //我也不知道在哪里，我只知道，绑定成功后WPF会帮我们决定怎么处理。
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
