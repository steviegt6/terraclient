using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using DiffPatch;

namespace PatchReviewer
{
	public class LineRangeHighlighter : IBackgroundRenderer
	{
		private readonly TextView textView;
		private readonly ScrollViewer scroller;

		protected TextAnchor startAnchor;
		protected TextAnchor endAnchor;

		public bool Valid => startAnchor != null && endAnchor != null && !startAnchor.IsDeleted && !endAnchor.IsDeleted;

		public Brush Brush { get; set; }
		public Pen BorderPen { get; set; }
		public float LineHeight { get; set; }

		public LineRangeHighlighter(TextEditor textEditor) {
			scroller = textEditor.GetScrollViewer();
			textView = textEditor.TextArea.TextView;

			BorderPen = new Pen();
			BorderPen.Freeze();

			Brush = new SolidColorBrush(Colors.Black);
			Brush.Freeze();

			LineHeight = 2;
		}

		public KnownLayer Layer => KnownLayer.Background;

		//line coordinates are 1 indexed
		public LineRange LineRange {
			get => new LineRange {first = startAnchor.Line, last = endAnchor.Line};
			set {
				var doc = textView.Document;

				startAnchor = doc.CreateAnchor(doc.GetLineByNumber(value.first).Offset);
				startAnchor.MovementType = AnchorMovementType.BeforeInsertion;

				endAnchor = doc.CreateAnchor(doc.GetLineByNumber(value.last).EndOffset);

				textView.InvalidateLayer(Layer);
			}
		}

		public void ScrollTo() {
			if (!Valid)
				return;

			var top = textView.GetVisualPosition(new TextViewPosition(startAnchor.Location), VisualYPosition.LineTop).Y;
			var bottom = textView.GetVisualPosition(new TextViewPosition(endAnchor.Location), VisualYPosition.LineBottom).Y;

			var windowCenter = scroller.VerticalOffset + scroller.ViewportHeight / 2;
			var targetCenter = (top + bottom) / 2;

			if (bottom - top < scroller.ViewportHeight - 40)
				scroller.ScrollToVerticalOffset(targetCenter - scroller.ViewportHeight / 2);
			else if (windowCenter < targetCenter) //scroller down
				scroller.ScrollToVerticalOffset(top - 20);
			else
				scroller.ScrollToVerticalOffset(bottom - scroller.ViewportHeight + 20);
		}

		public void Clear() {
			startAnchor = endAnchor = null;
			textView.InvalidateLayer(Layer);
		}

		public void Draw(TextView textView, DrawingContext drawingContext) {
			if (!Valid)
				return;
			
			var builder = new BackgroundGeometryBuilder();
			var line1 = textView.GetVisualLine(startAnchor.Line);
			if (line1 != null)
				builder.AddRectangle(textView, new Rect(0, 
					line1.VisualTop - textView.ScrollOffset.Y, 
					textView.ActualWidth, LineHeight));
			
			var line2 = textView.GetVisualLine(endAnchor.Line);
			if (line2 != null) {
				var y = line2.GetTextLineVisualYPosition(line2.TextLines.Last(), VisualYPosition.LineBottom);
				builder.AddRectangle(textView, new Rect(0, 
					y - textView.ScrollOffset.Y - LineHeight,
					textView.ActualWidth, LineHeight));
			}
			
			var geometry = builder.CreateGeometry();
			if (geometry != null)
				drawingContext.DrawGeometry(Brush, BorderPen, geometry);
		}
	}
}
