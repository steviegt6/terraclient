using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using DiffPatch;
using ICSharpCode.AvalonEdit.Search;

namespace PatchReviewer
{
	/// <summary>
	/// Interaction logic for SideBySide.xaml
	/// </summary>
	public partial class SideBySide
	{
		#region Dependency Properties

		public static readonly DependencyProperty LeftUnmatchedBrushProperty = DependencyProperty.Register(
			nameof(LeftUnmatchedBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush LeftUnmatchedBrush {
			get => (Brush) GetValue(LeftUnmatchedBrushProperty);
			set => SetValue(LeftUnmatchedBrushProperty, value);
		}

		public static readonly DependencyProperty RightUnmatchedBrushProperty = DependencyProperty.Register(
			nameof(RightUnmatchedBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush RightUnmatchedBrush {
			get => (Brush) GetValue(RightUnmatchedBrushProperty);
			set => SetValue(RightUnmatchedBrushProperty, value);
		}

		public static readonly DependencyProperty MatchedBrushProperty = DependencyProperty.Register(
			nameof(MatchedBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush MatchedBrush {
			get => (Brush) GetValue(MatchedBrushProperty);
			set => SetValue(MatchedBrushProperty, value);
		}

		public static readonly DependencyProperty MatchedHighlightBrushProperty = DependencyProperty.Register(
			nameof(MatchedHighlightBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush MatchedHighlightBrush {
			get => (Brush) GetValue(MatchedHighlightBrushProperty);
			set => SetValue(MatchedHighlightBrushProperty, value);
		}

		public static readonly DependencyProperty OffsetBrushProperty = DependencyProperty.Register(
			nameof(OffsetBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush OffsetBrush {
			get => (Brush) GetValue(OffsetBrushProperty);
			set => SetValue(OffsetBrushProperty, value);
		}

		public static readonly DependencyProperty PatchRangeBrushProperty = DependencyProperty.Register(
			nameof(PatchRangeBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush PatchRangeBrush {
			get => (Brush) GetValue(PatchRangeBrushProperty);
			set => SetValue(PatchRangeBrushProperty, value);
		}

		public static readonly DependencyProperty EditRangeBrushProperty = DependencyProperty.Register(
			nameof(EditRangeBrush), typeof(Brush), typeof(SideBySide), new PropertyMetadata(PropertyChanged));

		public Brush EditRangeBrush {
			get => (Brush) GetValue(EditRangeBrushProperty);
			set => SetValue(EditRangeBrushProperty, value);
		}

		private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var sbs = (SideBySide) d;
			switch (e.Property.Name) {
				case nameof(LeftUnmatchedBrush):
					sbs.leftMatchEditor.UnmatchedBrush = (Brush) e.NewValue;
					break;
				case nameof(RightUnmatchedBrush):
					sbs.rightMatchEditor.UnmatchedBrush = (Brush) e.NewValue;
					break;
				case nameof(MatchedBrush):
					sbs.leftMatchEditor.MatchedBrush = (Brush) e.NewValue;
					sbs.rightMatchEditor.MatchedBrush = (Brush) e.NewValue;
					break;
				case nameof(MatchedHighlightBrush):
					sbs.leftMatchEditor.MatchedHighlightBrush = (Brush) e.NewValue;
					sbs.rightMatchEditor.MatchedHighlightBrush = (Brush) e.NewValue;
					break;
				case nameof(OffsetBrush):
					sbs.leftMatchEditor.OffsetBrush = (Brush) e.NewValue;
					sbs.rightMatchEditor.OffsetBrush = (Brush) e.NewValue;
					break;
				case nameof(PatchRangeBrush):
					sbs.leftMatchEditor.LineRangeBrush = (Brush) e.NewValue;
					sbs.rightMatchEditor.LineRangeBrush = (Brush) e.NewValue;
					break;
				case nameof(EditRangeBrush):
					sbs.leftMatchEditor.EditRangeBrush = (Brush) e.NewValue;
					sbs.rightMatchEditor.EditRangeBrush = (Brush) e.NewValue;
					break;
			}
		}

		#endregion

		private readonly MatchedLineEditor leftMatchEditor;
		private readonly MatchedLineEditor rightMatchEditor;

		public IHighlightingDefinition SyntaxHighlighting {
			get => null;
			set {
				left.editor.SyntaxHighlighting = value;
				right.editor.SyntaxHighlighting = value;
			}
		}

		public bool IsModified => right.editor.IsModified;

		public SideBySide() {
			InitializeComponent();

			left.editor.IsReadOnly = true;
			left.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			PrepareEditors(left.editor, right.editor);
			PrepareEditors(right.editor, left.editor);

			(leftMatchEditor, rightMatchEditor) = MatchedLineEditor.Create(left.editor, right.editor);
		}

		private void PrepareEditors(TextEditor textEditor1, TextEditor textEditor2) {
			var options = textEditor1.TextArea.TextView.Options;
			options.ShowSpaces = true;
			options.ShowTabs = true;
			options.InheritWordWrapIndentation = false;
			textEditor1.ApplyTemplate();
			SearchPanel.Install(textEditor1);

			double requestedH = 0, requestedV = 0;
			textEditor1.GetScrollViewer().ScrollChanged += (sender, args) => {
				if (!Util.AreClose(requestedH, args.HorizontalOffset) || !Util.AreClose(requestedV, args.VerticalOffset)) {
					textEditor2.ScrollToHorizontalOffset(requestedH = args.HorizontalOffset);
					textEditor2.ScrollToVerticalOffset(requestedV = args.VerticalOffset);
				}
			};
		}
		


		public void SetTitles(string leftTitle, string rightTitle) {
			left.Title = leftTitle;
			right.Title = rightTitle;
		}

		public void ClearRangeMarkers() {
			leftMatchEditor.ClearRangeMarkers();
			rightMatchEditor.ClearRangeMarkers();
		}

		public void LoadDiff(IReadOnlyList<string> leftLines, IReadOnlyList<string> rightLines, bool underlyingChange = true, bool original = true) {
			var matchTree = MatchedLineTree.FromLines(leftLines, rightLines);
			leftMatchEditor.ReloadText(matchTree, leftLines, underlyingChange);
			rightMatchEditor.ReloadText(matchTree, rightLines, underlyingChange);

			if (underlyingChange && !original)
				rightMatchEditor.DiscardOriginalFileMarker();
		}

		public void MarkRange(LineRange leftRange, LineRange rightRange) {
			var range = LineRange.Union(leftMatchEditor.FromUnderlying(leftRange), rightMatchEditor.FromUnderlying(rightRange));
			leftMatchEditor.MarkRange(range);
			rightMatchEditor.MarkRange(range);
		}

		public void MarkRange(LineRange leftRange) {
			leftMatchEditor.MarkRange(leftMatchEditor.FromUnderlying(leftRange));
			rightMatchEditor.ClearRangeMarkers();
		}

		public void ScrollToMarked() {
			leftMatchEditor.ScrollToMarked();
		}

		public void SetEditableRange(LineRange rightRange) {
			rightMatchEditor.EditableRange = rightRange;
		}

		public IReadOnlyList<string> EditedLines => rightMatchEditor.UnderlyingLines;

		public void ReplaceEditedLines(IReadOnlyList<string> lines) => LoadDiff(leftMatchEditor.UnderlyingLines, lines, false);

		public bool CanReDiff => rightMatchEditor.LineTree != null && rightMatchEditor.ChangedSinceLoad;

		public void ReDiff() => LoadDiff(leftMatchEditor.UnderlyingLines, rightMatchEditor.UnderlyingLines, false);

		public List<Patch> Diff() {
			var lineTree = rightMatchEditor.LineTree;
			var lines1 = leftMatchEditor.UnderlyingLines;
			var lines2 = rightMatchEditor.UnderlyingLines;
			var matches = MatchedLineTree.ToMatches(lineTree, lines1.Count);
			return Differ.MakePatches(LineMatching.MakeDiffList(matches, lines1, lines2));
		}

		public Patch DiffEditableRange() {
			LineRange editRange;
			try {
				editRange = rightMatchEditor.EditableRange;
			}
			catch (Exception e) {
				throw new InvalidOperationException("Edit range markers have been lost.", e);
			}

			var lineTree = rightMatchEditor.LineTree;
			var leftTree = lineTree.Access(false);
			var rightTree = lineTree.Access(true);

			// grab context
			var firstNode = rightTree[editRange.first];
			while (firstNode.Prev != null && !firstNode.Prev.HasRightLine)
				firstNode = firstNode.Prev;
			while (firstNode.Prev != null && firstNode.Prev.SidesEqual)
				firstNode = firstNode.Prev;

			var lastNode = rightTree[editRange.last];
			while (lastNode.Next != null && !lastNode.Next.HasRightLine)
				lastNode = lastNode.Next;
			while (lastNode.Next != null && lastNode.Next.SidesEqual)
				lastNode = lastNode.Next;
			
			// extract matching from tree
			var range0 = new LineRange { first = lineTree.IndexOf(firstNode), last = lineTree.IndexOf(lastNode)};
			var range1 = new LineRange { first = leftTree.IndexOf(firstNode), last = leftTree.IndexOf(lastNode)};
			var range2 = new LineRange { first = rightTree.IndexOf(firstNode), last = rightTree.IndexOf(lastNode)};
			var matches = MatchedLineTree.ToMatches(firstNode.To(lastNode), range1.length);
			
			// create patch
			var lines1 = leftMatchEditor.GetLines(range0, true);
			var lines2 = rightMatchEditor.GetLines(range0, true);
			var patch = new Patch {
				start1 = range1.start, length1 = range1.length,
				start2 = range2.start, length2 = range2.length,
				diffs = LineMatching.MakeDiffList(matches, lines1, lines2)
			};
			patch.Trim(Differ.DefaultContext);
			return patch.length1 > 0 ? patch : null;
		}
	}
}