using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aisino.MES.Client.Common.CommonForms;
using System.Windows.Forms;

namespace Aisino.MES.Client.Common
{
    public enum UseType
    {
        加载数据,
        保存数据
    }
    public static class WaitFormService
    {
        static WaitForm form = null;
        public static void Show(UseType ut,Form mainform)
        {
            if (form == null)
            {
                form = new WaitForm();
                ThreadPool.QueueUserWorkItem(state =>
                {
                    form.TopMost = true;
                    form.ShowDialog(mainform);
                });
            }
            if (ut == 0)
            {
                form.Msg = "数据加载中……";
            }
            else
            {
                form.Msg = "数据保存中……";
            }
        }
        public static void Close()
        {
            if (form != null)
                form.Close();
            form = null;
        }
    }

}
