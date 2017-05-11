using System;
using System.Collections.Generic;
using System.Linq;
using Aisino.MES.Model.Models;
using Aisino.MES.Service.ExternalManager;
using DevComponents.AdvTree;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class SelectCustomerForm : DevComponents.DotNetBar.Office2007Form
    {
        #region 属性
        private CustomerProfile selectedCustomer;

        public CustomerProfile SelectedCustomer
        {
            get { return selectedCustomer; }
            set { selectedCustomer = value; }
        }
        #endregion

        public SelectCustomerForm()
        {
            InitializeComponent();
        }

        private void SelectCustomerForm_Load(object sender, EventArgs e)
        {
            CustomerBind();
        }

        private void CustomerBind()
        {
            ICustomerService myCustomerServiceInstance = ClientHelper.GetServiceInstance<ICustomerService>("customerService");
            try
            {
                advcustomer.Nodes.Clear();
                List<CustomerProfile> customers = myCustomerServiceInstance.GetAllCustomer().ToList();
                if (customers.Count>0)
                {
                    advcustomer.Nodes.Clear();
                    for (int i = 0; i < customers.Count; i++)
                    {
                        Node masterCustomerNode = new Node();
                        masterCustomerNode.Tag = customers[i];
                        masterCustomerNode.Text = customers[i].name;
                        masterCustomerNode.Cells.Add(new Cell(customers[i].customer_group.ToString()));
                        advcustomer.Nodes.Add(masterCustomerNode);
                    }
                }
                else
                {
                    advcustomer.Nodes.Clear();
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
            if (advcustomer.SelectedNodes.Count == 0)
            {
                AisinoMessageBox amb2 = new AisinoMessageBox("提示", "没有选择节点！",
                                                                AisinoMessageButton.OK,
                                                                AisinoMessageIcon.Warning);
                amb2.ShowDialog();
            }
            else
            {
                this.SelectedCustomer = advcustomer.SelectedNode.Tag as CustomerProfile;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ICustomerService myCustomerServiceInstance = ClientHelper.GetServiceInstance<ICustomerService>("customerService");
            advcustomer.Nodes.Clear();
            List<CustomerProfile> customers = myCustomerServiceInstance.FindCustomerByCondition(txtName.Text).ToList();
            if (customers.Count > 0)
            {
                advcustomer.Nodes.Clear();
                for (int i = 0; i < customers.Count; i++)
                {
                    Node masterCustomerNode = new Node();
                    masterCustomerNode.Tag = customers[i];
                    masterCustomerNode.Text = customers[i].name;
                    masterCustomerNode.Cells.Add(new Cell(customers[i].customer_group.ToString()));
                    advcustomer.Nodes.Add(masterCustomerNode);
                }
            }
            else
            {
                advcustomer.Nodes.Clear();
            }

        }
    }
}