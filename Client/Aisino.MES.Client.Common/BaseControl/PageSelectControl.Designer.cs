namespace Aisino.MES.Client.Common.BaseControl
{
    partial class PageSelectControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bar1 = new DevComponents.DotNetBar.Bar();
            this.li_info = new DevComponents.DotNetBar.LabelItem();
            this.tbi_curPage = new DevComponents.DotNetBar.TextBoxItem();
            this.li_totalPage = new DevComponents.DotNetBar.LabelItem();
            this.cbi_pageSelect = new DevComponents.DotNetBar.ComboBoxItem();
            this.li_lastPage = new DevComponents.DotNetBar.LabelItem();
            this.li_nextPage = new DevComponents.DotNetBar.LabelItem();
            this.li_prevPage = new DevComponents.DotNetBar.LabelItem();
            this.li_firstPage = new DevComponents.DotNetBar.LabelItem();
            this.li_ok = new DevComponents.DotNetBar.LabelItem();
            this.bti_ok = new DevComponents.DotNetBar.ButtonItem();
            this.bti_firstPage = new DevComponents.DotNetBar.ButtonItem();
            this.bti_prevPage = new DevComponents.DotNetBar.ButtonItem();
            this.bti_nextPage = new DevComponents.DotNetBar.ButtonItem();
            this.bti_lastPage = new DevComponents.DotNetBar.ButtonItem();
            this.labelItem1 = new DevComponents.DotNetBar.LabelItem();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).BeginInit();
            this.SuspendLayout();
            // 
            // bar1
            // 
            this.bar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar1.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.labelItem1,
            this.li_info,
            this.tbi_curPage,
            this.li_totalPage,
            this.li_ok,
            this.bti_ok,
            this.li_firstPage,
            this.bti_firstPage,
            this.li_prevPage,
            this.bti_prevPage,
            this.li_nextPage,
            this.bti_nextPage,
            this.li_lastPage,
            this.bti_lastPage,
            this.cbi_pageSelect});
            this.bar1.Location = new System.Drawing.Point(0, 0);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(1066, 28);
            this.bar1.Stretch = true;
            this.bar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.bar1.TabIndex = 0;
            this.bar1.TabStop = false;
            this.bar1.Text = "bar1";
            // 
            // li_info
            // 
            this.li_info.Enabled = false;
            this.li_info.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_info.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.li_info.Name = "li_info";
            this.li_info.Text = "共500条记录，每页20条，共25页";
            // 
            // tbi_curPage
            // 
            this.tbi_curPage.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.tbi_curPage.Name = "tbi_curPage";
            this.tbi_curPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.tbi_curPage.WatermarkColor = System.Drawing.SystemColors.GrayText;
            // 
            // li_totalPage
            // 
            this.li_totalPage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_totalPage.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.li_totalPage.Name = "li_totalPage";
            this.li_totalPage.Text = "/20页";
            // 
            // cbi_pageSelect
            // 
            this.cbi_pageSelect.DropDownHeight = 106;
            this.cbi_pageSelect.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.cbi_pageSelect.ItemHeight = 17;
            this.cbi_pageSelect.Name = "cbi_pageSelect";
            this.cbi_pageSelect.Visible = false;
            // 
            // li_lastPage
            // 
            this.li_lastPage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_lastPage.Name = "li_lastPage";
            this.li_lastPage.Text = "最后一页";
            this.li_lastPage.Visible = false;
            this.li_lastPage.Click += new System.EventHandler(this.bti_lastPage_Click);
            // 
            // li_nextPage
            // 
            this.li_nextPage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_nextPage.Name = "li_nextPage";
            this.li_nextPage.Text = "下一页";
            this.li_nextPage.Visible = false;
            this.li_nextPage.Click += new System.EventHandler(this.bti_nextPage_Click);
            // 
            // li_prevPage
            // 
            this.li_prevPage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_prevPage.Name = "li_prevPage";
            this.li_prevPage.Text = "上一页";
            this.li_prevPage.Visible = false;
            this.li_prevPage.Click += new System.EventHandler(this.bti_prevPage_Click);
            // 
            // li_firstPage
            // 
            this.li_firstPage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_firstPage.Name = "li_firstPage";
            this.li_firstPage.Text = "第一页";
            this.li_firstPage.Visible = false;
            this.li_firstPage.Click += new System.EventHandler(this.bti_firstPage_Click);
            // 
            // li_ok
            // 
            this.li_ok.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.li_ok.Name = "li_ok";
            this.li_ok.Text = "确定";
            this.li_ok.Visible = false;
            this.li_ok.Click += new System.EventHandler(this.bti_ok_Click);
            // 
            // bti_ok
            // 
            this.bti_ok.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.bti_ok.Image = global::Aisino.MES.Client.Common.Properties.Resources.play2;
            this.bti_ok.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.bti_ok.Name = "bti_ok";
            this.bti_ok.Text = "确定";
            this.bti_ok.Tooltip = "确定";
            this.bti_ok.Click += new System.EventHandler(this.bti_ok_Click);
            // 
            // bti_firstPage
            // 
            this.bti_firstPage.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.bti_firstPage.Image = global::Aisino.MES.Client.Common.Properties.Resources.skipbackward2;
            this.bti_firstPage.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.bti_firstPage.Name = "bti_firstPage";
            this.bti_firstPage.Text = "第一页";
            this.bti_firstPage.Tooltip = "第一页";
            this.bti_firstPage.Click += new System.EventHandler(this.bti_firstPage_Click);
            // 
            // bti_prevPage
            // 
            this.bti_prevPage.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.bti_prevPage.Image = global::Aisino.MES.Client.Common.Properties.Resources.rewind2;
            this.bti_prevPage.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.bti_prevPage.Name = "bti_prevPage";
            this.bti_prevPage.Text = "上一页";
            this.bti_prevPage.Tooltip = "上一页";
            this.bti_prevPage.Click += new System.EventHandler(this.bti_prevPage_Click);
            // 
            // bti_nextPage
            // 
            this.bti_nextPage.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.bti_nextPage.Image = global::Aisino.MES.Client.Common.Properties.Resources.fastforward2;
            this.bti_nextPage.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.bti_nextPage.Name = "bti_nextPage";
            this.bti_nextPage.Text = "下一页";
            this.bti_nextPage.Tooltip = "下一页";
            this.bti_nextPage.Click += new System.EventHandler(this.bti_nextPage_Click);
            // 
            // bti_lastPage
            // 
            this.bti_lastPage.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.bti_lastPage.Image = global::Aisino.MES.Client.Common.Properties.Resources.skipforward2;
            this.bti_lastPage.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.bti_lastPage.Name = "bti_lastPage";
            this.bti_lastPage.SubItemsExpandWidth = 14;
            this.bti_lastPage.Text = "最后一页";
            this.bti_lastPage.Tooltip = "最后一页";
            this.bti_lastPage.Click += new System.EventHandler(this.bti_lastPage_Click);
            // 
            // labelItem1
            // 
            this.labelItem1.Image = global::Aisino.MES.Client.Common.Properties.Resources.bookmark;
            this.labelItem1.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.labelItem1.Name = "labelItem1";
            // 
            // PageSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.bar1);
            this.Name = "PageSelectControl";
            this.Size = new System.Drawing.Size(1066, 29);
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Bar bar1;
        private DevComponents.DotNetBar.TextBoxItem tbi_curPage;
        private DevComponents.DotNetBar.LabelItem li_totalPage;
        private DevComponents.DotNetBar.ButtonItem bti_ok;
        private DevComponents.DotNetBar.ButtonItem bti_firstPage;
        private DevComponents.DotNetBar.ButtonItem bti_prevPage;
        private DevComponents.DotNetBar.ButtonItem bti_nextPage;
        private DevComponents.DotNetBar.ButtonItem bti_lastPage;
        private DevComponents.DotNetBar.ComboBoxItem cbi_pageSelect;
        private DevComponents.DotNetBar.LabelItem li_info;
        private DevComponents.DotNetBar.LabelItem li_ok;
        private DevComponents.DotNetBar.LabelItem li_firstPage;
        private DevComponents.DotNetBar.LabelItem li_prevPage;
        private DevComponents.DotNetBar.LabelItem li_nextPage;
        private DevComponents.DotNetBar.LabelItem li_lastPage;
        private DevComponents.DotNetBar.LabelItem labelItem1;
    }
}
