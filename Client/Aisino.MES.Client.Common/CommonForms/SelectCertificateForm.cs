using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Aisino.MES.Model.Models;
using Aisino.MES.Service.ExternalManager;
using System.Linq;
using DevComponents.AdvTree;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class SelectCertificateForm : DevComponents.DotNetBar.Office2007Form
    {
        #region 属性
        private CustomerShipingCertificate selectedCertificate;

        public CustomerShipingCertificate SelectedCertificate
        {
            get { return selectedCertificate; }
            set { selectedCertificate = value; }
        }

        private GoodsKind goodskind;

        public GoodsKind Goodskind
        {
            get { return goodskind; }
            set { goodskind = value; }
        }

        private CustomerProfile customer;

        public CustomerProfile Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        private int inoutType;

        public int InoutType
        {
            get { return inoutType; }
            set { inoutType = value; }
        }

        public string Contractnum;
        
        #endregion

        public SelectCertificateForm()
        {
            InitializeComponent();
        }

        private void SelectCertificateForm_Load(object sender, EventArgs e)
        {
            CertificateBind();
        }

        private void CertificateBind()
        {
            ICertificateService myCertificateServiceInstance = ClientHelper.GetServiceInstance<ICertificateService>("CertificateService");
            List<CustomerShipingCertificate> certificate = new List<CustomerShipingCertificate>();
            try
            {
                advcertificate.Nodes.Clear();
                if (Contractnum == null)
                {
                    certificate = myCertificateServiceInstance.FindCertificateByGoodsKindAndCustomer(this.goodskind, this.Customer.customer_id, this.inoutType).ToList();
                }
                else
                {
                    List<CustomerShipingCertificate> customercertificates = myCertificateServiceInstance.GetAllCertificate().ToList().Where(d => d.contract == Contractnum && d.bill_status == (int)Aisino.MES.Model.Models.CustomerShipingCertificate.CertificateStatus.审批通过).ToList();
                    foreach (CustomerShipingCertificate customercertificate in customercertificates)
                    {
                        if (customercertificate.excepted_count > customercertificate.finished_count)
                        {
                            certificate.Add(customercertificate);
                        }
                    }
                }
                if (certificate.Count > 0)
                {
                    advcertificate.Nodes.Clear();
                    for (int i = 0; i < certificate.Count; i++)
                    {
                        Node masterContractNode = new Node();
                        masterContractNode.Tag = certificate[i];
                        masterContractNode.Text = certificate[i].bill_id;
                        masterContractNode.Cells.Add(new Cell(certificate[i].bill_memo));
                        //masterContractNode.Cells.Add(new Cell(certificate[i].excepted_count.ToString()));
                        masterContractNode.Cells.Add(new Cell(certificate[i].finished_count.ToString()));
                        masterContractNode.Cells.Add(new Cell(certificate[i].shiping_count.ToString()));
                        advcertificate.Nodes.Add(masterContractNode);
                    }
                }
                else
                {
                    advcertificate.Nodes.Clear();
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
            if (advcertificate.Nodes.Count > 0)
            {
                if (advcertificate.SelectedNodes.Count == 0)
                {
                    AisinoMessageBox amb2 = new AisinoMessageBox("提示", "没有选择节点！",
                                                                    AisinoMessageButton.OK,
                                                                    AisinoMessageIcon.Warning);
                    amb2.ShowDialog();
                }
                else
                {
                    this.SelectedCertificate = advcertificate.SelectedNode.Tag as CustomerShipingCertificate;
                }
            }
            else
            {
                this.SelectedCertificate = null;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}