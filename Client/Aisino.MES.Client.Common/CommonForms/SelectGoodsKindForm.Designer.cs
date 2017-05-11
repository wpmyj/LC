namespace Aisino.MES.Client.Common.CommonForms
{
    partial class SelectGoodsKindForm
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
            this.treGoodsKind = new DevComponents.AdvTree.AdvTree();
            this.rootNode = new DevComponents.AdvTree.Node();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.btnOk = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.treGoodsKind)).BeginInit();
            this.SuspendLayout();
            // 
            // treGoodsKind
            // 
            this.treGoodsKind.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.treGoodsKind.AllowDrop = true;
            this.treGoodsKind.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.treGoodsKind.BackgroundStyle.Class = "TreeBorderKey";
            this.treGoodsKind.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.treGoodsKind.Location = new System.Drawing.Point(1, 12);
            this.treGoodsKind.Name = "treGoodsKind";
            this.treGoodsKind.Nodes.AddRange(new DevComponents.AdvTree.Node[] {
            this.rootNode});
            this.treGoodsKind.NodesConnector = this.nodeConnector1;
            this.treGoodsKind.NodeStyle = this.elementStyle1;
            this.treGoodsKind.PathSeparator = ";";
            this.treGoodsKind.Size = new System.Drawing.Size(324, 327);
            this.treGoodsKind.Styles.Add(this.elementStyle1);
            this.treGoodsKind.TabIndex = 0;
            this.treGoodsKind.Text = "advTree1";
            // 
            // rootNode
            // 
            this.rootNode.Expanded = true;
            this.rootNode.Name = "rootNode";
            this.rootNode.Text = "粮食品种";
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
            // btnOk
            // 
            this.btnOk.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOk.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(174, 345);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "确定";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(255, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 23);
            this.btnCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SelectGoodsKindForm
            // 
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(327, 380);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.treGoodsKind);
            this.EnableGlass = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectGoodsKindForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择粮食品种";
            this.Load += new System.EventHandler(this.SelectGoodsKindForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.treGoodsKind)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree treGoodsKind;
        private DevComponents.AdvTree.Node rootNode;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.DotNetBar.ButtonX btnOk;
        private DevComponents.DotNetBar.ButtonX btnCancel;
    }
}