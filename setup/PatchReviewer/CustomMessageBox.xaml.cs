using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PatchReviewer
{
	public partial class CustomMessageBox
	{
		public class Button
		{
			public string Content { get; set; }
			public object Tag { get; set; }
		}

		public string Message {
			get => TextBlock_Message.Text;
			set => TextBlock_Message.Text = value;
		}

		#region EVIL_WINDOWS_STUFF

		private static readonly Dictionary<MessageBoxImage, Icon> IconMapping = new Dictionary<MessageBoxImage, Icon> {
			{MessageBoxImage.Exclamation, SystemIcons.Exclamation},
			{MessageBoxImage.Error, SystemIcons.Error},
			{MessageBoxImage.Information, SystemIcons.Information},
			{MessageBoxImage.Question, SystemIcons.Question}
		};

		private static ImageSource GetImageSource(MessageBoxImage image) {
			if (!IconMapping.TryGetValue(image, out var icon))
				icon = SystemIcons.Information;

			return Imaging.CreateBitmapSourceFromHIcon(
				icon.Handle,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
		}

		[DllImport("user32.dll")]
		static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

		const int GWL_EXSTYLE = -20;
		const int WS_EX_DLGMODALFRAME = 0x0001;
		const int SWP_NOSIZE = 0x0001;
		const int SWP_NOMOVE = 0x0002;
		const int SWP_NOZORDER = 0x0004;
		const int SWP_FRAMECHANGED = 0x0020;

		#endregion

		private MessageBoxImage image = MessageBoxImage.None;

		public MessageBoxImage Image {
			get => image;
			set {
				image = value;
				if (image == MessageBoxImage.None) {
					Image_MessageBox.Visibility = Visibility.Collapsed;
					return;
				}

				Image_MessageBox.Source = GetImageSource(image);
				Image_MessageBox.Visibility = Visibility.Visible;
			}
		}

		public ObservableCollection<Button> Buttons { get; } = new ObservableCollection<Button>();

		private bool hasResult;
		private object result;

		internal CustomMessageBox() {
			InitializeComponent();

			PreviewKeyDown += (s, e) => {
				if (e.Key == Key.Escape)
					Close();
			};
		}

		protected override void OnSourceInitialized(EventArgs e) {
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_DLGMODALFRAME);
			// Update the window's non-client area to reflect the changes
			SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			hasResult = true;
			result = ((FrameworkElement) sender).Tag;
			Close();
		}

		public bool ShowDialog<T>(out T t, params (string, T)[] buttons) {
			foreach (var (name, result) in buttons)
				Buttons.Add(new Button {
					Content = name,
					Tag = result
				});
			
			ShowDialog();

			t = hasResult ? (T) result : default(T);
			return hasResult;
		}

		public MessageBoxResult ShowDialogOk(string okButton = "OK") {
			if (!ShowDialog(out var result, 
				(okButton, MessageBoxResult.OK)))
				result = MessageBoxResult.Cancel;
			return result;
		}

		public MessageBoxResult ShowDialogOkCancel(string okButton, string cancelButton = "Cancel") {
			if (!ShowDialog(out var result, 
					(okButton, MessageBoxResult.OK),
					(cancelButton, MessageBoxResult.Cancel)))
				result = MessageBoxResult.Cancel;
			return result;
		}

		public MessageBoxResult ShowDialogYesNoCancel(string yesButton, string noButton, string cancelButton = "Cancel") {
			if (!ShowDialog(out var result, 
					(yesButton, MessageBoxResult.Yes),
					(noButton, MessageBoxResult.No),
					(cancelButton, MessageBoxResult.Cancel)))
				result = MessageBoxResult.Cancel;
			return result;
		}
	}
}