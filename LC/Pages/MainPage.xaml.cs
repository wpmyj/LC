using Aisino.MES.Client.Common;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.MainForms.Pages
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        #region 选择子系统委托
        /************************
         * 选择子系统后，主界面可以接受成功信息
         * 主界面可以根据接收到的信息按需处理
         ************************/
        private delegate void SelectSubSystemDelegate(IList<MainMenuModel> lstMenuDisplayModel);
        public class SubSystemMenuArgs : EventArgs
        {
            private IList<MainMenuModel> _lstMenuDisplayModel;
            public SubSystemMenuArgs(IList<MainMenuModel> lstMenuDisplayModel)
            {
                this._lstMenuDisplayModel = lstMenuDisplayModel;
            }

            public IList<MainMenuModel> lstMenuDisplayModel
            {
                get { return _lstMenuDisplayModel; }
            }
        }
        public delegate void SelectSubSystemHandle(Object sender, SubSystemMenuArgs e);
        public event SelectSubSystemHandle SelectSubSystem;

        #endregion

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string usercode = GlobalObjects.currentLoginUser.UserCode;
            IAsyncProxy<ISubSystemModelService> asyncProxySubSystem = await Task.Run(() => ServiceHelper.GetSubSystemService());
            IList<SubSystemDisplayModel> LstSubSystemDisplayModel = await asyncProxySubSystem.CallAsync(c => c.FindSubSystemByUserCode(usercode.Trim()));
            foreach(SubSystemDisplayModel ssdm in LstSubSystemDisplayModel)
            {
                TextBlock tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Inlines.Add(new Bold(new Run(ssdm.SubSystemName)));
                tb.Inlines.Add(new LineBreak());
                tb.Inlines.Add(new Run(ssdm.Remark));

                Tile tile = new Tile();
                //tile.Content = ssdm.SubSystemName;
                tile.Content = tb;
                tile.FontSize = 18;
                tile.Tag = ssdm.SubSystemCode;
                tile.TileType = ssdm.MetroType == "Single" ? TileType.Single : ssdm.MetroType == "Double" ? TileType.Double : TileType.Quadruple;
                BrushConverter brushConverter = new BrushConverter(); 
                tile.Background =  (Brush)brushConverter.ConvertFromString(ssdm.BackColor);
                tile.MouseLeftButtonUp += tile_MouseLeftButtonUp;
                
                
                
                //考虑如何增加图形
                //Path path = new Path();
                //var gc=new GeometryConverter();
                //path.Data = (Geometry)gc.ConvertFromString("F1 M 26.3223,22.7077L 43.3206,22.7077L 43.3206,27.1212L 31.6976,27.1212L 31.6976,35.2485L 42.535,35.2485L 42.535,39.6624L 31.6976,39.6624L 31.6976,52.1595L 26.3223,52.1595L 26.3223,22.7077 Z M 46.073,52.1595L 46.073,21.1345L 51.491,21.1345L 51.491,52.1595L 46.073,52.1595 Z ");
                
                MainTileList.Items.Add(tile);
            }
        }

        async void tile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string subSystemCode = (sender as Tile).Tag.ToString();
            IAsyncProxy<IMenuModelService> asyncProxySysMenu = await Task.Run(() => ServiceHelper.GetMenuService());
            IList<MainMenuModel> LstMenuDisplayModel = await asyncProxySysMenu.CallAsync(c => c.FindMenusByUserCodeAndSubSystemCode(GlobalObjects.currentLoginUser.UserCode, subSystemCode));
            SelectSubSystem(this, new SubSystemMenuArgs(LstMenuDisplayModel));
        }
    }
}
