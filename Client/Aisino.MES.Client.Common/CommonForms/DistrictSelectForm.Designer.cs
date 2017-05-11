namespace Aisino.MES.Client.Common.CommonForms
{
    partial class DistrictSelectForm
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
            this.advtreedistic = new DevComponents.AdvTree.AdvTree();
            this.columnHeader1 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader2 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader3 = new DevComponents.AdvTree.ColumnHeader();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.btnok = new DevComponents.DotNetBar.ButtonX();
            this.btncancel = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.advtreedistic)).BeginInit();
            this.SuspendLayout();
            // 
            // advtreedistic
            // 
            this.advtreedistic.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advtreedistic.AllowDrop = true;
            this.advtreedistic.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advtreedistic.BackgroundStyle.Class = "TreeBorderKey";
            this.advtreedistic.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.advtreedistic.Columns.Add(this.columnHeader1);
            this.advtreedistic.Columns.Add(this.columnHeader2);
            this.advtreedistic.Columns.Add(this.columnHeader3);
            this.advtreedistic.Location = new System.Drawing.Point(12, 12);
            this.advtreedistic.Name = "advtreedistic";
            this.advtreedistic.NodesConnector = this.nodeConnector1;
            this.advtreedistic.NodeStyle = this.elementStyle1;
            this.advtreedistic.PathSeparator = ";";
            this.advtreedistic.Size = new System.Drawing.Size(490, 325);
            this.advtreedistic.Styles.Add(this.elementStyle1);
            this.advtreedistic.TabIndex = 0;
            this.advtreedistic.Text = "advTree1";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "产地代码";
            this.columnHeader1.Width.Absolute = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "产地名称";
            this.columnHeader2.Width.Absolute = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Name = "columnHeader3";
            this.columnHeader3.Text = "备注";
            this.columnHeader3.Width.Absolute = 150;
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
            // btnok
            // 
            this.btnok.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnok.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnok.Location = new System.Drawing.Point(346, 343);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(75, 23);
            this.btnok.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnok.TabIndex = 1;
            this.btnok.Text = "确定";
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // btncancel
            // 
            this.btncancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btncancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btncancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btncancel.Location = new System.Drawing.Point(427, 343);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(75, 23);
            this.btncancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btncancel.TabIndex = 2;
            this.btncancel.Text = "取消";
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // DistrictSelectForm
            // 
            this.AcceptButton = this.btnok;
            this.CancelButton = this.btncancel;
            this.ClientSize = new System.Drawing.Size(515, 378);
            this.Controls.Add(this.advtreedistic);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnok);
            this.EnableGlass = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DistrictSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择产地";
            this.Load += new System.EventHandler(this.DistrictSelectForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.advtreedistic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree advtreedistic;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private DevComponents.AdvTree.ColumnHeader columnHeader2;
        private DevComponents.AdvTree.ColumnHeader columnHeader3;
        private DevComponents.DotNetBar.ButtonX btnok;
        private DevComponents.DotNetBar.ButtonX btncancel;
    }
}