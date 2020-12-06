using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using DiffPatch;

namespace PatchReviewer
{
	public static class Util
	{
		public class RangeSegment : ISegment
		{
			public int Offset { get; set; }
			public int Length { get; set; }
			public int EndOffset => Offset + Length;
		}

		public class FullyEditableReadOnlySectionProvider : IReadOnlySectionProvider
		{
			public bool CanInsert(int offset) => true;

			public IEnumerable<ISegment> GetDeletableSegments(ISegment segment) {
				yield return segment;
			}
		}

		public static string[] GetLines(this string s) {
			var arr = s.Split('\n');
			for (int i = 0; i < arr.Length; i++)
				arr[i] = arr[i].TrimEnd('\r');

			return arr;
		}

		private static readonly FieldInfo scrollViewerField =
			typeof(TextEditor).GetField("scrollViewer", BindingFlags.Instance | BindingFlags.NonPublic);

		public static ScrollViewer GetScrollViewer(this TextEditor textEditor) =>
			(ScrollViewer) scrollViewerField.GetValue(textEditor);

		public static ISegment SimpleSegment(this LineRange range) =>
			new RangeSegment {
				Offset = range.start,
				Length = range.length
			};

		//imported from DoubleUtil in WPF internals
		public static bool AreClose(double value1, double value2) {
			if (value1 == value2)
				return true;
			double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.22044604925031E-16;
			double num2 = value1 - value2;
			if (-num1 < num2)
				return num1 > num2;
			return false;
		}

		public static IReadOnlySectionProvider FullyEditable() => new FullyEditableReadOnlySectionProvider();
	}
}
