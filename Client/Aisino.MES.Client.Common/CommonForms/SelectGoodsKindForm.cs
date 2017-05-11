using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Aisino.MES.Service.BaseManager;
using Aisino.MES.Model.Models;
using DevComponents.AdvTree;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class SelectGoodsKindForm : DevComponents.DotNetBar.Office2007Form
    {
        #region 属性
        private GoodsKind _selectedGoodsKind;

        public GoodsKind SelectedGoodsKind
        {
            get { return _selectedGoodsKind; }
            set { _selectedGoodsKind = value; }
        }
        #endregion
        public SelectGoodsKindForm()
        {
            InitializeComponent();
        }

        private void SelectGoodsKindForm_Load(object sender, EventArgs e)
        {
            //初始化树
            initTreeData();
        }

        #region 绑定树节点
        private void initTreeData()
        {
            IBaseManageService baseManageService = ClientHelper.GetServiceInstance<IBaseManageService>("baseManageService");

            try
            {
                IEnumerable<GoodsKind> rootGoodsKind = baseManageService.GetAllRootGoodsKind();
                if (rootGoodsKind != null)
                {
                    foreach (GoodsKind gk in rootGoodsKind)
                    {
                        Node goodsKindNode = new Node();
                        goodsKindNode.Tag = gk;
                        goodsKindNode.Text = gk.goods_kind_name;
                        BindSubGoodsKind(goodsKindNode, gk);

                        treGoodsKind.Nodes[0].Nodes.Add(goodsKindNode);
                    }
                }
            }
            catch (Aisino.MES.Service.AisinoMesServiceException ex)
            {
            }
        }

        private void BindSubGoodsKind(Node parentNode, GoodsKind parentGoodsKind)
        {
            parentNode.Nodes.Clear();

            if (parentGoodsKind.SubGoodsKind != null)
            {
                foreach (GoodsKind gk in parentGoodsKind.SubGoodsKind)
                {
                    Node goodsKindNode = new Node();
                    goodsKindNode.Tag = gk;
                    goodsKindNode.Text = gk.goods_kind_name;
                    BindSubGoodsKind(goodsKindNode, gk);
                    parentNode.Nodes.Add(goodsKindNode);
                }
            }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(treGoodsKind.SelectedNodes.Count > 0)
            {
                if(treGoodsKind.SelectedNodes[0].Text != "粮食品种")
                {
                    SelectedGoodsKind = treGoodsKind.SelectedNodes[0].Tag as GoodsKind;
                }
            }
        }
    }
}