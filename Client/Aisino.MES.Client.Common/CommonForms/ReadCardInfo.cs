using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Aisino.MES.Service.EnrolmentManager;
using Aisino.MES.Service.ManuManager;
using Aisino.MES.Rfid;
using Aisino.MES.Model.Models;
using Aisino.MES.Model.NewModels;
using Aisino.MES.Service.BaseManager;
using Aisino.MES.Rfid.Data;

namespace Aisino.MES.Client.Common.CommonForms
{
    public partial class ReadCardInfo : DevComponents.DotNetBar.Office2007Form
    {
        private bool HasReadInfo;
        private string LastCardID;
        public ReadCardInfo()
        {
            InitializeComponent();
        }

        private void ReadCardInfo_Load(object sender, EventArgs e)
        {
            dgvAssay.AutoGenerateColumns = false;
            dgvPlan.AutoGenerateColumns = false;
            HasReadInfo = false;
        }

        private void tm_Read_Tick(object sender, EventArgs e)
        {
            //�Ȼ�ȡ��Ϣ��
            //_batchNum = txtPlanBatchNumber.Text = "11EPI20130926001001";
            //��ȡ������ǩ����==================================
            IEnrolmentService myServiceInstance = ClientHelper.GetServiceInstance<IEnrolmentService>("enrolmentService");
            IPlanTaskBatchService myPlanTaskBatchInstance = ClientHelper.GetServiceInstance<IPlanTaskBatchService>("planTaskBatchService");
            if (DeviceHelper.DeskReader.Used == 0)
            {
                return;
            }
            AisinoJK116Reader read = new AisinoJK116Reader();
            read.ComPort = Convert.ToInt16(DeviceHelper.DeskReader.Port);
            read.BaudRate = Convert.ToInt16(DeviceHelper.DeskReader.Rate);
            read.CardType = Enum.Parse(typeof(EnrolmentRFIDReader.CardType), DeviceHelper.DeskReader.CardType).ToString();
            IRFIDTagIssueService myservice = ClientHelper.GetServiceInstance<IRFIDTagIssueService>("rfidTagIssueService");
            EnrolmentInfo newEnrolmentInfo = new EnrolmentInfo();
            string MessageString = string.Empty;
            string cardID = read.ReadTagMainid();
            if (cardID == string.Empty)
            {
                return;
            }
            if (cardID == LastCardID)
            {
                return;
            }
            else
            {
                LastCardID = cardID;
            }
            ShowMessageInfo(myservice, cardID);

        }

        private void ShowMessageInfo(IRFIDTagIssueService myservice, string cardID)
        {
            //��ȡ����Ϣ�������ȡ�������˻�            
            Enrolment tempEnrolment = myservice.FindRunnigRFIDTagIssueByCode(cardID);
            PlanTaskBatch ptb = tempEnrolment.PlanTasks.Last().PlanTaskBatches.Last();

            //��ȡ������Ϣ�����ؿ���Ϣ            
            if (tempEnrolment != null)
            {
                //������Ϣ
                ClearAll();
                //�����
                if (tempEnrolment.RFIDTagIssues.Count > 0)
                {
                    lblCardNumber.Text = tempEnrolment.RFIDTagIssues.Last().RFIDTag.tag_number;
                }
                //����
                if (tempEnrolment.owner_name != null && tempEnrolment.owner_name != string.Empty)
                {
                    lblOwnerName.Text = tempEnrolment.owner_name;
                }
                else if (tempEnrolment.carrier_name != null && tempEnrolment.carrier_name != string.Empty)
                {
                    lblOwnerName.Text = tempEnrolment.carrier_name;
                }
                //������
                lblCarNumber.Text = tempEnrolment.plate_number;
                //����
                lblChanDi.Text = tempEnrolment.districtName;
                //Ʒ��
                lblGoodsKind.Text = tempEnrolment.goodsKindName;
                //�ͻ�
                lblKeHu.Text = tempEnrolment.customerName;
                //�ƻ����

                if (ptb != null)
                {
                    lblPlantaskNumber.Text = ptb.plantask_number;
                    //���α��
                    lblPTBatchNumber.Text = ptb.plantask_batch_number;
                }
                //assay
                CardInfoAssay NewCardInfoAssay = new CardInfoAssay();                
                if (ptb.Samples != null && ptb.Samples.Count > 0)
                {
                    Assay tempAssay = ptb.Samples.First().Assays.First();
                    NewCardInfoAssay.AssayNumber = tempAssay.assay_number;
                    IList<AssayResult> theAssayResultList = tempAssay.AssayBills.First().AssayResults.ToList();
                    foreach (AssayResult tempAR in theAssayResultList)
                    {
                        if (tempAR.GrainQualityIndex.name.Contains("ˮ"))
                        {
                            NewCardInfoAssay.ShuiFen = tempAR.assay_result_value;
                        }
                        if (tempAR.GrainQualityIndex.name.Contains("����"))
                        {
                            NewCardInfoAssay.ZaZhi = tempAR.assay_result_value;
                        }
                        if (tempAR.GrainQualityIndex.name.Contains("����"))
                        {
                            NewCardInfoAssay.RongZhong = tempAR.assay_result_value;
                        }
                        if (tempAR.GrainQualityIndex.name.Contains("������"))
                        {
                            NewCardInfoAssay.BuWanShanLi = tempAR.assay_result_value;
                        }
                        if (tempAR.GrainQualityIndex.name.Contains("����"))
                        {
                            NewCardInfoAssay.ChuCaolv = tempAR.assay_result_value;
                        }
                        if (tempAR.GrainQualityIndex.name.Contains("������"))
                        {
                            NewCardInfoAssay.ZhengJingMiLi = tempAR.assay_result_value;
                        }
                    }
                }

                //PLAN
                CardInfoPlan NewCardInfoPlan = new CardInfoPlan();
                decimal JingZhong = 0;
                NewCardInfoPlan.BangDian = ptb.PlanTaskBatchSiteScales.FirstOrDefault() == null ? "��" : ptb.PlanTaskBatchSiteScales.FirstOrDefault().SiteScale.scale_name;
                if (ptb.gross_weight.HasValue)
                {
                    NewCardInfoPlan.GrossWeight = ptb.gross_weight.Value.ToString("#");
                }
                else
                {
                    NewCardInfoPlan.GrossWeight = string.Empty;
                }
                if (ptb.tare_weight.HasValue)
                {
                    NewCardInfoPlan.TareWeight = ptb.tare_weight.Value.ToString("#");
                }
                else
                {
                    NewCardInfoPlan.TareWeight = string.Empty;
                }
                if (ptb.batch_count.HasValue)
                {
                    JingZhong = ptb.batch_count.Value;
                }
                decimal kouLiang = 0;
                if (ptb.BusinessDisposes != null && ptb.BusinessDisposes.Count > 0)
                {
                    BusinessDispose tempBusinessDispose = ptb.BusinessDisposes.FirstOrDefault();
                    if (tempBusinessDispose.BusinessDisposeDetails != null && tempBusinessDispose.BusinessDisposeDetails.Count > 0)
                    {
                        kouLiang = tempBusinessDispose.BusinessDisposeDetails.Sum(bdd => bdd.dispose_count) * JingZhong;
                        NewCardInfoPlan.KouLiang = kouLiang.ToString("0.00");
                    }
                }
                NewCardInfoPlan.JieSuanShuLiang = (JingZhong - kouLiang).ToString("0.00");
                List<CardInfoAssay> assayList = new List<CardInfoAssay>();
                assayList.Add(NewCardInfoAssay);
                dgvAssay.DataSource = assayList;
                List<CardInfoPlan> planList = new List<CardInfoPlan>();
                planList.Add(NewCardInfoPlan);
                dgvPlan.DataSource = planList;
            }
        }

        private void ClearAll()
        {
            lblCardNumber.Text = string.Empty;
            lblCarNumber.Text = string.Empty;
            lblChanDi.Text = string.Empty;
            lblGoodsKind.Text = string.Empty;
            lblKeHu.Text = string.Empty;
            lblOwnerName.Text = string.Empty;
            lblPlantaskNumber.Text = string.Empty;
            lblPTBatchNumber.Text = string.Empty;
            dgvAssay.Rows.Clear();
            dgvPlan.Rows.Clear();
        }
    }
}