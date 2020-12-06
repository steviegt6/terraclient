using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using DiffPatch;

namespace PatchReviewer
{
	public class EditRangeProvider : LineRangeHighlighter, IReadOnlySectionProvider
	{
		public EditRangeProvider(TextEditor textEditor) : base(textEditor) {
			Brush = new SolidColorBrush(Colors.Gray);
			Brush.Freeze();
		}

		public bool CanInsert(int offset) {
			return startAnchor == null || offset >= startAnchor.Offset && offset <= endAnchor.Offset;
		}

		public IEnumerable<ISegment> GetDeletableSegments(ISegment segment) {
			if (startAnchor == null) {
				yield return segment;
				yield break;
			}

			if (segment.EndOffset <= startAnchor.Offset || segment.Offset >= endAnchor.Offset)
				yield break;

			var range = new LineRange {
				start = Math.Max(segment.Offset, startAnchor.Offset),
				end = Math.Min(segment.EndOffset, endAnchor.Offset)
			};
			yield return range.SimpleSegment();
		}
	}
}
