using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aisino.MES.Client.Common.BaseControl
{
    public partial class PageSelectControl : UserControl
    {
        public delegate void PageChangedDelegate(object sender, EventArgs e);

        [Description("当前页改变时发生的事件"), Category("分页设置")]
        public event PageChangedDelegate PageChanged;

        /// <summary> 
        /// 默认构造函数，设置分页初始信息
        /// </summary>
        public PageSelectControl()
        {
            InitializeComponent();

            this.pageSize = 50;
            this.recordCount = 0;
            this.currentPage = 1; //默认为第一页
            this.InitData();


        }

        /// <summary> 
        /// 带参数的构造函数
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// </summary>
        public PageSelectControl(int recordCount, int pageSize)
        {
            InitializeComponent();

            this.pageSize = pageSize;
            this.recordCount = recordCount;
            this.currentPage = 1; //默认为第一页
            this.InitData();
        }

        protected virtual void OnPageChanged(EventArgs e)
        {
            if (PageChanged != null)
            {
                PageChanged(this, e);
            }
        }

        int currentPage = 1;//当前页 
        /// <summary>
        /// 当前页 
        /// </summary>
        [Description("当前页"), Category("分页设置")]
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }
        int pageSize = 10;//每页显示条数
        /// <summary>
        /// 每页显示条数
        /// </summary>
        [Description("每页显示条数"), Category("分页设置")]
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        int pageTotal = 0;//总共多少页 
        /// <summary>
        /// 总共多少页 
        /// </summary>
        [Description("总共多少页"), Category("分页设置")]
        public int PageTotal
        {
            get { return pageTotal; }
            set { pageTotal = value; }
        }
        /// <summary>
        /// 总的记录数
        /// </summary>
        private int recordCount;//总的记录数
        [Description("总的记录数"), Category("分页设置")]
        public int RecordCount
        {
            get { return recordCount; }
            set
            {
                recordCount = value;
                //InitData();// 初始化数据
                //PageChanged();//当前页改变事件
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            if (this.pageSize < 1)
                this.pageSize = 10; //如果每页记录数不正确，即更改为10
            if (this.recordCount < 0)
                this.recordCount = 0; //如果记录总数不正确，即更改为0

            //取得总页数
            if (this.recordCount % this.pageSize == 0)
            {
                this.pageTotal = this.recordCount / this.pageSize;
            }
            else
            {
                this.pageTotal = this.recordCount / this.pageSize + 1;
            }

            //设置当前页
            if (this.currentPage > this.pageTotal)
            {
                this.currentPage = this.pageTotal;
            }
            if (this.currentPage < 1)
            {
                this.currentPage = 1;
            }

            //设置按钮的可用性
            bool enable = (this.currentPage > 1);
            this.bti_prevPage.Enabled = enable;
            this.bti_firstPage.Enabled = enable;
            this.li_prevPage.Enabled = enable;
            this.li_firstPage.Enabled = enable;

            enable = (this.currentPage < this.pageTotal);
            this.bti_nextPage.Enabled = enable;
            this.bti_lastPage.Enabled = enable;
            this.li_nextPage.Enabled = enable;
            this.li_lastPage.Enabled = enable;

            this.tbi_curPage.Text = this.currentPage.ToString();

            li_info.Text = String.Format("共{0}条记录，每页{1}条，共{2}页", recordCount.ToString(), PageSize.ToString(), PageTotal.ToString());
            li_totalPage.Text = String.Format("/{0}页", PageTotal.ToString());
        }

        public void RefreshData(int page)
        {
            this.currentPage = page;
            EventArgs e = new EventArgs();
            OnPageChanged(e);
        }

        private void txtCurrentPage_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int num;
                try
                {
                    num = Convert.ToInt16(this.tbi_curPage.Text);
                }
                catch (Exception ex)
                {
                    num = 1;
                }

                if (num > this.pageTotal)
                    num = this.pageTotal;
                if (num < 1)
                    num = 1;

                this.RefreshData(num);
            }
        }

        private void bti_prevPage_Click(object sender, EventArgs e)
        {
            if (this.currentPage > 1)
            {
                this.RefreshData(this.currentPage - 1);
            }
            else
            {
                this.RefreshData(1);
            }
        }

        private void bti_firstPage_Click(object sender, EventArgs e)
        {
            this.RefreshData(1);
        }

        private void bti_nextPage_Click(object sender, EventArgs e)
        {
            if (this.currentPage < this.pageTotal)
            {
                this.RefreshData(this.currentPage + 1);
            }
            else if (this.pageTotal < 1)
            {
                this.RefreshData(1);
            }
            else
            {
                this.RefreshData(this.pageTotal);
            }
        }

        private void bti_lastPage_Click(object sender, EventArgs e)
        {
            if (this.pageTotal > 0)
            {
                this.RefreshData(this.pageTotal);
            }
            else
            {
                this.RefreshData(1);
            }
        }

        private void bti_ok_Click(object sender, EventArgs e)
        {
            int num;
            try
            {
                num = Convert.ToInt16(this.tbi_curPage.Text);
            }
            catch (Exception ex)
            {
                num = 1;
            }

            if (num > this.pageTotal)
                num = this.pageTotal;
            if (num < 1)
                num = 1;

            this.RefreshData(num);
        }
    }
}
