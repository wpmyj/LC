using Aisino.MES.Client.Common;
using Aisino.MES.Client.WPFCommon.BasePages;
using LC.Contracts.ClassManager;
using LC.Contracts.StudentManager;
using LC.Model.Business.ClassModel;
using LC.Model.Business.StudentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Data;
using WcfClientProxyGenerator.Async;

namespace LC.ClassesManager.Pages
{
    /// <summary>
    /// ClassStudentsManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClassStudentsManagerPage : BusinessBasePage
    {
        private List<int> classStudentIds;
        public ClassStudentsManagerPage()
        {
            InitializeComponent();
        }

        private void BusinessBasePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth > 100)
            {
                dockPanel.Width = this.ActualWidth;
                dockPanel.Height = this.ActualHeight;
            }
        }

        private async void BusinessBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            classStudentIds = new List<int>();
            await bindClassTypeList();
            await bindStudentList();
        }

        private async Task bindClassTypeList()
        {
            IAsyncProxy<IClassTypeService> _classtypeAyncProxy = await Task.Run(() => ServiceHelper.GetClassTypeService());
            IList<ClassTypeModel> RM = await _classtypeAyncProxy.CallAsync(c => c.GetAllClassTypeWithClasses());
            this.ClassTypeTree.ItemsSource = RM;
        }

        private async Task bindStudentList()
        {
            IAsyncProxy<IStudentService> studentAyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());
            IList<StudentDisplayModel> RM = await studentAyncProxy.CallAsync(c => c.GetAll());
            this.gvStudent.ItemsSource = RM;
        }

        private async Task bindStudentListByClass(int classid)
        {
            this.classStudentIds.Clear();
            IAsyncProxy<IStudentService> studentAyncProxy = await Task.Run(() => ServiceHelper.GetStudentService());
            IList<StudentDisplayModel> RM = await studentAyncProxy.CallAsync(c => c.FindStudentsByClassId(classid));
            this.gvStudent.SelectedItems.Clear();
            //在列表中选中当前班级学生
            foreach(StudentDisplayModel sdm in RM)
            {
                //当前选中班级的所有学生加入该区间保存，用于后期处理
                this.classStudentIds.Add(sdm.Id);
                foreach(var item in this.gvStudent.Items)
                {
                    if(sdm.Id == (item as StudentDisplayModel).Id)
                    {
                        this.gvStudent.SelectedItems.Add(item);
                        break;
                    }
                }
            }
            
        }

        //private async void ckShowAllStudents_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (ckShowAllStudents.IsChecked == true)
        //    {
        //        this.gvStudent.ItemsSource = null;
        //        this.gvStudent.Items.Clear();
        //        await bindStudentList();
        //    }
        //    else
        //    {
        //        this.gvStudent.ItemsSource = null;
        //        this.gvStudent.Items.Clear();
        //    }
        //}

        private async void ClassTypeTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClassDisplayModel cdm = this.ClassTypeTree.SelectedItem as ClassDisplayModel;
            if(cdm != null)
            {
                await bindStudentListByClass(cdm.Id);
            }            
        }

        private async void btnUpdateMenu_Click(object sender, RoutedEventArgs e)
        {
            ClassDisplayModel cdm = this.ClassTypeTree.SelectedItem as ClassDisplayModel;
            List<int> studentIds = new List<int>();
            foreach(var item in this.gvStudent.SelectedItems)
            {
                studentIds.Add((item as StudentDisplayModel).Id);
            }
            IAsyncProxy<IClassesService> _classAyncProxy = await Task.Run(() => ServiceHelper.GetClassService());
            //await _classAyncProxy.CallAsync(c => c.UpdateClassStudents(cdm.Id,studentIds));
            classStudentIds = classStudentIds.Distinct().ToList();
            await _classAyncProxy.CallAsync(c => c.UpdateClassStudents(cdm.Id, classStudentIds));            
        }

        private void gvStudent_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                //选中一个学员
                this.classStudentIds.Add((e.AddedItems[0] as StudentDisplayModel).Id);
                
                classStudentIds = classStudentIds.Distinct().ToList();
            }
            if(e.RemovedItems.Count > 0)
            {
                //不选择某个学生
                this.classStudentIds.Remove((e.RemovedItems[0] as StudentDisplayModel).Id);
                classStudentIds = classStudentIds.Distinct().ToList();
            }
        }

        private async void gvStudent_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            ClassDisplayModel cdm = this.ClassTypeTree.SelectedItem as ClassDisplayModel;
            if (cdm != null)
            {
                await bindStudentListByClass(cdm.Id);
            }
        }
    }
}
