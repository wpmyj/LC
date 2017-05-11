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
    public partial class SelectContractForm : DevComponents.DotNetBar.Office2007Form
    {
        #region ����
        private Contract selectedContract;

        public Contract SelectedContract
        {
            get { return selectedContract; }
            set { selectedContract = value; }
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

        private CustomerShipingCertificate certificate;

        public CustomerShipingCertificate Certificate
        {
            get { return certificate; }
            set { certificate = value; }
        }

        private List<Contract> _theContractList;

        public List<Contract> TheContractList
        {
            get { return _theContractList; }
            set { _theContractList = value; }
        }

        #endregion
        public SelectContractForm()
        {
            InitializeComponent();
        }

        private void SelectContractForm_Load(object sender, EventArgs e)
        {
            ContractBind();
        }

        private void ContractBind()
        {
            IContractService myContractServiceInstance = ClientHelper.GetServiceInstance<IContractService>("contractService");
            try
            {
                advcontract.Nodes.Clear();
                if (_theContractList == null)
                {
                    _theContractList = myContractServiceInstance.FindContractByGoodsKindAndCustomerAndInoutType(this.goodskind, this.Customer.customer_id, this.inoutType).ToList();
                }

                if (_theContractList.Count > 0)
                {
                    advcontract.Nodes.Clear();
                    for (int i = 0; i < _theContractList.Count; i++)
                    {
                        Node masterContractNode = new Node();
                        masterContractNode.Tag = _theContractList[i];
                        masterContractNode.Text = _theContractList[i].contract_number;
                        masterContractNode.Cells.Add(new Cell(_theContractList[i].contract_title));
                        advcontract.Nodes.Add(masterContractNode);
                    }
                }
                else
                {
                    advcontract.Nodes.Clear();
                }
            }
            catch (Aisino.MES.Service.AisinoMesServiceException ex)
            {
                AisinoMessageBox amb = new AisinoMessageBox("����",
                                                ex.Message,
                                                ex.InnerException.ToString(),
                                                "����ϵϵͳ����Ա",
                                                  AisinoMessageButton.OK,
                                                  AisinoMessageIcon.Error);
                amb.ShowDialog();
            }
            catch (Exception ex)
            {
                AisinoMessageBox amb = new AisinoMessageBox("����",
                                                ex.Message,
                                                ex.InnerException.ToString(),
                                                "����ϵϵͳ����Ա",
                                                  AisinoMessageButton.OK,
                                                  AisinoMessageIcon.Error);
                amb.ShowDialog();
            }
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            if (advcontract.SelectedNodes.Count == 0)
            {
                AisinoMessageBox amb2 = new AisinoMessageBox("��ʾ", "û��ѡ��ڵ㣡",
                                                                AisinoMessageButton.OK,
                                                                AisinoMessageIcon.Warning);
                amb2.ShowDialog();
            }
            else
            {
                this.SelectedContract = advcontract.SelectedNode.Tag as Contract;
                if (selectedContract.CustomerShipingCertificates.Count != 0)
                {
                    AisinoMessageBox amb3 = new AisinoMessageBox("��ʾ", "�ú�ͬ�Ŵ���ƾ֤,�Ƿ����",
                                                                        AisinoMessageButton.YesNo,
                                                                        AisinoMessageIcon.Warning);
                    if (amb3.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (selectedContract.CustomerShipingCertificates.Count == 1)
                        {
                            certificate = selectedContract.CustomerShipingCertificates.ToList()[0];
                        }
                        else
                        {
                            using (SelectCertificateForm certificateForm = new SelectCertificateForm())
                            {
                                certificateForm.Contractnum = selectedContract.contract_number;
                                if (certificateForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                                {
                                    certificate = certificateForm.SelectedCertificate;
                                }
                            }
                        }
                    }

                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}