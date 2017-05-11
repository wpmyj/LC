using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Aisino.MES.Service.BaseManager;
using Aisino.MES.Client.Common;
using Aisino.MES.Model.Models;
using System.Linq;
using DevComponents.AdvTree;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class DistrictSelectForm : DevComponents.DotNetBar.Office2007Form
    {
        #region 属性
        private District selectedDistrict;

        public District SelectedDistrict
        {
            get { return selectedDistrict; }
            set { selectedDistrict = value; }
        }
        #endregion

        public DistrictSelectForm()
        {
            InitializeComponent();
        }

        private void DistrictSelectForm_Load(object sender, EventArgs e)
        {
            DistrictBound();
        }

        private void DistrictBound()
        {
            IBaseManagerOtherService myServiceInstance = ClientHelper.GetServiceInstance<IBaseManagerOtherService>("baseManagerOtherService");

            try
            {
                advtreedistic.Nodes.Clear();
                District districts = myServiceInstance.GetRootDistrict();
                if (districts != null)
                {
                    advtreedistic.Nodes.Clear();
                    Node masterDistrictNode = new Node();
                    masterDistrictNode.Tag = districts;
                    masterDistrictNode.Text = districts.code;
                    masterDistrictNode.Cells.Add(new Cell(districts.name));
                    masterDistrictNode.Cells.Add(new Cell(districts.remark));
                    BindSubDistrictNode(masterDistrictNode, districts);
                    advtreedistic.Nodes.Add(masterDistrictNode);
                }
                else
                {
                    advtreedistic.Nodes.Clear();
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

        private void BindSubDistrictNode(Node masterDistrictNode, District districts)
        {
            advtreedistic.Nodes.Clear();
            if (districts.SubDistrict != null && districts.SubDistrict.Count > 0)
            {
                //设置列头
                SetDetailHead(masterDistrictNode);
                foreach (District dis in districts.SubDistrict)
                {
                    Node childdisNode = new Node();
                    childdisNode.Tag = dis;
                    childdisNode.Text = dis.code;
                    childdisNode.Cells.Add(new Cell(dis.name));
                    childdisNode.Cells.Add(new Cell(dis.remark));
                    BindSubDistrictNode(childdisNode, dis);
                    masterDistrictNode.Nodes.Add(childdisNode);
                }
            }
        }

        private void SetDetailHead(Node districtNode)
        {
            DevComponents.AdvTree.ColumnHeader clmHeadSub1 = new DevComponents.AdvTree.ColumnHeader();
            clmHeadSub1.Name = "cdcode";
            clmHeadSub1.Text = "产地代码";
            clmHeadSub1.Width.Absolute = 150;

            DevComponents.AdvTree.ColumnHeader clmHeadSub2 = new DevComponents.AdvTree.ColumnHeader();
            clmHeadSub2.Name = "cdname";
            clmHeadSub2.Text = "产地名称";
            clmHeadSub2.Width.Absolute = 150;

            DevComponents.AdvTree.ColumnHeader clmHeadSub3 = new DevComponents.AdvTree.ColumnHeader();
            clmHeadSub3.Name = "cdremark";
            clmHeadSub3.Text = "备注";
            clmHeadSub3.Width.Absolute = 200;

            districtNode.NodesColumns.Add(clmHeadSub1);
            districtNode.NodesColumns.Add(clmHeadSub2);
            districtNode.NodesColumns.Add(clmHeadSub3);
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            if (advtreedistic.SelectedNodes.Count == 0)
            {
                AisinoMessageBox amb2 = new AisinoMessageBox("提示", "没有选择节点！",
                                                                 AisinoMessageButton.OK,
                                                                 AisinoMessageIcon.Warning);
                amb2.ShowDialog();
            }
            else
            {
                this.SelectedDistrict = advtreedistic.SelectedNode.Tag as District;

            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}