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

namespace Aisino.MES.Client.WPFCommon.Controls
{
    /// <summary>
    /// Sure.xaml 的交互逻辑
    /// </summary>
    public partial class Confirm : UserControl  //,ICommand
    {
        public Confirm()
        {
            InitializeComponent();
        }

        //public ICommand Command { get; set; }

        //public object CommandParameter { get; set; }

        //public IInputElement CommandTarget { get; set; }

        //public bool CanExecute(object parameter)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 当命令可执行状态发生改变时，应激发该事件。
        ///// </summary>
        //public event EventHandler CanExecuteChanged;

        ///// <summary>
        ///// 命令被执行，执行与业务相关的Clear逻辑。
        ///// </summary>
        ///// <param name="parameter">执行命令的目标对象。</param>
        //public void Execute(object parameter)
        //{
        //    //IView view = parameter as IView;
        //    //if (view != null) view.Clear();
        //}

    }
}
