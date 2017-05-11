/********************************************************
 * File : MessageBox
 * Version : 1.0.0.6
 * Author : Vito.K / ( Ye.Wang )
 * E-mail : vito2015@live.com / ( ye.wang@combuilder.com.sg )
 * Date：2013/8/2 15:46:42
 * Description :  
 * 	// code description here.
 * ChangedHistory :
 * 
 *		[1.0.0.6] - 2013-12-23 11:28:49
 *		1) 修改 MessageBoxModule 创建CtrlButton的方式以及响应动作由Event转为Command并由强引用转为弱引用.
 *      2) 加入 2个 具有 MessageBoxImage 参数的Show 方法,用以兼容系统默认的MessageBox.
 *      
 * 
 ********************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aisino.MES.Client.WPFCommon.WPFMessageBox
{
	[TemplatePart( Name = "PART_Title", Type = typeof( TextBlock ) )]
	[TemplatePart( Name = "PART_Message", Type = typeof( TextBlock ) )]
	[TemplatePart( Name = "PART_CtrlButtonCollection", Type = typeof( ItemsControl ) )]
	internal sealed class MessageBoxModule : Window
	{
		#region Private static fields

		private static Style CTRL_BUTTON_STYLE = Button.StyleProperty.DefaultMetadata.DefaultValue as Style;
		private static readonly Brush DefaultTitleForeground = new SolidColorBrush( Color.FromArgb( 100, 45, 100, 160 ) );

		private static bool B_USED_CUSTOM_BRUSHES = false;
		private static bool B_USED_CUSTOM_SIZE = false;
		private static MessageBoxCustomInfo MB_CUSTOMINFO;

		#endregion // Private static fields

		#region Dependency Properties

		public new static readonly DependencyProperty TitleProperty =
	DependencyProperty.Register( "Title", typeof( string ), typeof( MessageBoxModule ), new PropertyMetadata( "标题" ) );

		public static readonly DependencyProperty MessageProperty =
	DependencyProperty.Register( "Message", typeof( string ), typeof( MessageBoxModule ), new PropertyMetadata( "" ) );

		public static readonly DependencyProperty TitleForegroundProperty =
	DependencyProperty.Register( "TitleForeground", typeof( Brush ), typeof( MessageBoxModule ), new PropertyMetadata( DefaultTitleForeground ) );

		public static readonly DependencyProperty CtrlButtonCollectionProperty =
	DependencyProperty.Register( "CtrlButtonCollection", typeof( ObservableCollection<Button> ), typeof( MessageBoxModule ), new PropertyMetadata( OnCtrlButtonCollectionChanged ) );


		public static readonly DependencyProperty CtrlButtonStyleProperty =
			DependencyProperty.Register( "CtrlButtonStyle", typeof( Style ), typeof( MessageBoxModule ), new PropertyMetadata() );


		#endregion // Dependency Properties

		#region ctors

		static MessageBoxModule()
		{
			DefaultStyleKeyProperty.OverrideMetadata( typeof( MessageBoxModule ), new FrameworkPropertyMetadata( typeof( MessageBoxModule ) ) );
		}

		public MessageBoxModule()
		{
			SetValue( CtrlButtonCollectionProperty, new ObservableCollection<Button>() );

			try
			{
				this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
				this.AllowsTransparency = true;
				this.WindowStyle = System.Windows.WindowStyle.None;
				this.ShowInTaskbar = true;
				this.Topmost = true;

				/*
				 use "MessageBoxModule.SetDefaultCtorButtonStyle" at custom setting only.
				 default use CtrlButtonStyleProperty. 
				 remark by ye.wang 2012.12.01
				*/
				//Resources.Source = new Uri( @"/MIV.Bus.WPF.UIShell;component/Styles/MLWindowCtrlButtonStyles.xaml", UriKind.Relative );
				//if ( Resources.Contains( "MessageBoxButtonStyle" ) )
				//{
				//	MessageBoxModule.SetDefaultCtorButtonStyle( Resources["MessageBoxButtonStyle"] as Style );
				//}


			}
			catch { }

		}

		#endregion // ctors

		#region Public Static Functions

		public static void SetDefaultCtorButtonStyle( Style buttonStyle )
		{
			CTRL_BUTTON_STYLE = buttonStyle;
		}

		public static void SetMessageBoxCustomDefine( MessageBoxCustomInfo mbCustomIf )
		{
			if ( !default( MessageBoxCustomInfo ).Equals( mbCustomIf ) )
			{
				MessageBoxModule.MB_CUSTOMINFO = mbCustomIf;
				MessageBoxModule.B_USED_CUSTOM_BRUSHES = true;
				MessageBoxModule.B_USED_CUSTOM_SIZE = true;

			}
			else
			{
				MessageBoxModule.MB_CUSTOMINFO = default( MessageBoxCustomInfo );
				MessageBoxModule.B_USED_CUSTOM_BRUSHES = false;
				MessageBoxModule.B_USED_CUSTOM_SIZE = false;
			}
		}

		public static void ResetMessageBoxCustomDefine()
		{
			CTRL_BUTTON_STYLE = Button.StyleProperty.DefaultMetadata.DefaultValue as Style;
			MB_CUSTOMINFO = default( MessageBoxCustomInfo );
			B_USED_CUSTOM_BRUSHES = false;
			B_USED_CUSTOM_SIZE = false;
		}

		#region Show MessageBox Functions

		public static MessageBoxResult Show( string messageBoxText )
		{
			return Show( null, messageBoxText, "", MessageBoxButton.OK );
		}

		public static MessageBoxResult Show( string messageBoxText, string caption )
		{
			return Show( null, messageBoxText, caption, MessageBoxButton.OK );
		}

		public static MessageBoxResult Show( string messageBoxText, string caption, MessageBoxButton button )
		{
			return Show( null, messageBoxText, caption, button );
		}

		public static MessageBoxResult Show( Window owner, string messageBoxText )
		{
			return Show( owner, messageBoxText, "", MessageBoxButton.OK );
		}

		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, MessageBoxButton button )
		{
			MessageBoxResult defRsult = MessageBoxResult.None;
			switch ( button )
			{
				case MessageBoxButton.OK:
					defRsult = MessageBoxResult.OK;
					break;
				case MessageBoxButton.OKCancel:
					defRsult = MessageBoxResult.Cancel;
					break;
				case MessageBoxButton.YesNo:
					defRsult = MessageBoxResult.No;
					break;
				case MessageBoxButton.YesNoCancel:
					defRsult = MessageBoxResult.Cancel;
					break;
				default:
					break;
			}

			return Show( owner, messageBoxText, caption, button, MessageBoxImage.None, defRsult );
		}

		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, IList<MessageBoxButtonInfo> ctrlButtons )
		{
			var mbox = new MessageBoxModule();
			mbox.Message = messageBoxText;
			mbox.Title = caption;
			mbox.Owner = owner;

			IsUseCustomInfoDefine( ref mbox );

			if ( owner != null )
			{
				mbox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			if ( null != ctrlButtons && ctrlButtons.Count > 0 )
			{
				foreach ( var btnInfo in ctrlButtons )
				{
					switch ( btnInfo.Result )
					{
						case MessageBoxResult.Cancel:
						case MessageBoxResult.No:
							{
								var btn = CreateCtrlButtonWithResult( mbox, btnInfo.ContentText, false, btnInfo.Action, btnInfo.IsDefault );
								mbox.CtrlButtonCollection.Add( btn );
							}
							break;
						case MessageBoxResult.None:
							{
								var btn = CreateCtrlButtonWithResult( mbox, btnInfo.ContentText, null, btnInfo.Action, btnInfo.IsDefault );
								mbox.CtrlButtonCollection.Add( btn );
							}
							break;
						case MessageBoxResult.OK:
						case MessageBoxResult.Yes:
						default:
							{
								var btn = CreateCtrlButtonWithResult( mbox, btnInfo.ContentText, true, btnInfo.Action, btnInfo.IsDefault );
								mbox.CtrlButtonCollection.Add( btn );
							}
							break;
					}
				}

				var result = mbox.ShowDialog();
				return MessageBoxResult.None;
			}
			else
			{
				return Show( owner, messageBoxText, caption, MessageBoxButton.OK );
			}


		}


		/******************* 以下 2个 方法中的 icon 参数为兼容 System.Windows.MessageBox 的留白**************************/

		public static MessageBoxResult Show( string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult )
		{
			return Show( null, messageBoxText, caption, button, icon, defaultResult );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="messageBoxText"></param>
		/// <param name="caption"></param>
		/// <param name="button"></param>
		/// <param name="icon">为兼容 System.Windows.MessageBox 的留白.</param>
		/// <param name="defaultResult"></param>
		/// <returns></returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult )
		{
			var mbox = new MessageBoxModule();
			mbox.Message = messageBoxText;
			mbox.Title = caption;
			mbox.Owner = owner;

			IsUseCustomInfoDefine( ref mbox );

			if ( owner != null )
			{
				mbox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			switch ( button )
			{
				case MessageBoxButton.OKCancel:

					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "确定", true, null, defaultResult == MessageBoxResult.OK ) );

					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "取消", false, null, defaultResult == MessageBoxResult.Cancel ) );
					break;
				//break;
				case MessageBoxButton.YesNo:
					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "是", true, null, defaultResult == MessageBoxResult.Yes ) );

					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "否", false, null, defaultResult == MessageBoxResult.No ) );

					break;
				case MessageBoxButton.YesNoCancel:
					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "是", true, null, defaultResult == MessageBoxResult.Yes ) );

					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "否", false, null, defaultResult == MessageBoxResult.No ) );

					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "取消", null, null, defaultResult == MessageBoxResult.Cancel ) );
					break;
				case MessageBoxButton.OK:
				default:
					mbox.CtrlButtonCollection.Add( CreateCtrlButtonWithResult( mbox, "确定", true, null, defaultResult == MessageBoxResult.OK ) );
					break;
			}
            var result = mbox.ShowDialog();
            
			switch ( button )
			{

				//break;
				case MessageBoxButton.OKCancel:
					{
						return result == true ? MessageBoxResult.OK
							: result == false ? MessageBoxResult.Cancel :
							MessageBoxResult.None;
					}
				//break;
				case MessageBoxButton.YesNo:
					{
						return result == true ? MessageBoxResult.Yes : MessageBoxResult.No;
					}
				//break;
				case MessageBoxButton.YesNoCancel:
					{
						return result == true ? MessageBoxResult.Yes
							: result == false ? MessageBoxResult.No :
							MessageBoxResult.Cancel;
					}

				case MessageBoxButton.OK:
				default:
					{
						return result == true ? MessageBoxResult.OK : MessageBoxResult.None;
					}
			}
		}
		#endregion // Show MessageBox Functions

		#endregion // Public Static Functions

		#region Properties

		public new string Title
		{
			get { return (string)GetValue( TitleProperty ); }
			set { SetValue( TitleProperty, value ); }
		}

		public string Message
		{
			get { return (string)GetValue( MessageProperty ); }
			set { SetValue( MessageProperty, value ); }
		}

		public Brush TitleForeground
		{
			get { return (Brush)GetValue( TitleForegroundProperty ); }
			set { SetValue( TitleForegroundProperty, value ); }
		}

		public ObservableCollection<Button> CtrlButtonCollection
		{
			get { return (ObservableCollection<Button>)GetValue( CtrlButtonCollectionProperty ); }
			set { SetValue( CtrlButtonCollectionProperty, value ); }
		}

		public Style CtrlButtonStyle
		{
			get { return (Style)GetValue( CtrlButtonStyleProperty ); }
			set { SetValue( CtrlButtonStyleProperty, value ); }
		}

		#endregion // Properties

		#region Private static functions

		private static Button CreateCtrlButton( string content )
		{
			Button btn = new Button();
			btn.Padding = new Thickness( 20, 3, 20, 3 );
			btn.Content = content;
			//btn.ToolTip = content;
			if ( null != CTRL_BUTTON_STYLE )
			{
				btn.Style = CTRL_BUTTON_STYLE;
			}
			btn.Focusable = true;
			return btn;
		}

		private static Button CreateCtrlButtonWithResult( MessageBoxModule mbox, string content, bool? dialogResult, Action action, bool isDefault = false )
		{
			var btn = CreateCtrlButton( content );
			btn.IsDefault = isDefault;
			Action actionDialogResult = new Action( () =>
			{
				mbox.DialogResult = dialogResult;
				if ( null == dialogResult )
					mbox.Close();
			} );

			if ( null == action )
				action = new Action( () => { } );

			Action mulitcastDelegate = (Action)MulticastDelegate.Combine( actionDialogResult, action );

			if ( null != mulitcastDelegate )
			{
				btn.Command = new MessageBoxRelayCommand( mulitcastDelegate );
			}
			return btn;
		}

		#region old button action
		//private static Button CreateCtrlButton( string content, RoutedEventHandler clickHandler )
		//{
		//	Button btn = new Button();
		//	btn.Padding = new Thickness( 20, 3, 20, 3 );
		//	btn.Content = content;
		//	btn.ToolTip = content;
		//	if ( null != CTRL_BUTTON_STYLE )
		//	{
		//		btn.Style = CTRL_BUTTON_STYLE;
		//	}
		//	btn.Click += clickHandler;

		//	return btn;
		//}

		//private static Button CreateCtrlButton_ResultTrue( MessageBoxModule mbox, string content )
		//{
		//	return CreateCtrlButton( content, new RoutedEventHandler( ( o, e ) =>
		//	{
		//		try
		//		{
		//			mbox.DialogResult = true;
		//			//mbox.Close();
		//		}
		//		catch { }
		//	} ) );
		//}

		//private static Button CreateCtrlButton_ResultFalse( MessageBoxModule mbox, string content )
		//{
		//	return CreateCtrlButton( content, new RoutedEventHandler( ( o, e ) =>
		//	{
		//		try
		//		{
		//			mbox.DialogResult = false;
		//			//mbox.Close();
		//		}
		//		catch { }
		//	} ) );
		//}

		//private static Button CreateCtrlButton_ResultNull( MessageBoxModule mbox, string content )
		//{
		//	return CreateCtrlButton( content, new RoutedEventHandler( ( o, e ) =>
		//	{
		//		try
		//		{
		//			mbox.DialogResult = null;
		//			//mbox.Close();
		//		}
		//		catch { }
		//	} ) );
		//} 
		#endregion

		private static void IsUseCustomInfoDefine( ref MessageBoxModule mbox )
		{
			if ( B_USED_CUSTOM_BRUSHES && null != mbox )
			{
				if ( MB_CUSTOMINFO.IsBackgroundChanged )
				{
					mbox.Background = MB_CUSTOMINFO.MB_Background;
				}
				if ( MB_CUSTOMINFO.IsBorderBrushChanged )
				{
					mbox.BorderBrush = MB_CUSTOMINFO.MB_Borderbrush;
				}
				if ( MB_CUSTOMINFO.IsBorderThicknessChanged )
				{
					mbox.BorderThickness = MB_CUSTOMINFO.MB_BorderThickness;
				}
				if ( MB_CUSTOMINFO.IsForegroundChanged )
				{
					mbox.Foreground = MB_CUSTOMINFO.MB_Foreground;
				}
				if ( MB_CUSTOMINFO.IsTitleForegroundChanged )
				{
					mbox.TitleForeground = MB_CUSTOMINFO.MB_Title_Foreground;
				}
			}
			if ( B_USED_CUSTOM_SIZE && null != mbox )
			{
				if ( MB_CUSTOMINFO.IsMaxHeightChanged )
				{
					mbox.MaxHeight = MB_CUSTOMINFO.MB_MaxHeight;
				}
				if ( MB_CUSTOMINFO.IsMaxWidthChanged )
				{
					mbox.MaxWidth = MB_CUSTOMINFO.MB_MaxWidth;
				}
			}
		}

		private static void OnCtrlButtonCollectionChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (MessageBoxModule)d ).OnCtrlButtonCollectionChanged( e );
		}

		private void OnCtrlButtonCollectionChanged( DependencyPropertyChangedEventArgs e )
		{

		}

		#endregion // Private static functions

		#region Override functions

		protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
		{
			this.DragMove();

			base.OnMouseLeftButtonDown( e );
		}
		protected override void OnClosed( EventArgs e )
		{
			if ( null != CtrlButtonCollection )
			{
				this.CtrlButtonCollection.Clear();
			}
			base.OnClosed( e );
		}

		#endregion // Override functions
	}

	/// <summary>
	/// 	<see cref="MIV.Bus.WPF.UIShell.Controls.MessageBoxModule"/> 组件中 MessageBoxButton 的自定义设置信息.
	/// </summary>
	/// <remarks>Designed by ye.wang</remarks>
	public class MessageBoxButtonInfo
	{
		#region fields

		private string _contentText = "";
		private MessageBoxResult _result = MessageBoxResult.OK;
		private Action _action = null;
		private bool _isDefault = false;
		#endregion // fields

		#region ctor

		/// <summary>
		/// <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 自定义按钮的基本信息.
		/// </summary>
		/// <param name="contentText">按钮的文本内容</param>
		/// <param name="result">按钮响应的返回结果</param>
		/// <param name="action">按钮的响应动作</param>
		public MessageBoxButtonInfo( string contentText, MessageBoxResult result, Action action, bool isDefault = false )
		{
			this._contentText = contentText;
			this._result = result;
			this._isDefault = isDefault;
			if ( null != action )
			{
				this._action = action;
			}
			else
			{
				this._action = new Action( () =>
				{
				} );
			}
		}

		#endregion // ctor

		#region Readonly Properties

		/// <summary>
		/// 获取 <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 按钮的文本内容.
		/// </summary>
		public string ContentText
		{
			get { return _contentText; }
		}

		/// <summary>
		/// 获取 <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 按钮响应的返回结果.
		/// </summary>
		public MessageBoxResult Result
		{
			get { return _result; }
		}

		/// <summary>
		/// 获取 <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 按钮的响应动作.
		/// </summary>
		public Action Action
		{
			get { return _action; }
		}

		/// <summary>
		/// 获取 <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 按钮是否是默认按钮.
		/// </summary>
		public bool IsDefault
		{
			get
			{
				return _isDefault;
			}
		}

		#endregion // Readonly Properties
	}

	/// <summary>
	/// <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 自定义信息结构.
	/// </summary>
	public struct MessageBoxCustomInfo
	{
		#region private fields

		private bool isBackgroundChanged;
		private bool isTitleForegroundChanged;
		private bool isForegroundChanged;
		private bool isBorderBrushChanged;
		private bool isBorderThicknessChanged;
		private bool isUseCustomWidth;
		private bool isUseCustomHeight;

		private Brush mb_background;
		private Brush mb_title_foreground;
		private Brush mb_foreground;
		private Brush mb_borderbrush;
		private Thickness mb_borderthickness;


		private double mb_maxWidth;
		private double mb_maxHeight;

		#endregion // private fields

		#region public properties

		public bool IsBackgroundChanged
		{
			get { return isBackgroundChanged; }
		}
		public bool IsTitleForegroundChanged
		{
			get { return isTitleForegroundChanged; }
		}
		public bool IsForegroundChanged
		{
			get { return isForegroundChanged; }
		}
		public bool IsBorderBrushChanged
		{
			get { return isBorderBrushChanged; }
		}
		public bool IsBorderThicknessChanged
		{
			get { return isBorderThicknessChanged; }
		}

		public bool IsMaxWidthChanged
		{
			get { return isUseCustomWidth; }
		}

		public bool IsMaxHeightChanged
		{
			get { return isUseCustomHeight; }
		}

		public Brush MB_Background
		{
			get { return mb_background; }
			set
			{
				mb_background = value;
				isBackgroundChanged = true;
			}
		}
		public Brush MB_Title_Foreground
		{
			get { return mb_title_foreground; }
			set
			{
				mb_title_foreground = value;
				isTitleForegroundChanged = true;
			}
		}
		public Brush MB_Foreground
		{
			get { return mb_foreground; }
			set
			{
				mb_foreground = value;
				isForegroundChanged = true;
			}
		}
		public Brush MB_Borderbrush
		{
			get { return mb_borderbrush; }
			set
			{
				mb_borderbrush = value;
				isBorderBrushChanged = true;
			}
		}
		public Thickness MB_BorderThickness
		{
			get { return mb_borderthickness; }
			set
			{
				mb_borderthickness = value;
				isBorderThicknessChanged = true;
			}
		}

		public double MB_MaxWidth
		{
			get { return mb_maxWidth; }
			set
			{
				mb_maxWidth = value;
				isUseCustomWidth = true;
			}
		}

		public double MB_MaxHeight
		{
			get { return mb_maxHeight; }
			set
			{
				mb_maxHeight = value;
				isUseCustomHeight = true;
			}
		}



		#endregion // public properties


	}

	/// <summary>
	/// <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBoxModule"/> 组件的自定义事件
	/// </summary>
	[Obsolete( "请使用弱引用命令.", true )]
	public class MessageBoxCommand : ICommand
	{
		#region Private Fields
		private readonly Action<object> _command;
		private readonly Func<object, bool> _canExecute;
		#endregion

		#region Constructor
		public MessageBoxCommand( Action<object> command, Func<object, bool> canExecute = null )
		{
			if ( command == null )
				throw new ArgumentNullException( "command" );
			_canExecute = canExecute;
			_command = command;
		}
		#endregion

		#region ICommand Members
		public void Execute( object parameter )
		{
			_command( parameter );
		}
		public bool CanExecute( object parameter )
		{
			if ( _canExecute == null )
				return true;
			return _canExecute( parameter );
		}
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		#endregion
	}

	/// <summary>
	/// <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBoxModule"/> 组件的自定义事件
	/// </summary>
	public class MessageBoxRelayCommand : RelayCommand
	{
		public MessageBoxRelayCommand( Action execute )
			: base( execute )
		{
		}
		public MessageBoxRelayCommand( Action execute, Func<bool> canExecute )
			: base( execute, canExecute )
		{
		}
	}

	/// <summary>
	/// 显示消息框.
	/// </summary>
	public sealed class AisinoMessageBox
	{
		#region ctors

		static AisinoMessageBox()
		{
		}
		private AisinoMessageBox()
		{
		}

		#endregion // ctors

		//private double maxWidth=350;

		//public double MaxWidth
		//{
		//    get { return maxWidth; }
		//    set { maxWidth = value; }
		//}

		#region custom settings

		/// <summary>
		/// 设置 <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 的按钮样式.
		/// </summary>
		/// <param name="buttonStyle"></param>
		public static void SetDefaultCtorButtonStyle( Style buttonStyle )
		{
			MessageBoxModule.SetDefaultCtorButtonStyle( buttonStyle );
		}

		/// <summary>
		/// 设置 <see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 的一些自定义信息.
		/// </summary>
		/// <param name="mbCustomIf"><see cref="MIV.Bus.WPF.UIShell.Controls.MessageBox"/> 自定义信息结构</param>
		public static void SetMessageBoxCustomDefine( MessageBoxCustomInfo mbCustomIf )
		{
			MessageBoxModule.SetMessageBoxCustomDefine( mbCustomIf );
		}

		public static void ResetMessageBoxCustomDefine()
		{
			MessageBoxModule.ResetMessageBoxCustomDefine();
		}

		#endregion // custom settings

		#region Show functions

		/// <summary>
		/// 显示一个消息框，该消息框包含消息并返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 System.String，用于指定要显示的文本。</param>
		/// <returns>一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText )
		{
			return MessageBoxModule.Show( messageBoxText );
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText, string caption )
		{
			return MessageBoxModule.Show( messageBoxText, caption );
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息、标题栏标题和按钮，并且返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <returns>一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText, string caption, MessageBoxButton button )
		{
			return MessageBoxModule.Show( messageBoxText, caption, button );
		}

		/// <summary>
		/// 在指定窗口的前面显示消息框。该消息框显示消息并返回结果。
		/// </summary>
		/// <param name="owner">一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText )
		{
			return MessageBoxModule.Show( owner, messageBoxText );
		}

		/// <summary>
		///  在指定窗口的前面显示消息框。该消息框显示消息、标题栏标题和按钮，并且返回结果。
		/// </summary>
		/// <param name="owner"> 一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, MessageBoxButton button )
		{
			return MessageBoxModule.Show( owner, messageBoxText, caption, button );
		}

		/// <summary>
		/// 在指定窗口的前面显示消息框。该消息框显示消息、标题栏标题和按钮，并且支持自定义按钮和动作。
		/// </summary>
		/// <param name="owner"> 一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="ctrlButtons">一组自定义的按钮和响应动作。</param>
		/// <returns> <paramref name="ctrlButtons"/> 中点击的 MessageBoxButtonInfo.Result 结果.</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, IList<MessageBoxButtonInfo> ctrlButtons )
		{
			return MessageBoxModule.Show( owner, messageBoxText, caption, ctrlButtons );
		}


		/// <summary>
		///  显示一个消息框，该消息框包含消息、标题栏标题、按钮和图标，并接受默认消息框结果和返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <param name="icon"> 
		/// 一个 <see cref="System.Windows.MessageBoxImage"/> 值，用于指定要显示的图标。
		/// 未使用,留白.
		/// </param>
		/// <param name="defaultResult"> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定消息框的默认结果。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult )
		{
			return MessageBoxModule.Show( messageBoxText, caption, button, icon, defaultResult );
		}

		/// <summary>
		/// 在指定窗口的前面显示消息框。该消息框显示消息、标题栏标题、按钮和图标，并接受默认消息框结果和返回结果。
		/// </summary>
		/// <param name="owner">一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <param name="icon"> 
		/// 一个 <see cref="System.Windows.MessageBoxImage"/> 值，用于指定要显示的图标。
		/// 未使用,留白.
		/// </param>
		/// <param name="defaultResult"> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定消息框的默认结果。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult )
		{
			return MessageBoxModule.Show( owner, messageBoxText, caption, button, icon, defaultResult );
		}
		#endregion // Show functions
	}
    
	public sealed class MIVMessageBox
	{
		public static MIVMessageBoxType MessageBoxType { get; set; }

		#region ctors
		static MIVMessageBox()
		{
			MessageBoxType = MIVMessageBoxType.System;
		}
		private MIVMessageBox()
		{
		}
		#endregion // ctors

		#region Show functions

		/// <summary>
		/// 显示一个消息框，该消息框包含消息并返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 System.String，用于指定要显示的文本。</param>
		/// <returns>一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText )
		{
			if ( MessageBoxType== MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( messageBoxText );

			return MessageBoxModule.Show( messageBoxText );
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息和标题栏标题，并且返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <returns>一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText, string caption )
		{
			if ( MessageBoxType == MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( messageBoxText, caption );
			return MessageBoxModule.Show( messageBoxText, caption );
		}

		/// <summary>
		/// 显示一个消息框，该消息框包含消息、标题栏标题和按钮，并且返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <returns>一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText, string caption, MessageBoxButton button )
		{
			if ( MessageBoxType == MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( messageBoxText, caption, button );
			return MessageBoxModule.Show( messageBoxText, caption, button );
		}

		/// <summary>
		/// 在指定窗口的前面显示消息框。该消息框显示消息并返回结果。
		/// </summary>
		/// <param name="owner">一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText )
		{
			if ( MessageBoxType == MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( owner, messageBoxText );
			return MessageBoxModule.Show( owner, messageBoxText );
		}

		/// <summary>
		///  在指定窗口的前面显示消息框。该消息框显示消息、标题栏标题和按钮，并且返回结果。
		/// </summary>
		/// <param name="owner"> 一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, MessageBoxButton button )
		{
			if ( MessageBoxType == MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( owner, messageBoxText, caption, button );
			return MessageBoxModule.Show( owner, messageBoxText, caption, button );
		}

		/// <summary>
		/// 在指定窗口的前面显示消息框。该消息框显示消息、标题栏标题和按钮，并且支持自定义按钮和动作。
		/// </summary>
		/// <param name="owner"> 一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="ctrlButtons">一组自定义的按钮和响应动作。</param>
		/// <returns> <paramref name="ctrlButtons"/> 中点击的 MessageBoxButtonInfo.Result 结果.</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, IList<MessageBoxButtonInfo> ctrlButtons )
		{
			return MessageBoxModule.Show( owner, messageBoxText, caption, ctrlButtons );
		}


		/// <summary>
		///  显示一个消息框，该消息框包含消息、标题栏标题、按钮和图标，并接受默认消息框结果和返回结果。
		/// </summary>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <param name="icon"> 
		/// 一个 <see cref="System.Windows.MessageBoxImage"/> 值，用于指定要显示的图标。
		/// 未使用,留白.
		/// </param>
		/// <param name="defaultResult"> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定消息框的默认结果。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult )
		{
			if ( MessageBoxType == MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( messageBoxText, caption, button, icon, defaultResult );
			return MessageBoxModule.Show( messageBoxText, caption, button, icon, defaultResult );
		}

		/// <summary>
		/// 在指定窗口的前面显示消息框。该消息框显示消息、标题栏标题、按钮和图标，并接受默认消息框结果和返回结果。
		/// </summary>
		/// <param name="owner">一个 <see cref="System.Windows.Window"/>，表示消息框的所有者窗口。</param>
		/// <param name="messageBoxText">一个 <see cref="System.String"/>，用于指定要显示的文本。</param>
		/// <param name="caption"> 一个 <see cref="System.String"/>，用于指定要显示的标题栏标题。</param>
		/// <param name="button">一个 <see cref="System.Windows.MessageBoxButton"/> 值，用于指定要显示哪个按钮或哪些按钮。</param>
		/// <param name="icon"> 
		/// 一个 <see cref="System.Windows.MessageBoxImage"/> 值，用于指定要显示的图标。
		/// 未使用,留白.
		/// </param>
		/// <param name="defaultResult"> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定消息框的默认结果。</param>
		/// <returns> 一个 <see cref="System.Windows.MessageBoxResult"/> 值，用于指定用户单击了哪个消息框按钮。</returns>
		public static MessageBoxResult Show( Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult )
		{
			if ( MessageBoxType == MIVMessageBoxType.System )
				return System.Windows.MessageBox.Show( owner, messageBoxText, caption, button, icon, defaultResult );
			return MessageBoxModule.Show( owner, messageBoxText, caption, button, icon, defaultResult );
		}
		#endregion // Show functions
	}

	public enum MIVMessageBoxType
	{ 
		System = 0,
		UIShell = 1
	}
}
