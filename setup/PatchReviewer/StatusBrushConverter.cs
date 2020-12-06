using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PatchReviewer
{
	public class StatusBrushConverter : IValueConverter
	{
		/*#region Styling Properties

		public static readonly DependencyProperty OffsetPatchBrushProperty = DependencyProperty.Register(
			nameof(OffsetPatchBrush), typeof(Brush), typeof(ReviewWindow));

		public Brush OffsetPatchBrush {
			get => (Brush)GetValue(OffsetPatchBrushProperty);
			set => SetValue(OffsetPatchBrushProperty, value);
		}

		public static readonly DependencyProperty GoodPatchBrushProperty = DependencyProperty.Register(
			nameof(GoodPatchBrush), typeof(Brush), typeof(ReviewWindow));

		public Brush GoodPatchBrush {
			get => (Brush)GetValue(GoodPatchBrushProperty);
			set => SetValue(GoodPatchBrushProperty, value);
		}

		public static readonly DependencyProperty WarningPatchBrushProperty = DependencyProperty.Register(
			nameof(WarningPatchBrush), typeof(Brush), typeof(ReviewWindow));

		public Brush WarningPatchBrush {
			get => (Brush)GetValue(WarningPatchBrushProperty);
			set => SetValue(WarningPatchBrushProperty, value);
		}

		public static readonly DependencyProperty BadPatchBrushProperty = DependencyProperty.Register(
			nameof(BadPatchBrush), typeof(Brush), typeof(ReviewWindow));

		public Brush BadPatchBrush {
			get => (Brush)GetValue(BadPatchBrushProperty);
			set => SetValue(BadPatchBrushProperty, value);
		}

		public static readonly DependencyProperty FailPatchBrushProperty = DependencyProperty.Register(
			nameof(FailPatchBrush), typeof(Brush), typeof(ReviewWindow));

		public Brush FailPatchBrush {
			get => (Brush)GetValue(FailPatchBrushProperty);
			set => SetValue(FailPatchBrushProperty, value);
		}

		#endregion*/

		public Brush OffsetPatchBrush { get; set; }
		public Brush GoodPatchBrush { get; set; }
		public Brush WarningPatchBrush { get; set; }
		public Brush BadPatchBrush { get; set; }
		public Brush FailPatchBrush { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var status = (ResultStatus)value;

			switch (status) {
				case ResultStatus.EXACT:
					return Brushes.Transparent;
				case ResultStatus.OFFSET:
					return OffsetPatchBrush;
				case ResultStatus.GOOD:
					return GoodPatchBrush;
				case ResultStatus.WARNING:
					return WarningPatchBrush;
				case ResultStatus.BAD:
					return BadPatchBrush;
				case ResultStatus.FAILED:
					return FailPatchBrush;
				default:
					throw new ArgumentException("ResultStatus: " + status);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
