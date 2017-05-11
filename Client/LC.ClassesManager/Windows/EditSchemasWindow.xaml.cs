using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.Windows;
using Aisino.MES.Resources;
using LC.Contracts.ClassManager;
using LC.Model;
using LC.Model.Business.ClassModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WcfClientProxyGenerator.Async;

namespace LC.ClassesManager.Windows
{
    /// <summary>
    /// EditSchemasWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditSchemasWindow : BaseWindow
    {
        private OperationMode _om;

        public OperationMode Om
        {
            get { return _om; }
            set { _om = value; }
        }

        private int schemasId;

        public int SchemasId
        {
            get { return schemasId; }
            set { schemasId = value; }
        }


        private int classTypeId;

        public int ClassTypeId
        {
            get { return classTypeId; }
            set { classTypeId = value; }
        }

        IAsyncProxy<IClassTypeService> classTypeAsyncProxy = null;

        public EditSchemasWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            SchemasEditModel schemasEditModel = new SchemasEditModel();
            try
            {
                classTypeAsyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());

                int seq = await classTypeAsyncProxy.CallAsync(c => c.GetMaxSeqByClassType(this.classTypeId));

                this.numSeq.Value = seq + 1;
            }
            catch (TimeoutException timeProblem)
            {
                strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            }
            catch (FaultException<LCFault> af)
            {
                strErrorMsg = af.Detail.Message;
            }
            catch (FaultException unknownFault)
            {
                strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
            }
            catch (CommunicationException commProblem)
            {
                strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            }
            catch (Exception ex)
            {
                strErrorMsg = ex.Message;
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "初始化界面失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                return;
            }

            if (Om == OperationMode.AddMode)
            {
                this.Title = "AddSchemas";
                //txtName.IsEnabled = true;
                //BindClassInfo();
            }
            else
            {
                this.Title = "EditSchemas";
                //txtName.IsEnabled = false;
                //BindClassInfo();
            }
        }


        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            #region 新增
            if (Om == OperationMode.AddMode)
            {
                string strErrorMsg = string.Empty;
                try
                {
                    SchemasEditModel newSchemasModel = new SchemasEditModel();
                    newSchemasModel.LessonName = txtLesson.Text.Trim();
                    newSchemasModel.LevelName = txtLevel.Text.Trim();
                    newSchemasModel.Seq = Convert.ToInt16(numSeq.Value);
                    newSchemasModel.TypeId = this.classTypeId;

                    newSchemasModel = await classTypeAsyncProxy.CallAsync(c => c.AddSchemas(newSchemasModel));
                    this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "Add Schemas Sucess！");
                    this.DialogResult = true;
                }
                catch (TimeoutException timeProblem)
                {
                    strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
                }
                catch (FaultException<LCFault> af)
                {
                    strErrorMsg = af.Detail.Message;
                }
                catch (FaultException unknownFault)
                {
                    strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
                }
                catch (CommunicationException commProblem)
                {
                    strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
                }
                if (strErrorMsg != string.Empty)
                {
                    await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
                }
            }

            #endregion

            #region 修改
            //else
            //{
            //    string strErrorMsg = string.Empty;
            //    try
            //    {
            //        classEditModel.Name = txtName.Text.Trim();
            //        classEditModel.TypeId = (cmbClassType.SelectedItem as ClassTypeModel).Id;
            //        classEditModel.LastCount = Convert.ToInt16(numLastCount.Value);
            //        classEditModel.StartDate = dateStart.DisplayDate;
            //        classEditModel.EndDate = dateEnd.DisplayDate;

            //        classEditModel = await classesAsyncProxy.CallAsync(c => c.Update(classEditModel));

            //        this.ShowAutoCloseDialogOwter(UIResources.MsgInfo, "修改班级成功！");
            //        this.DialogResult = true;
            //    }
            //    catch (TimeoutException timeProblem)
            //    {
            //        strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            //    }
            //    catch (FaultException<LCFault> af)
            //    {
            //        strErrorMsg = af.Detail.Message;
            //    }
            //    catch (FaultException unknownFault)
            //    {
            //        strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
            //    }
            //    catch (CommunicationException commProblem)
            //    {
            //        strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            //    }
            //    if (strErrorMsg != string.Empty)
            //    {
            //        await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "修改班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            //    }
            //}
            #endregion
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalObjects.blOpenCancelWarning == true)
            {
                MessageDialogResult result = await DialogManager.ShowMessageAsync(this, UIResources.MsgInfo, "是否确认取消操作？", MessageDialogStyle.AffirmativeAndNegative, null);
                if (result == MessageDialogResult.Affirmative)
                {
                    this.DialogResult = false;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private async void txtLevel_LostFocus(object sender, RoutedEventArgs e)
        {
            string strErrorMsg = string.Empty;
            try
            {
                
            }
            catch (TimeoutException timeProblem)
            {
                strErrorMsg = timeProblem.Message + UIResources.TimeOut + timeProblem.Message;
            }
            catch (FaultException<LCFault> af)
            {
                strErrorMsg = af.Detail.Message;
            }
            catch (FaultException unknownFault)
            {
                strErrorMsg = UIResources.UnKnowFault + unknownFault.Message;
            }
            catch (CommunicationException commProblem)
            {
                strErrorMsg = UIResources.ConProblem + commProblem.Message + commProblem.StackTrace;
            }
            if (strErrorMsg != string.Empty)
            {
                await DialogManager.ShowMessageAsync(this, UIResources.MsgError, "新增班级失败！原因：" + strErrorMsg, MessageDialogStyle.Affirmative, null);
            }
        }
    }
}
