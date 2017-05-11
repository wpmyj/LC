namespace Aisino.MES.Client.Common.CommonForms
{
    partial class SelectBorLineForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.advBorLine = new DevComponents.AdvTree.AdvTree();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.btncancel = new DevComponents.DotNetBar.ButtonX();
            this.btnok = new DevComponents.DotNetBar.ButtonX();
            this.columnHeader1 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader2 = new DevComponents.AdvTree.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.advBorLine)).BeginInit();
            this.SuspendLayout();
            // 
            // advBorLine
            // 
            this.advBorLine.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advBorLine.AllowDrop = true;
            this.advBorLine.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advBorLine.BackgroundStyle.Class = "TreeBorderKey";
            this.advBorLine.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.advBorLine.Columns.Add(this.columnHeader1);
            this.advBorLine.Columns.Add(this.columnHeader2);
            this.advBorLine.Location = new System.Drawing.Point(3, 9);
            this.advBorLine.Name = "advBorLine";
            this.advBorLine.NodesConnector = this.nodeConnector1;
            this.advBorLine.NodeStyle = this.elementStyle1;
            this.advBorLine.PathSeparator = ";";
            this.advBorLine.Size = new System.Drawing.Size(490, 325);
            this.advBorLine.Styles.Add(this.elementStyle1);
            this.advBorLine.TabIndex = 0;
            this.advBorLine.Text = "advTree1";
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.Class = "";
            this.elementStyle1.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // btncancel
            // 
            this.btncancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btncancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btncancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btncancel.Location = new System.Drawing.Point(417, 339);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(75, 23);
            this.btncancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btncancel.TabIndex = 10;
            this.btncancel.Text = "取消";
            // 
            // btnok
            // 
            this.btnok.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnok.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnok.Location = new System.Drawing.Point(336, 339);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(75, 23);
            this.btnok.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnok.TabIndex = 9;
            this.btnok.Text = "确定";
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "工艺路线编号";
            this.columnHeader1.Width.Absolute = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "工艺路线名称";
            this.columnHeader2.Width.Absolute = 150;
            // 
            // SelectBorLineForm
            // 
            this.ClientSize = new System.Drawing.Size(495, 362);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnok);
            this.Controls.Add(this.advBorLine);
            this.DoubleBuffered = true;
            this.Name = "SelectBorLineForm";
            this.Text = "工艺路线选择";
            this.Load += new System.EventHandler(this.SelectBorLineForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.advBorLine)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree advBorLine;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private DevComponents.AdvTree.ColumnHeader columnHeader2;
        private DevComponents.DotNetBar.ButtonX btncancel;
        private DevComponents.DotNetBar.ButtonX btnok;
    }
}