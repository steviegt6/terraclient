using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using DiffPatch;

namespace PatchReviewer
{
	internal class MatchedLineEditor : IBackgroundRenderer
	{
		public static readonly Color MatchedBackground = Colors.LightBlue;
		public static readonly Color MatchedHighlightBackground = Color.FromRgb(191, 239, 255);
		public static readonly Color UnmatchedBackground = Colors.Orange;
		public static readonly Color OffsetBackground = Colors.LightGray;
		
		public Brush MatchedBrush;
		public Brush UnmatchedBrush;
		public Brush MatchedHighlightBrush;
		public Brush OffsetBrush;
		public Pen BorderPen;

		public Brush LineRangeBrush {
			get => rangeMarker.Brush;
			set => rangeMarker.Brush = value;
		}

		public Brush EditRangeBrush {
			get => editRangeMarker.Brush;
			set => editRangeMarker.Brush = value;
		}

		private MatchedLineTree lineTree;
		public MatchedLineTree LineTree {
			get => lineTree;
			set {
				lineTree = value;
				if (lineTree != null && lineTree.Count != currentDocument.LineCount)
					throw new ArgumentException("Line mismatch");
			}
		}

		private readonly bool side;
		private MatchedLineEditor otherEditor;

		private readonly TextEditor textEditor;
		private readonly TextArea textArea;
		private readonly TextView textView;
		private readonly UndoStack undoStack;
		private TextDocument currentDocument;

		private readonly LineRangeHighlighter rangeMarker;
		private readonly EditRangeProvider editRangeMarker;
		private int changeCountSinceLoad;

		private MatchedLineEditor(TextEditor textEditor, UndoStack undoStack, bool side) {
			this.side = side;
			this.textEditor = textEditor;
			textArea = textEditor.TextArea;
			textView = textArea.TextView;

			//set undoStack and rebind textEditor.IsModified property
			this.undoStack = undoStack;
			undoStack.PropertyChanged += (o, e) => {
				if (e.PropertyName == "IsOriginalFile")
					textEditor.IsModified = !undoStack.IsOriginalFile;
			};

			//init pens and brushes
			BorderPen = new Pen();
			MatchedBrush = new SolidColorBrush(MatchedBackground);
			MatchedHighlightBrush = new SolidColorBrush(MatchedHighlightBackground);
			UnmatchedBrush = new SolidColorBrush(UnmatchedBackground);
			OffsetBrush = new SolidColorBrush(OffsetBackground);
			BorderPen.Freeze();
			MatchedBrush.Freeze();
			MatchedHighlightBrush.Freeze();
			UnmatchedBrush.Freeze();
			OffsetBrush.Freeze();

			//visual events
			textView.BackgroundRenderers.Add(this);
			textView.BackgroundRenderers.Add(rangeMarker = new LineRangeHighlighter(textEditor));
			textView.LineTransformers.Add(new DiffTextColorizer(this));
			
			editRangeMarker = new EditRangeProvider(textEditor);
			if (!textEditor.IsReadOnly)
				textView.BackgroundRenderers.Add(editRangeMarker);

			textView.DocumentChanged += (sender, args) => AttachToDocument();

			//add custom line number margin
			var lineNumbers = new MatchedLineNumberMargin(this);
			var line = (Line)DottedLineMargin.Create();
			textArea.LeftMargins.Insert(0, lineNumbers);
			textArea.LeftMargins.Insert(1, line);
			var lineNumbersForeground = new Binding("LineNumbersForeground") { Source = textEditor };
			lineNumbers.SetBinding(Control.ForegroundProperty, lineNumbersForeground);
			line.SetBinding(Shape.StrokeProperty, lineNumbersForeground);
		}

		#region Public Interface
		public static (MatchedLineEditor left, MatchedLineEditor right) Create(TextEditor leftEditor, TextEditor rightEditor) {
			var undoStack = new UndoStack();
			var left = new MatchedLineEditor(leftEditor, undoStack, false);
			var right = new MatchedLineEditor(rightEditor, undoStack, true);
			left.otherEditor = right;
			right.otherEditor = left;
			left.AttachToDocument();
			right.AttachToDocument();
			return (left, right);
		}

		public IReadOnlyList<string> UnderlyingLines => GetUnderlyingLines(LineTree, currentDocument.Text.GetLines());

		public IReadOnlyList<string> GetLines(LineRange lineRange, bool underlying) {
			var segment = GetSegment(lineRange);
			IReadOnlyList<string> lines = currentDocument.GetText(segment.start, segment.length).GetLines();

			if (underlying)
				lines = GetUnderlyingLines(LineTree.Slice(lineRange), lines);

			return lines;
		}

		public void ReloadText(MatchedLineTree matchTree, IReadOnlyList<string> underlyingLines, bool underlyingChange) {
			var newText = BuildEditorText(matchTree, underlyingLines);
			rangeMarker.Clear();

			if (underlyingChange) {
				editRangeMarker.Clear();
				if (!textEditor.IsReadOnly)
					textArea.ReadOnlySectionProvider = Util.FullyEditable();

				LineTree = null;
				textEditor.Text = newText;
				LineTree = matchTree;
				undoStack.MarkAsOriginalFile();
			}
			else {
				if (LineTree == null)
					throw new ArgumentException("!underlyingChange without LineTree");

				if (underlyingLines.Count != LineTree.Access(side).Count)
					throw new ArgumentException("!underlyingChange with different line count");

				LineRange? editRange = null;
				if (editRangeMarker.Valid)
					editRange = EditableRange;
				
				//using character replace keeps the caret position somewhat reasonable
				//in the future, we could generate a proper offset change map, and even make undo supported, but it's not worth the time at the moment
				LineTree = null;
				currentDocument.Replace(0, currentDocument.TextLength, newText, OffsetChangeMappingType.CharacterReplace);
				currentDocument.UndoStack.ClearAll();
				LineTree = matchTree;

				if (editRange.HasValue)
					EditableRange = editRange.Value;
			}

			changeCountSinceLoad = 0;
		}

		public void DiscardOriginalFileMarker() => undoStack.DiscardOriginalFileMarker();

		public LineRange FromUnderlying(LineRange range) => range.Map(LineTree.Access(side).CombinedIndexOf);

		public void MarkRange(LineRange range) {
			rangeMarker.LineRange = range + 1;
		}

		public void ScrollToMarked() {
			rangeMarker.ScrollTo();
		}

		public LineRange EditableRange {
			get {
				var r = editRangeMarker.LineRange - 1;
				if (r.first > 0) {
					var node = LineTree[r.first - 1];
					if (!node.HasLine(side))
						throw new InvalidOperationException("Edit range must follow an underlying line");

					r.first = LineTree.Access(side).IndexOf(node) + 1;
				}
			
				if (r.end < LineTree.Count) {
					var node = LineTree[r.end];
					if (!node.HasLine(side))
						throw new InvalidOperationException("Edit range must be followed by an underlying line");

					r.end = LineTree.Access(side).IndexOf(node);
				}
				else
					r.end = LineTree.Access(side).Count;

				return r;
			}
			set {
				var r = value;
				if (r.start > 0)
					r.first = LineTree.Access(side).CombinedIndexOf(r.first - 1) + 1;

				if (r.end < LineTree.Access(side).Count)
					r.last = LineTree.Access(side).CombinedIndexOf(r.last + 1) - 1;
				else
					r.end = LineTree.Access(side).Count;

				editRangeMarker.LineRange = r + 1;
				textArea.ReadOnlySectionProvider = editRangeMarker;
			}
		}

		public void ClearRangeMarkers() {
			rangeMarker.Clear();
			editRangeMarker.Clear();
		}

		public bool ChangedSinceLoad => changeCountSinceLoad != 0;
		#endregion

		private void AttachToDocument() {
			if (LineTree != null)
				throw new Exception("Document changed with LineTree attached");

			if (currentDocument != null) {
				currentDocument.Changing -= TextChanging;
				currentDocument.Changed -= TextChanged;
				currentDocument.TextChanged -= TextChanged;
			}

			currentDocument = textView.Document;

			currentDocument.Changing += TextChanging;
			currentDocument.Changed += TextChanged;
			currentDocument.TextChanged += TextChanged;
			currentDocument.UndoStack = undoStack;
		}

		private LineRange GetSegment(LineRange lineRange) => new LineRange {
			start = currentDocument.GetLineByNumber(lineRange.first + 1).Offset,
			end = currentDocument.GetLineByNumber(lineRange.last + 1).EndOffset
		};

		private LineRange GetLineRange(LineRange range) => new LineRange {
			first = currentDocument.GetLineByOffset(range.start).LineNumber - 1,
			last = currentDocument.GetLineByOffset(range.end).LineNumber - 1
		};

		private IReadOnlyList<string> GetUnderlyingLines(IEnumerable<MatchedLineNode> nodes, IReadOnlyList<string> textList) {
			var list = new List<string>();
			int i = 0;
			foreach (var node in nodes) {
				if (node.HasLine(side))
					list.Add(textList[i]);

				i++;
			}

			return list;
		}

		private string BuildEditorText(IEnumerable<MatchedLineNode> nodes, IReadOnlyList<string> lines) {
			int l = 0;
			return string.Join(Environment.NewLine, nodes.Select(line => line.HasLine(side) ? lines[l++] : ""));
		}

		private void InternalReplaceLines(LineRange range, string text) => InternalReplace(GetSegment(range), text);

		private void InternalReplace(LineRange range, string text) {
			ignoreChangeEvents = true;
			currentDocument.Replace(range.start, range.length, text, OffsetChangeMappingType.CharacterReplace);
			ignoreChangeEvents = false;
		}

		#region TextChange Events
		private bool ignoreChangeEvents;
		private bool isChanging;
		private LineRange changingLines;
		private LineRange changedSegment; //TextView offsets
		private void TextChanging(object sender, DocumentChangeEventArgs e) {
			if (LineTree == null || ignoreChangeEvents || !undoStack.AcceptChanges)
				return;

			var range = new LineRange { start = e.Offset, length = e.RemovalLength};
			var lineRange =  GetLineRange(range);
			if (!isChanging) {
				changingLines = lineRange;
				return;
			}

			if (!changedSegment.Contains(range)) {
				changingLines = LineRange.Union(changingLines, lineRange);
				//throw new InvalidOperationException("Multi-range change");
			}
		}
		
		private void TextChanged(object sender, DocumentChangeEventArgs e) {
			if (LineTree == null || ignoreChangeEvents || !undoStack.AcceptChanges)
				return;
			
			var range = new LineRange { start = e.Offset, length = e.InsertionLength};
			range = new LineRange {
				start = currentDocument.GetLineByOffset(range.start).Offset,
				end = currentDocument.GetLineByOffset(range.end).EndOffset
			};
			if (isChanging) {
				changedSegment = new LineRange {
					start = e.GetNewOffset(changedSegment.start, AnchorMovementType.BeforeInsertion),
					end = e.GetNewOffset(changedSegment.end, AnchorMovementType.AfterInsertion)
				};
				changedSegment = LineRange.Union(changedSegment, range);
			}
			else {
				changedSegment = range;
				isChanging = true;
			}
		}

		private void TextChanged(object sender, EventArgs e) {
			if (undoStack.AcceptChanges) {
				changeCountSinceLoad++;
				undoStack.PushOptional(new ModifyChangeCount(this));
			}

			if (!isChanging)
				return;

			isChanging = false;
			
			var newLineRange = GetLineRange(changedSegment);
			var newLines = GetLines(newLineRange, false);

			//complete deletion of lines
			if (changedSegment.length == 0 && LineTree.Slice(changingLines).Any(n => !n.HasLine(side)))
				newLines = new string[0];

			//absorb unmatched lines into changing range when inserting up to length of insertion
			while (newLineRange.length > changingLines.length &&
			       changingLines.last+1 < LineTree.Count && !LineTree[changingLines.last+1].HasLine(side)) {
				changingLines.end++;
				newLineRange.end++;
			}

			var otherLines = otherEditor.GetLines(changingLines, true);
			var tree = MatchedLineTree.FromLines(side ? otherLines : newLines, side ? newLines : otherLines);
			
			InternalReplaceLines(newLineRange, BuildEditorText(tree, newLines));
			otherEditor.InternalReplaceLines(changingLines, otherEditor.BuildEditorText(tree, otherLines));

			var op = new TreeEditOperation(this, changingLines, tree);
			op.Redo();
			currentDocument.UndoStack.Push(op);
		}

		#endregion

		public KnownLayer Layer => KnownLayer.Background;

		public void Draw(TextView textView, DrawingContext drawingContext) {
			if (LineTree == null)
				return;

			var matchedBuilder = new BackgroundGeometryBuilder();
			var unmatchedBuilder = new BackgroundGeometryBuilder();
			var offsetBuilder = new BackgroundGeometryBuilder();
			var builders = new[] {
				(matchedBuilder, MatchedBrush),
				(unmatchedBuilder, UnmatchedBrush),
				(offsetBuilder, OffsetBrush)
			};

			foreach (var vLine in textView.VisualLines) {
				var ml = LineTree[vLine.FirstDocumentLine.LineNumber - 1];

				BackgroundGeometryBuilder builder;
				if (!ml.HasLine(side))
					builder = offsetBuilder;
				else if (!ml.HasLine(!side))
					builder = unmatchedBuilder;
				else if (!ml.SidesEqual)
					builder = ml.DiffRanges(side) != null ? matchedBuilder : unmatchedBuilder;
				else
					continue;

				var linePosY = vLine.VisualTop - textView.ScrollOffset.Y;
				builder.AddRectangle(textView, new Rect(0, linePosY, textView.ActualWidth, vLine.Height));
			}

			foreach (var (builder, brush) in builders) {
				var geometry = builder.CreateGeometry();
				if (geometry != null) drawingContext.DrawGeometry(brush, BorderPen, geometry);
			}
		}

		private class TreeEditOperation : IUndoableOperation
		{
			private readonly MatchedLineEditor editor;
			private readonly LineRange oldRange;
			private readonly LineRange newRange;
			private readonly IReadOnlyList<MatchedLineNode> oldNodes;
			private readonly IReadOnlyList<MatchedLineNode> newNodes;

			private MatchedLineTree LineTree => editor.LineTree;

			public TreeEditOperation(MatchedLineEditor editor, LineRange range, IReadOnlyList<MatchedLineNode> newNodes) {
				this.editor = editor;
				this.oldRange = range;
				this.newNodes = newNodes;

				newRange = new LineRange { start = oldRange.start, length = newNodes.Count};
				oldNodes = LineTree.Slice(oldRange).ToArray();
			}

			private void Apply(LineRange range, IReadOnlyList<MatchedLineNode> nodes) {
				var first = LineTree[range.first];
				var last = LineTree[range.last];
				LineTree.InsertRange(last, nodes.Select(n => n.GetCopy));
				LineTree.RemoveRange(first, last);
			}

			public void Undo() => Apply(newRange, oldNodes);
			public void Redo() => Apply(oldRange, newNodes);
		}

		private class DiffTextColorizer : DocumentColorizingTransformer
		{
			private readonly MatchedLineEditor editor;

			public DiffTextColorizer(MatchedLineEditor editor) {
				this.editor = editor;
			}

			protected override void ColorizeLine(DocumentLine line) {
				var diffs = editor.LineTree?[line.LineNumber - 1].DiffRanges(editor.side);
				if (diffs == null)
					return;

				try {
					foreach (var range in diffs)
						ChangeLinePart(line.Offset + range.start, line.Offset + range.end,
							e => e.BackgroundBrush = editor.MatchedHighlightBrush);
				}
				catch (Exception) {
					ChangeLinePart(line.Offset, line.EndOffset, e => e.BackgroundBrush = new SolidColorBrush(Colors.Purple));
				}
			}
		}

		private class ModifyChangeCount : IUndoableOperation
		{
			private readonly MatchedLineEditor editor;

			public ModifyChangeCount(MatchedLineEditor editor) {
				this.editor = editor;
			}

			public void Redo() {
				editor.changeCountSinceLoad++;
			}

			public void Undo() {
				editor.changeCountSinceLoad--;
			}
		}

		private class MatchedLineNumberMargin : LineNumberMargin
		{
			private readonly MatchedLineEditor editor;

			public MatchedLineNumberMargin(MatchedLineEditor editor) {
				this.editor = editor;
			}

			protected override void OnRender(DrawingContext drawingContext) {
				if (TextView == null || !TextView.VisualLinesValid)
					return;

				var foreground = (Brush)GetValue(Control.ForegroundProperty);
				foreach (VisualLine line in TextView.VisualLines) {
					int lineNumber = line.FirstDocumentLine.LineNumber;
					if (editor.LineTree != null) {
						var node = editor.LineTree[lineNumber - 1];
						if (!node.HasLine(editor.side))
							continue;

						lineNumber = editor.LineTree.Access(editor.side).IndexOf(node) + 1;
					}

#pragma warning disable CS0618 // Type or member is obsolete
					FormattedText text = new FormattedText(
						lineNumber.ToString(CultureInfo.CurrentCulture),
						CultureInfo.CurrentCulture,
						FlowDirection.LeftToRight,
						typeface,
						emSize,
						foreground,
						null,
						TextOptions.GetTextFormattingMode(this)
					);
#pragma warning restore CS0618 // Type or member is obsolete

					double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextTop);
					drawingContext.DrawText(text, new Point(RenderSize.Width - text.Width, y - TextView.VerticalOffset));
				}
			}
		}
	}
}