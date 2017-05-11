using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Aisino.MES.Client.WPFCommon.Controls
{
    /// <summary>
    /// TransManagerToolBar.xaml 的交互逻辑
    /// </summary>
    public partial class ToolBarUserControl : UserControl
    {
        public ToolBarUserControl()
        {
            InitializeComponent();
        }
        #region 按钮可见性
        private Visibility btnAddVisibility;

        public Visibility BtnAddVisibility
        {
            get { return btnAddVisibility; }
            set { this.btnAdd.Visibility = value; }
        }

        private Visibility btnSaveVisibility;

        public Visibility BtnSaveVisibility
        {
            get { return btnSaveVisibility; }
            set { this.btnSave.Visibility = value; }
        }

        private Visibility btnSubmitVisibility;

        public Visibility BtnSubmitVisibility
        {
            get { return btnSubmitVisibility; }
            set { this.btnSubmit.Visibility = value; }
        }

        private Visibility btnConfirmVisibility;

        public Visibility BtnConfirmVisibility
        {
            get { return btnConfirmVisibility; }
            set { this.btnConfirm.Visibility = value; }
        }

        private Visibility btnAbolishVisibility;

        public Visibility BtnAbolishVisibility
        {
            get { return btnAbolishVisibility; }
            set { this.btnAbolishVisibility = value; }
        }

        private Visibility btnCheckmarkVisibility;

        public Visibility BtnCheckmarkVisibility
        {
            get { return btnCheckmarkVisibility; }
            set { this.btnCheckmark.Visibility = value; }
        }

        private Visibility btnResetVisibility;

        public Visibility BtnResetVisibility
        {
            get { return btnResetVisibility; }
            set { this.btnReset.Visibility = value; }
        }

        #endregion

        #region 按钮可用性
        private bool btnAddEnable;

        public bool BtnAddEnable
        {
            get { return btnAddEnable; }
            set { this.btnAdd.IsEnabled = value; }
        }

        private bool btnSaveEnable;

        public bool BtnSaveEnable
        {
            get { return btnSaveEnable; }
            set { this.btnSave.IsEnabled = value; }
        }

        private bool btnSubmitEnable;

        public bool BtnSubmitEnable
        {
            get { return btnSubmitEnable; }
            set { this.btnSubmit.IsEnabled = value; }
        }

        private bool btnConfirmEnable;

        public bool BtnConfirmEnable
        {
            get { return btnConfirmEnable; }
            set { this.btnConfirm.IsEnabled = value; }
        }

        private bool btnAbolishEnable;

        public bool BtnAbolishEnable
        {
            get { return btnAbolishEnable; }
            set { this.btnAbolish.IsEnabled = value; }
        }

        private bool btnCheckmarkEnable;

        public bool BtnCheckmarkEnable
        {
            get { return BtnCheckmarkEnable; }
            set { this.btnCheckmark.IsEnabled = value; }
        }

        private bool btnResetEnable;

        public bool BtnResetEnable
        {
            get { return btnResetEnable; }
            set { this.btnReset.IsEnabled = value; }
        }
        #endregion


        #region 控件按钮路由事件
        public static readonly RoutedEvent AddEvent = EventManager.RegisterRoutedEvent(
        "Add", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Add
        {
            add { AddHandler(AddEvent, value); }
            remove { RemoveHandler(AddEvent, value); }
        }

        private void AddButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(AddEvent, this));
        }

        public static readonly RoutedEvent SaveEvent = EventManager.RegisterRoutedEvent(
        "Save", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }

        private void SaveButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(SaveEvent, this));
        }

        public static readonly RoutedEvent ConfirmEvent = EventManager.RegisterRoutedEvent(
        "Confirm", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Confirm
        {
            add { AddHandler(ConfirmEvent, value); }
            remove { RemoveHandler(ConfirmEvent, value); }
        }

        private void ConfirmButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(ConfirmEvent, this));
        }

        public static readonly RoutedEvent CheckmarkEvent = EventManager.RegisterRoutedEvent(
        "Checkmark", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Checkmark
        {
            add { AddHandler(CheckmarkEvent, value); }
            remove { RemoveHandler(CheckmarkEvent, value); }
        }

        private void CheckmarkButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(CheckmarkEvent, this));
        }

        public static readonly RoutedEvent SubmitEvent = EventManager.RegisterRoutedEvent(
        "Submit", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Submit
        {
            add { AddHandler(SubmitEvent, value); }
            remove { RemoveHandler(SubmitEvent, value); }
        }

        private void SubmitButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(SubmitEvent, this));
        }

        public static readonly RoutedEvent ResetEvent = EventManager.RegisterRoutedEvent(
        " Reset", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Reset
        {
            add { AddHandler(ResetEvent, value); }
            remove { RemoveHandler(ResetEvent, value); }
        }

        private void ResetButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(ResetEvent, this));
        }

        public static readonly RoutedEvent AbolishEvent = EventManager.RegisterRoutedEvent(
        "Abolish", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Abolish
        {
            add { AddHandler(AbolishEvent, value); }
            remove { RemoveHandler(AbolishEvent, value); }
        }

        private void AbolishButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(AbolishEvent, this));
        }

        public static readonly RoutedEvent SearchEvent = EventManager.RegisterRoutedEvent(
        "Search", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarUserControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        private void SearchButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(SearchEvent, this));
        }
        #endregion

        
    }
}
