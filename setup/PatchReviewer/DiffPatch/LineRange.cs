using System;
using System.Collections.Generic;
using System.Linq;

namespace DiffPatch
{
	public struct LineRange
	{
		public int start, end;

		public int length {
			get => end - start;
			set => end = start + value;
		}

		public int last {
			get => end - 1;
			set => end = value + 1;
		}

		public int first {
			get => start;
			set => start = value;
		}
		
		public LineRange Map(Func<int, int> f) => new LineRange {start = f(start), end = f(end)};

		public bool Contains(int i) => start <= i && i < end;
		public bool Contains(LineRange r) => r.start >= start && r.end <= end;
		public bool Intersects(LineRange r) => r.start < end || r.end > start;

		public override string ToString() => "[" + start + "," + end + ")";

		public static bool operator ==(LineRange r1, LineRange r2) => r1.start == r2.start && r1.end == r2.end;
		public static bool operator !=(LineRange r1, LineRange r2) => r1.start != r2.start || r1.end != r2.end;

		public static LineRange operator +(LineRange r, int i) => new LineRange {start = r.start + i, end = r.end + i};
		public static LineRange operator -(LineRange r, int i) => new LineRange {start = r.start - i, end = r.end - i};

		public static LineRange Union(LineRange r1, LineRange r2) => new LineRange {
			start = Math.Min(r1.start, r2.start),
			end = Math.Max(r1.end, r2.end)
		};

		public static LineRange Intersection(LineRange r1, LineRange r2) => new LineRange {
			start = Math.Max(r1.start, r2.start),
			end = Math.Min(r1.end, r2.end)
		};

		public IEnumerable<LineRange> Except(IEnumerable<LineRange> except, bool presorted = false) {
			if (!presorted)
				except = except.OrderBy(r => r.start);

			int start = this.start;
			foreach (var r in except) {
				if (r.start - start > 0)
					yield return new LineRange { start = start, end = r.start };

				start = r.end;
			}
			
			if (this.end - start > 0)
				yield return new LineRange { start = start, end = this.end };
		}
	}
}
