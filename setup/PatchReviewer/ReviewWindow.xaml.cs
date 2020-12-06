using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using DiffPatch;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PatchReviewer
{
	public partial class ReviewWindow : INotifyPropertyChanged
	{
		public bool AutoHeaders { get; set; }

		public ObservableCollection<FilePatcherViewModel> Files { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		private FilePatcherViewModel File;
		private ResultViewModel Result;

		private LineRange leftEditRange, rightEditRange;

		private bool editorsInSync;
		//private Patch userPatch;

		public ReviewWindow(IEnumerable<FilePatcher> files, string commonBasePath = null) {
			Files = new ObservableCollection<FilePatcherViewModel>(files.Select(f => new FilePatcherViewModel(f, commonBasePath)).OrderBy(f => f.Label));

			InitializeComponent();
			SetupEditors();
			Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(SelectFirstItem));
		}

		private void SelectFirstItem() {
			Select(Files[0]);
			if (NextReviewItem(false) is ResultViewModel r)
				Select(r);
		}

		private void SetupEditors() {
			filePanel.SetTitles("Original File", "Patched File");

			patchPanel.SetTitles("Original Patch", "Applied Patch");
			patchPanel.SyntaxHighlighting = LoadHighlighting(Properties.Resources.Patch_Mode);
		}

		private IHighlightingDefinition LoadHighlighting(byte[] resource) {
			using (Stream s = new MemoryStream(resource))
			using (XmlTextReader reader = new XmlTextReader(s))
				return HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}

		#region TreeView Items

		/*private void PopulateTreeView() {
			foreach (var f in files)
				treeView.Items.Add(UpdateItem(new TreeViewItem(), f, true));
		}*/

		/*public string GetFileLabel(FilePatcher f) {
			var label = f.patchFile.basePath;
			if (commonBasePath != null && label.StartsWith(commonBasePath))
				return label.Substring(commonBasePath.Length);

			return label;
		}

		private TreeViewItem UpdateItem(TreeViewItem item, FilePatcher f, bool regenChildren) {
			item.Header = GetFileLabel(f);
			item.Background = StatusBrush(GetStatus(f));
			//item.FontFamily = new FontFamily("Courier New");
			item.Tag = f;
			SetItemModifiedStyle(item, false);

			if (regenChildren) {
				item.Items.Clear();
				foreach (var r in f.results)
					item.Items.Add(UpdateItem(new TreeViewItem(), r));
			}

			return item;
		}

		private TreeViewItem UpdateItem(TreeViewItem item, Patcher.Result r) =>
			UpdateItem(item, r, IsRemoved(r));

		private TreeViewItem UpdateItem(TreeViewItem item, Patcher.Result r, bool removed) {
			var text = r.Summary();
			if (removed)
				text = $"REMOVED: {r.patch.Header}";

			item.Margin = new Thickness(-15, 0, 0, 0);
			item.Header = new StackPanel { Orientation = Orientation.Horizontal, Children = {
					new TextBlock { Text =  , Width = 30 },
					new TextBlock { Text = text },
				}
			};

			item.Background = StatusBrush(GetStatus(r));
			//item.FontFamily = new FontFamily("Courier New");
			item.Tag = r;
			SetItemModifiedStyle(item, false);

			return item;
		}*/

		/*private static void SetItemModifiedStyle(TreeViewItem item, bool modified) {
			item.FontWeight = modified ? FontWeights.Bold : FontWeights.Normal;

			// ugly hacks, relly should have bindings and MMVM, but lack time right now
			if (item.Tag is FilePatcher) {
				var title = ((string)item.Header).TrimEnd(' ', '*');
				if (modified)
					title += " *";
				item.Header = title;
			}
			else {
				var endTextBox = ((StackPanel)item.Header).Children.OfType<TextBlock>().Last();
				var title = endTextBox.Text.TrimEnd(' ', '*');
				if (modified)
					title += " *";
				endTextBox.Text = title;
			}
		}*/


		private ResultViewModel NextReviewItem(bool backwards) {
			if (treeView?.SelectedItem == null)
				return null;

			// not the fastest way to implement this, but one of the easiest
			var results = Files.SelectMany(f => f.Results).ToList();
			if (backwards)
				results.Reverse();

			int i;
			if (treeView.SelectedItem is ResultViewModel selectedResult) {
				i = results.IndexOf(selectedResult) + 1;
			}
			else if (treeView.SelectedItem is FilePatcherViewModel selectedFile) {
				// last result of prev file / next result of current file
				var r = backwards ? 
					Files.Take(Files.IndexOf(selectedFile)).LastOrDefault(f => f.Results.Any())?.Results.Last() :
					Files.Skip(Files.IndexOf(selectedFile)).FirstOrDefault(f => f.Results.Any())?.Results.First();

				if (r == null)
					return null;

				i = results.IndexOf(r);
			}
			else {
				throw new Exception("SelectedItem of unknown type");
			}

			return results.Skip(i).FirstOrDefault(r => r.Status < ResultStatus.OFFSET);
		}

		private void Select(FilePatcherViewModel f) => SelectItem(f);
		private void Select(ResultViewModel r) => SelectItem(r.File, r);

		private void SelectItem(params object[] heirachy) {
			var ic = (ItemsControl)treeView;
			foreach (var item in heirachy) {
				ic = (ItemsControl)ic.ItemContainerGenerator.ContainerFromItem(item);
				if (ic is TreeViewItem tvItem) {
					tvItem.IsExpanded = true;
					tvItem.BringIntoView();
				}
				if (ic == null) {
					Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => SelectItem(heirachy)));
					return;
				}
			}

			((TreeViewItem)ic).IsSelected = true;
		}

		#endregion

		private void SelectTab(TabItem tabItem) => Dispatcher.BeginInvoke(new Action(() => tabItem.IsSelected = true));

		private object _selectedTreeViewItem;
		private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			if (e.NewValue == _selectedTreeViewItem)
				return;

			if (e.OldValue != _selectedTreeViewItem)
				throw new Exception($"{nameof(_selectedTreeViewItem)} desync");

			if (ItemHasUserChanges) {
				if (ApproveOnExit() == MessageBoxResult.Cancel) {
					Dispatcher.BeginInvoke(new Action(() => {
						if (e.OldValue is FilePatcherViewModel)
							Select((FilePatcherViewModel)e.OldValue);
						else
							Select((ResultViewModel)e.OldValue);
					}));
					return;
				}

				//incase something happens during approval that overrides this
				if (treeView.SelectedItem != e.NewValue)
					return;
			}

			if (Result != null)
				Result.EditingPatch = null;

			_selectedTreeViewItem = e.NewValue;
			if (_selectedTreeViewItem is FilePatcherViewModel file)
				OnItemSelected(file);
			else
				OnItemSelected((ResultViewModel)_selectedTreeViewItem);
		}

		private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (e.RemovedItems.Count == 0)
				return;

			if (e.RemovedItems[0] == fileTab && filePanel.CanReDiff)
				RediffFileEditor();
			else if (e.RemovedItems[0] == patchTab && patchPanel.CanReDiff)
				RediffPatchEditor();
		}
		
		#region Result and Patch Functions

		// can save as long as 
		private bool CanSave => (File?.IsModified ?? false) && !ItemHasUserChanges;

		private bool ItemHasUserChanges => Result?.ModifiedInEditor ?? File?.ModifiedInEditor ?? false;
		private bool CanRevert => ItemHasUserChanges || (Result?.IsRemoved ?? false);

		private bool CanRediff => (fileTab.IsSelected ? filePanel : patchPanel).CanReDiff || !editorsInSync;

		private void OnItemSelected(FilePatcherViewModel f) => ReloadPanes(f, null, false);
		private void OnItemSelected(ResultViewModel r) => ReloadPanes(r.File, r, false);

		private void ReloadPanes(FilePatcherViewModel f, ResultViewModel r, bool linesChanged) {
			bool newFile = File != f;
			if (newFile) {
				if (File != null)
					File.PropertyChanged -= TrackPatchedLineChanges;
				if (f != null)
					f.PropertyChanged += TrackPatchedLineChanges;
			}

			if (Result != r && Result?.EditingPatch != null)
				throw new Exception("Leftover editing patch in result");

			File = f;
			Result = r;

			if (newFile || linesChanged) {
				filePanel.LoadDiff(File.BaseLines, File.PatchedLines);
				filePanel.SyntaxHighlighting =
					HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(File.BasePath));
			}

			if (Result == null) {
				titleLabel.DataContext = File;
				patchTab.Visibility = Visibility.Hidden;

				filePanel.ClearRangeMarkers();
				AddEditWarning();
				SelectTab(fileTab);
			}
			else {
				titleLabel.DataContext = Result;
				patchTab.Visibility = Visibility.Visible;

				Result.ModifiedInEditor = false;
				Result.EditingPatch = Result.AppliedPatch;
				ReloadEditingPatch();
				ReCalculateEditRange();
				filePanel.ScrollToMarked();
			}
		}

		private void TrackPatchedLineChanges(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == nameof(File.PatchedLines))
				ReloadPanes(File, Result, true);
		}

		private void AddEditWarning() {
			if (File.ResultsAreValuable) {
				var textArea = filePanel.right.editor.TextArea;
				textArea.ReadOnlySectionProvider = new EditWarningReadOnlySectionProvider(textArea);
			}
		}

		private void ReCalculateEditRange() {
			leftEditRange = new LineRange { start = 0, end = File.BaseLines.Length};
			rightEditRange = new LineRange { start = 0, end = File.PatchedLines.Length};

			int i = File.Results.ToList().IndexOf(Result);
			var prev = File.Results.Take(i).LastOrDefault(r => r.AppliedPatch != null);
			if (prev != null) {
				leftEditRange.start = prev.End1;
				rightEditRange.start = prev.End2;
			}

			var next = File.Results.Skip(i + 1).FirstOrDefault(r => r.AppliedPatch != null);
			if (next != null) {
				leftEditRange.end = next.Start1;
				rightEditRange.end = next.Start2;
			}

			filePanel.SetEditableRange(rightEditRange);

			var viewPatch = Result.ViewPatch;
			if (viewPatch != null && !(leftEditRange.Contains(viewPatch.Range1) && rightEditRange.Contains(viewPatch.Range2))) {
				var choice = new CustomMessageBox {
					Title = "Patch out of order",
					Message = "Patch does not fit between neighbours. Either patches overlap, or have different order to original file.",
					Image = MessageBoxImage.Error
				}.ShowDialogOk();
			}
		}

		private void ReloadEditingPatch(bool soft = false) {
			//mark patch ranges
			var p = Result.EditingPatch;
			if (p != null)
				filePanel.MarkRange(
					new LineRange { start = p.start1, length = p.length1},
					new LineRange { start = p.start2, length = p.length2});
			else if (Result.End1 <= File.BaseLines.Length) // mark the patch we were working on in the left hand side
				filePanel.MarkRange(
					new LineRange { start = Result.Start1, end = Result.End1 });
			else
				filePanel.ClearRangeMarkers();

			if (p == null) { //removed or failed, show the original patch in the patch panel
				p = new Patch(Result.OriginalPatch);
				p.start2 += Result.SearchOffset;
			}

			if (soft)
				patchPanel.ReplaceEditedLines(p.ToString().GetLines());
			else
				patchPanel.LoadDiff(
					Result.OriginalPatch.ToString().GetLines(),
					p.ToString().GetLines());

			//TODO bind this
			string patchEffect = Result.IsRemoved ? "Removed" : Result.Status == ResultStatus.FAILED ? "Failed" : "Applied";
			patchPanel.right.Title = patchEffect + " Patch";

			editorsInSync = true;
		}

		private void RediffFile() {
			File.Repatch(filePanel.Diff(), Patcher.Mode.OFFSET);
			// triggers ReloadPanes
		}

		private void RediffFileEditor() {
			filePanel.ReDiff();
			if (Result == null)
				return;

			//recalculate user patch
			try {
				Result.EditingPatch = filePanel.DiffEditableRange();
			}
			catch (InvalidOperationException e) {
				MessageBox.Show(this, e.Message, "Rediff Failed", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			ReloadEditingPatch();
		}

		private IEnumerable<string> PatchedLinesExcludingCurrentResult =>
			File.PatchedLines.Slice(new LineRange { start = 0, end = rightEditRange.start})
				.Concat(File.BaseLines.Slice(leftEditRange))
				.Concat(File.PatchedLines.Slice(new LineRange { start = rightEditRange.end, end = File.PatchedLines.Length}));

		private void RediffPatchEditor() {
			patchPanel.ReDiff();
			editorsInSync = false;

			var p = FormatAssistEditingPatch();
			//offset given other patches are already applied
			p.start1 += rightEditRange.start - leftEditRange.start;
			
			var patcher = new Patcher(new[] {p}, PatchedLinesExcludingCurrentResult);
			patcher.Patch(Patcher.Mode.FUZZY);

			var r = patcher.Results.Single();
			if (!r.success) {
				new CustomMessageBox {
					Title = "Patch Failed",
					Message = "Patch could not be applied, please ensure that the context is correct",
					Image = MessageBoxImage.Error
				}.ShowDialogOk();

				return;
			}

			p = r.appliedPatch;
			var appliedRange = p.TrimmedRange1;
			// calculate Result.AppliedPatch.Range2 to match PatchedLinesExcludingCurrentResult
			var keepoutRanges = new List<LineRange>();
			int delta = 0;
			foreach (var _result in File.Results) {
				if (_result == Result || !(_result.AppliedPatch is Patch applied))
					continue;

				var range = new LineRange { start = applied.start1 + delta, length = applied.length2 };
				keepoutRanges.Add(range);

				delta += applied.length2 - applied.length1;

				if (range.end <= appliedRange.start)
					p.start1 -= applied.length2 - applied.length1;
			}

			if (keepoutRanges.Any(range => range.Contains(appliedRange))) {
				new CustomMessageBox {
					Title = "Patch Failed",
					Message = $"Patch applied ({r.mode}) inside another patch {appliedRange}",
					Image = MessageBoxImage.Error
				}.ShowDialogOk("Ignore");
				
				return;
			}

			CustomMessageBox msgBox = null;
			if (!new LineRange { start = rightEditRange.start, length = leftEditRange.length}.Contains(appliedRange)) {
				msgBox = new CustomMessageBox {
					Title = "Moved Patch",
					Message = "Patch has been moved.\nLoad assisted patch?",
					Image = MessageBoxImage.Question
				};
			}
			if (r.mode == Patcher.Mode.FUZZY) {
				if (msgBox == null) {
					msgBox = new CustomMessageBox {
						Title = "Fuzzy Patch",
						Message = "Fuzzy patch mode was required to make the patch succeed.\nLoad assisted patch?",
						Image = MessageBoxImage.Question
					};
				}
				msgBox.Message += $" (Quality { (int)(r.fuzzyQuality * 100)})";
			}

			if (msgBox != null && msgBox.ShowDialogOkCancel("Load", "Undo") == MessageBoxResult.Cancel)
				return;

			Result.EditingPatch = p;
			
			filePanel.LoadDiff(File.BaseLines, patcher.ResultLines, true, false);
			ReloadEditingPatch(r.mode != Patcher.Mode.FUZZY);
			ReCalculateEditRange();
			filePanel.ScrollToMarked();
		}

		private MessageBoxResult ApproveUserPatch(bool remove = false) {
			if (!editorsInSync) {
				new CustomMessageBox {
					Title = "Cannot Approve",
					Message = $"Editors not in sync.",
					Image = MessageBoxImage.Error
				}.ShowDialogOk();
				return MessageBoxResult.Cancel;
			}

			if (remove) {
				Result.EditingPatch = null;
			}
			else if (Result.EditingPatch == null) {
				if (!Enumerable.SequenceEqual(filePanel.EditedLines, PatchedLinesExcludingCurrentResult))
					throw new Exception("Inconsistent state");

				var choice = new CustomMessageBox {
					Title = "Remove Empty Patch?",
					Message = "The approved patch is empty. Remove it?",
					Image = MessageBoxImage.Question
				}.ShowDialogOkCancel("Remove");

				if (choice == MessageBoxResult.Cancel)
					return MessageBoxResult.Cancel;
			}

			Result.Approve();


			// triggers reload of panes
			File.PatchedLines = (Result.EditingPatch != null ? filePanel.EditedLines : PatchedLinesExcludingCurrentResult).ToArray();

			if (CanSave && !File.ResultsAreValuable) {
				var save = new CustomMessageBox {
					Title = "Save?",
					Message = "All items requiring user review have been approved. Save patch and results?",
					Image = MessageBoxImage.Question
				}.ShowDialogOkCancel("Save");
				if (save == MessageBoxResult.OK)
					SaveFile();
			}

			return MessageBoxResult.OK;
		}

		private Patch FormatAssistEditingPatch() {
			var lines = patchPanel.EditedLines.ToArray();
			for (int i = 1; i < lines.Length; i++) {
				var l = lines[i];
				if (l.Length == 0 || l[0] != ' ' && l[0] != '-' && l[0] != '+')
					lines[i] = ' ' + l;
			}

			Patch p;
			try {
				p = PatchFile.FromLines(lines, false).patches[0];
			}
			catch (Exception e) {
				MessageBox.Show(this, e.Message, "Invalid Patch", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}

			p.RecalculateLength();
			patchPanel.ReplaceEditedLines(p.ToString().GetLines());
			return p;
		}

		private MessageBoxResult ApproveOnExit() {
			MessageBoxResult choice;
			Action yesAction;

			if (Result == null) {
				choice = new CustomMessageBox {
					Title = "Unapplied Changes",
					Message = "Rediff file to convert edits to patches?",
					Image = MessageBoxImage.Question
				}.ShowDialogYesNoCancel("Rediff", "Revert");

				yesAction = RediffFile;
			}
			else if (CanRediff) {
				choice = new CustomMessageBox {
					Title = "Unapproved Changes",
					Message = "Modifications would be lost. Rediff and approve?",
					Image = MessageBoxImage.Question
				}.ShowDialogYesNoCancel("Rediff", "Revert");
					
				if (choice == MessageBoxResult.Cancel)
					return MessageBoxResult.Cancel;

				if (choice == MessageBoxResult.Yes) {
					ExecuteRediff(null, null);
					if (CanRediff)//failed to sync
						return MessageBoxResult.Cancel;
				}
					
				yesAction = () => ApproveUserPatch();
			}
			else if (Result.EditingPatch == null) {
				choice = new CustomMessageBox {
					Title = "Unapproved Changes",
					Message = "Remove patch?",
					Image = MessageBoxImage.Question
				}.ShowDialogYesNoCancel("Remove", "Revert");
					
				yesAction = () => ApproveUserPatch(true);
			}
			else {
				choice = new CustomMessageBox {
					Title = "Unapproved Changes",
					Message = "Approve changes to patch?",
					Image = MessageBoxImage.Question
				}.ShowDialogYesNoCancel("Approve", "Revert");
					
				yesAction = () => ApproveUserPatch();
			}

			if (choice == MessageBoxResult.Yes)
				yesAction();
			else if (choice == MessageBoxResult.No)
				ExecuteRevert(null, null);

			return choice;
		}

		// Feature disabled
		/*private void RepatchFile() {
			var patches = File.results.Where(r => !IsRemoved(r)).Select(r => r.success ? r.appliedPatch : r.patch);
			Repatch(patches, Patcher.Mode.OFFSET);
			modifiedFiles.Add(File);
		}*/

		private void SaveFile()
		{
			if (File.ResultsAreValuable) {
				var choice = new CustomMessageBox {
					Title = "Unapproved patches",
					Message = "Any failed or fuzzy patches will be saved in their original state. Only the patch file will be saved",
					Image = MessageBoxImage.Warning
				}.ShowDialogOkCancel("Save");

				if (choice == MessageBoxResult.Cancel)
					return;
			}

			bool resultsWereValuable = File.ResultsAreValuable;
			File.SaveApprovedPatches(AutoHeaders);

			if (!File.Results.Any()) {
				new CustomMessageBox {
					Title = "Patch File Deleted",
					Message = "Patch file was deleted as all patches were removed.",
					Image = MessageBoxImage.Information
				}.ShowDialogOk();
			}

			if (!resultsWereValuable && File.ResultsAreValuable) {
				new CustomMessageBox {
					Title = "Save Failed",
					Message = "Some patches did not reapply properly after saving.",
					Image = MessageBoxImage.Warning
				}.ShowDialogOk();
				return;
			}
		}

		#endregion

		#region CommandBindings

		private void CanExecuteSave(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanSave;
		}

		private void ExecuteSave(object sender, ExecutedRoutedEventArgs e) {
			SaveFile();
		}

		private void CanExecuteReloadFile(object sender, CanExecuteRoutedEventArgs e) {
			if (File == null)
				return;

			//TODO: actually property change bind to the text editor
			File.ModifiedInEditor = filePanel.IsModified && Result == null;

			e.CanExecute = File.IsModified;
		}

		private void ExecuteReloadFile(object sender, ExecutedRoutedEventArgs e) {
			var choice = new CustomMessageBox {
				Title = "Abandon Changes?",
				Message = "Are you sure you want to revert all user changes?"
			}.ShowDialogOkCancel("Reload");

			if (choice == MessageBoxResult.Cancel)
				return;

			Result = null;
			File.Repatch(); // triggers ReloadPanes
			Select(File);
		}

		private void CanExecuteNextReviewItem(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = NextReviewItem(false) != null;
		}

		private void ExecuteNextReviewItem(object sender, ExecutedRoutedEventArgs e) {
			Select(NextReviewItem(false));
		}

		private void CanExecutePrevReviewItem(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = NextReviewItem(true) != null;
		}

		private void ExecutePrevReviewItem(object sender, ExecutedRoutedEventArgs e) {
			Select(NextReviewItem(true));
		}

		private void CanExecuteRediffFile(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}

		private void ExecuteRediffFile(object sender, ExecutedRoutedEventArgs e) {
			if (File.ResultsAreValuable) {
				var choice = new CustomMessageBox {
					Title = "Unapproved Patch Results",
					Message = "This file still has unapproved patch results. " +
					          "Editing in file mode will replace the results list, " +
					          "effectively approving the current patched file."
				}.ShowDialogOkCancel("Approve All");

				if (choice == MessageBoxResult.Cancel)
					return;
			}

			RediffFile();
		}

		private void CanExecuteRepatchFile(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}

		private void ExecuteRepatchFile(object sender, ExecutedRoutedEventArgs e) {
			new CustomMessageBox {
				Title = "Feature Disabled",
				Message = "Feature is probably redundant. Consider saving instead."
			}.ShowDialogOk("Cancel");

			/*if (FilePatcherViewModel.ResultsAreValuable) {
				var choice = new CustomMessageBox {
					Title = "Unapproved Patch Results",
					Message = "Are you sure you want to approve all patch results and repatch the file?"
				}.ShowDialogOkCancel("Approve All");

				if (choice == MessageBoxResult.Cancel)
					return;
			}

			RepatchFile();*/
		}

		private void CanExecuteRediff(object sender, CanExecuteRoutedEventArgs e) {
			if (fileTab == null)
				return;

			e.CanExecute = CanRediff;
		}

		private void ExecuteRediff(object sender, ExecutedRoutedEventArgs e) {
			if (fileTab.IsSelected)
				RediffFileEditor();
			else
				RediffPatchEditor();
		}

		private void CanExecuteRevert(object sender, CanExecuteRoutedEventArgs e) {
			if (fileTab == null)
				return;

			//TODO: actually property change bind to the text editor
			if (Result != null)
				Result.ModifiedInEditor |= filePanel.IsModified || patchPanel.IsModified;

			e.CanExecute = CanRevert;
		}

		private void ExecuteRevert(object sender, ExecutedRoutedEventArgs e) {
			if (Result?.IsRemoved ?? false)
				Result.UndoRemove();

			ReloadPanes(File, Result, true);
		}

		private void CanExecuteApprove(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = Result != null && !CanRediff && (Result.Status != ResultStatus.EXACT || Result.ModifiedInEditor);
		}

		private void ExecuteApprove(object sender, ExecutedRoutedEventArgs e) {
			if (filePanel.CanReDiff)
				RediffFileEditor();

			ApproveUserPatch();
		}

		private void CanExecuteRemove(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = Result != null && !Result.IsRemoved;
		}

		private void ExecuteRemove(object sender, ExecutedRoutedEventArgs e) {
			ApproveUserPatch(true);
		}

		#endregion

		private class EditWarningReadOnlySectionProvider : IReadOnlySectionProvider
		{
			private readonly TextArea textArea;
			private DateTime lastWarning;

			public EditWarningReadOnlySectionProvider(TextArea textArea) {
				this.textArea = textArea;
			}

			private bool Warning() {
				if ((DateTime.Now - lastWarning).TotalMilliseconds < 200)
					return false;

				var choice = new CustomMessageBox {
					Title = "Unapproved Patch Results",
					Message = "This file still has unapproved patch results. " +
					          "Editing in file mode will replace the results list, " +
					          "effectively approving the current patched file.",
					Image = MessageBoxImage.Warning
				}.ShowDialogOkCancel("Edit Anyway");

				if (choice == MessageBoxResult.OK) {
					textArea.ReadOnlySectionProvider = Util.FullyEditable();
					return true;
				}

				lastWarning = DateTime.Now;
				return false;
			}

			public bool CanInsert(int offset) {
				//ugly hack
				if (new StackFrame(1).GetMethod().DeclaringType?.Name != "ImeSupport")
					return Warning();

				return false;
			}

			public IEnumerable<ISegment> GetDeletableSegments(ISegment segment) {
				if (Warning())
					yield return segment;
			}
		}
	}
}