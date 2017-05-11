using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class WaitForm : Form
    {
        Action<string> setMsg;
        Action close;
        public WaitForm()
        {
            InitializeComponent();
            setMsg = invoke;
            close = base.Close;
        }
        public string Msg
        {
            get
            {
                return this.label1.Text;
            }
            set
            {
                while (!this.IsHandleCreated) ;
                this.Invoke(setMsg, value);
            }
        }
        public new void Close()
        {
            while (!this.IsHandleCreated) ;
            this.Invoke(close);
        }
        void invoke(string msg)
        {
            this.label1.Text = msg;
        }
    }
}
