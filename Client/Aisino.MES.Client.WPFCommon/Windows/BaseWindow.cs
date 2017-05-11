using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.WPFMessageBox;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Aisino.MES.Client.WPFCommon.Windows
{
    public class BaseWindow : MetroWindow
    {
        protected DialogMessage dialogmessage;
        public BaseWindow()
        {
            dialogmessage = new DialogMessage();
        }

        public async void ShowAutoCloseDialogOwter(string title, string message)
        {
            dialogmessage.Title = title;
            dialogmessage.Message = message;
            var dialog = (BaseMetroDialog)this.Resources["AutoCloseDialog"];
            dialog.DataContext = dialogmessage;

            await this.ShowMetroDialogAsync(dialog);

            await TaskEx.Delay(GlobalObjects.AutoClose);

            await this.HideMetroDialogAsync(dialog);
        }
        public MetroWindow GetMainWindow()
        {
            //所有page弹出提示框，统一使用当前系统主窗口
            return Application.Current.MainWindow as MetroWindow;
        }

    }
}
