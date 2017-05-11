using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Aisino.MES.Service.ManuManager;
using DevComponents.AdvTree;
using Aisino.MES.Model.Models;
using System.Linq;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class SelectBorLineForm : DevComponents.DotNetBar.Office2007Form
    {
        #region 属性
        private BorLine selectedBorLine;

        public BorLine SelectedBorLine
        {
            get { return selectedBorLine; }
            set { selectedBorLine = value; }
        }
        #endregion
        public SelectBorLineForm()
        {
            InitializeComponent();
        }

        private void SelectBorLineForm_Load(object sender, EventArgs e)
        {
            BorLineBind();
        }

        private void BorLineBind()
        {
            IBorLineService myBorLineInstance = ClientHelper.GetServiceInstance<IBorLineService>("borLineService");

            try
            {
                advBorLine.Nodes.Clear();
                List<BorLine> borline = myBorLineInstance.SelectAllBorLine().ToList();
                if (borline.Count > 0)
                {
                    advBorLine.Nodes.Clear();
                    for (int i = 0; i < borline.Count; i++)
                    {
                        Node masterContractNode = new Node();
                        masterContractNode.Tag = borline[i];
                        masterContractNode.Text = borline[i].bor_line_name;
                        masterContractNode.Cells.Add(new Cell(borline[i].bor_line_code));
                        advBorLine.Nodes.Add(masterContractNode);
                    }
                }
                else
                {
                    advBorLine.Nodes.Clear();
                }
            }
            catch (Aisino.MES.Service.AisinoMesServiceException ex)
            {
                AisinoMessageBox amb = new AisinoMessageBox("错误",
                                                ex.Message,
                                                ex.InnerException.ToString(),
                                                "请联系系统管理员",
                                                  AisinoMessageButton.OK,
                                                  AisinoMessageIcon.Error);
                amb.ShowDialog();
            }
            catch (Exception ex)
            {
                AisinoMessageBox amb = new AisinoMessageBox("错误",
                                                ex.Message,
                                                ex.InnerException.ToString(),
                                                "请联系系统管理员",
                                                  AisinoMessageButton.OK,
                                                  AisinoMessageIcon.Error);
                amb.ShowDialog();
            }
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            if (advBorLine.SelectedNodes.Count == 0)
            {
                AisinoMessageBox amb2 = new AisinoMessageBox("提示", "没有选择节点！",
                                                                AisinoMessageButton.OK,
                                                                AisinoMessageIcon.Warning);
                amb2.ShowDialog();
            }
            else
            {
                this.SelectedBorLine = advBorLine.SelectedNode.Tag as BorLine;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}