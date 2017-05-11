using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace Aisino.MES.Client.Common
{
    public class AisinoMessageBox
    {
        private DialogResult _result;

        public DialogResult Result
        {
            get { return _result; }
        }

        private TaskDialogInfo _info;

        public TaskDialogInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public AisinoMessageBox(string textTitle, string textHead, string textDetail, string textFoot,AisinoMessageButton btn, AisinoMessageIcon ico)
        {
            _info = new TaskDialogInfo(textTitle, (eTaskDialogIcon)ico,
                    textHead, textDetail, (eTaskDialogButton)btn, eTaskDialogBackgroundColor.Blue,
                    null, null, null, textFoot, Aisino.MES.Client.Common.Properties.Resources.About);
        }

        public AisinoMessageBox(string textTitle, string textHead, string textFoot, AisinoMessageButton btn, AisinoMessageIcon ico)
        {
            _info = new TaskDialogInfo(textTitle, (eTaskDialogIcon)ico,
                   textHead, null, (eTaskDialogButton)btn, eTaskDialogBackgroundColor.Blue,
                   null, null, null, textFoot, Aisino.MES.Client.Common.Properties.Resources.About);
        }

        public AisinoMessageBox(string textTitle, string textHead, AisinoMessageButton btn, AisinoMessageIcon ico)
        {
            _info = new TaskDialogInfo(textTitle, (eTaskDialogIcon)ico,
                   textHead, null, (eTaskDialogButton)btn, eTaskDialogBackgroundColor.Blue,
                   null, null, null, null, null);
        }

        public DialogResult ShowDialog()
        {
            eTaskDialogResult result = TaskDialog.Show(_info);
            
            if (result == eTaskDialogResult.Ok)
                _result = DialogResult.OK;
            if (result == eTaskDialogResult.Cancel)
                _result = DialogResult.Cancel;
            if (result == eTaskDialogResult.Yes)
                _result = DialogResult.Yes;
            if (result == eTaskDialogResult.No)
                _result = DialogResult.No;

            return _result;
        }
    }

    public enum AisinoMessageButton
    {
        OK = eTaskDialogButton.Ok,
        OKCancel = eTaskDialogButton.Ok | eTaskDialogButton.Cancel,
        YesNo = eTaskDialogButton.Yes | eTaskDialogButton.No,
        YesNoCancel = eTaskDialogButton.Yes | eTaskDialogButton.No | eTaskDialogButton.Cancel
    }

    public enum AisinoMessageIcon
    {
        Error = eTaskDialogIcon.ShieldStop,
        Warning = eTaskDialogIcon.Shield,
        Help = eTaskDialogIcon.ShieldHelp,
        Info = eTaskDialogIcon.ShieldOk
    }
}