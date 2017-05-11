using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Client.Common.BaseControl
{
    public partial class BaseMdiChildForm : DevComponents.DotNetBar.Office2007Form
    {
        public BaseMdiChildForm()
        {
            InitializeComponent();
        }

        protected SysMenu _sysMenu;

        public SysMenu SysMenu
        {
            get { return _sysMenu; }
            set { _sysMenu = value; }
        }
    }
}