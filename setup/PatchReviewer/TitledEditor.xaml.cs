using System.ComponentModel;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace PatchReviewer
{
	/// <summary>
	/// Interaction logic for SideBySide.xaml
	/// </summary>
	public partial class TitledEditor
	{
		public string Title {
			get => (string) title.Content;
			set => title.Content = value;
		}

		public TitledEditor() {
			InitializeComponent();

			DependencyPropertyDescriptor
				.FromProperty(TextEditor.IsModifiedProperty, typeof(TextEditor))
				.AddValueChanged(editor, (s, e) => UpdateTitle());
		}

		void UpdateTitle() {
			title.FontWeight = editor.IsModified ? FontWeights.Bold : FontWeights.Normal;
			Title = Title.TrimEnd(' ', '*') + (editor.IsModified ? " *" : "");
		}
	}
}