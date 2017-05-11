using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Behaviours;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;

namespace Aisino.MES.Client.WPFCommon.BasePages
{
    public class BusinessBasePage : Page
    {
        public bool add = false;

        protected DialogMessage dialogmessage;
        public BusinessBasePage()
        {
            dialogmessage = new DialogMessage();
        }
        #region 关闭按钮事件
        /************************
         * 点击关闭按钮后，item接收并移除
         ************************/
        //public delegate void CloseDelegate(string pageName);
        public class PageCloseArgs : EventArgs
        {
            private string _pageName;
            public PageCloseArgs(string pageName)
            {
                this._pageName = pageName;
            }
            public string pageName
            {
                get { return _pageName; }
            }
        }

        public event EventHandler<PageCloseArgs> ClosePage;

        protected virtual void OnClosePage(PageCloseArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler<PageCloseArgs> handler = ClosePage;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        //public delegate void CloseHandle(Object sender, CloseArgs e);
        //public event CloseHandle CloseSend;

        #endregion
        protected virtual void SetRight()
        {
            Type t = this.GetType();
            //根据类型，用户，操作模式获取所有需要控制的权限
            //list<right> findrightby……
            //for循环，获取控件名称，设置对应的信息
            //(this.GetType().GetField("Add", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this) as MenuItem).IsEnabled = false;
            
        }

        //public async void ShowAutoCloseDialogOwter(string title, string message)
        //{
        //    dialogmessage.Title = title;
        //    dialogmessage.Message = message;
        //    var dialog = (BaseMetroDialog)this.Resources["AutoCloseDialog"];
        //    dialog.DataContext = dialogmessage;

        //    await this.ShowMetroDialogAsync(dialog);

        //    await TaskEx.Delay(GlobalObjects.AutoClose);

        //    await this.HideMetroDialogAsync(dialog);
        //}

        public MetroWindow GetMainWindow()
        {
            //所有page弹出提示框，统一使用当前系统主窗口
            return Application.Current.MainWindow as MetroWindow;
        }

        public MetroWindow GetMetroParentWindow(DependencyObject dependencyObject)
        {
            DependencyObject element = dependencyObject;
            while (true)
            {
                element = VisualTreeHelper.GetParent(element);
                if (element is MetroWindow)
                {
                    break;
                }
            }
            return element as MetroWindow;
        }

        public Window GetParentWindow(DependencyObject dependencyObject)
        {
            DependencyObject element = dependencyObject;
            while (true)
            {
                element = VisualTreeHelper.GetParent(element);
                if (element is Window)
                {
                    break;
                }
            }
            return element as Window;
        }

        public async void ShowAutoCloseDialogInner(string title,string message)
        {
            dialogmessage.Title = title;
            dialogmessage.Message = message;
            var dialog = (BaseMetroDialog)this.Resources["AutoCloseDialog"];
            dialog.DataContext = dialogmessage;
            await this.GetMainWindow().ShowMetroDialogAsync(dialog);

            await TaskEx.Delay(GlobalObjects.AutoClose);

            await this.GetMainWindow().HideMetroDialogAsync(dialog);
        }
    }

    
}
