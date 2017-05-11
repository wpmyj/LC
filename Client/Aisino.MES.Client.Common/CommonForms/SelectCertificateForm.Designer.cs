namespace Aisino.MES.Client.Common.CommonForms
{
    partial class SelectCertificateForm
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
            this.advcertificate = new DevComponents.AdvTree.AdvTree();
            this.columnHeader1 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader2 = new DevComponents.AdvTree.ColumnHeader();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.btncancel = new DevComponents.DotNetBar.ButtonX();
            this.btnok = new DevComponents.DotNetBar.ButtonX();
            this.columnHeader3 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader4 = new DevComponents.AdvTree.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.advcertificate)).BeginInit();
            this.SuspendLayout();
            // 
            // advcertificate
            // 
            this.advcertificate.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advcertificate.AllowDrop = true;
            this.advcertificate.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advcertificate.BackgroundStyle.Class = "TreeBorderKey";
            this.advcertificate.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.advcertificate.Columns.Add(this.columnHeader1);
            this.advcertificate.Columns.Add(this.columnHeader2);
            this.advcertificate.Columns.Add(this.columnHeader3);
            this.advcertificate.Columns.Add(this.columnHeader4);
            this.advcertificate.Location = new System.Drawing.Point(0, 4);
            this.advcertificate.Name = "advcertificate";
            this.advcertificate.NodesConnector = this.nodeConnector1;
            this.advcertificate.NodeStyle = this.elementStyle1;
            this.advcertificate.PathSeparator = ";";
            this.advcertificate.Size = new System.Drawing.Size(622, 325);
            this.advcertificate.Styles.Add(this.elementStyle1);
            this.advcertificate.TabIndex = 0;
            this.advcertificate.Text = "advTree1";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "凭证单号";
            this.columnHeader1.Width.Absolute = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "备注";
            this.columnHeader2.Width.Absolute = 150;
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
            this.btncancel.Location = new System.Drawing.Point(537, 335);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(75, 23);
            this.btncancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btncancel.TabIndex = 8;
            this.btncancel.Text = "取消";
            // 
            // btnok
            // 
            this.btnok.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnok.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnok.Location = new System.Drawing.Point(456, 335);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(75, 23);
            this.btnok.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnok.TabIndex = 7;
            this.btnok.Text = "确定";
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Name = "columnHeader3";
            this.columnHeader3.Text = "实提数量";
            this.columnHeader3.Width.Absolute = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Name = "columnHeader4";
            this.columnHeader4.Text = "凭证数量";
            this.columnHeader4.Width.Absolute = 150;
            // 
            // SelectCertificateForm
            // 
            this.ClientSize = new System.Drawing.Size(620, 362);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnok);
            this.Controls.Add(this.advcertificate);
            this.Name = "SelectCertificateForm";
            this.Text = "凭证选择";
            this.Load += new System.EventHandler(this.SelectCertificateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.advcertificate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree advcertificate;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private DevComponents.AdvTree.ColumnHeader columnHeader2;
        private DevComponents.DotNetBar.ButtonX btncancel;
        private DevComponents.DotNetBar.ButtonX btnok;
        private DevComponents.AdvTree.ColumnHeader columnHeader3;
        private DevComponents.AdvTree.ColumnHeader columnHeader4;
    }
}