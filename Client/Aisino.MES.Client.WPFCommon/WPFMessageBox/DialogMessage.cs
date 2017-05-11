using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aisino.MES.Client.WPFCommon.WPFMessageBox
{
    public class DialogMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    //改变时通知
                    prochanged("Title");
                }
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    //改变时通知
                    prochanged("Message");
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
