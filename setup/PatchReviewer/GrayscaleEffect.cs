using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PatchReviewer
{
	//http://bursjootech.blogspot.com.au/2008/06/grayscale-effect-pixel-shader-effect-in.html
	public class GrayscaleEffect : ShaderEffect
	{
		private static PixelShader _pixelShader = new PixelShader { UriSource = GetResourcePackUri("GrayscaleEffect.ps") };

		public static Uri GetResourcePackUri(string resourceFileName) {
			var assemblyShortName = typeof(GrayscaleEffect).Assembly.ToString().Split(',')[0];

			string uriString = "pack://application:,,,/" + assemblyShortName + ";component/" + resourceFileName;
			return new Uri(uriString);
		}

		public GrayscaleEffect() {
			PixelShader = _pixelShader;
			
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(DesaturationFactorProperty);
		}

		public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);
		public Brush Input {
			get => (Brush)GetValue(InputProperty);
			set => SetValue(InputProperty, value);
		}
		
		public static readonly DependencyProperty DesaturationFactorProperty = DependencyProperty.Register("DesaturationFactor", typeof(double), typeof(GrayscaleEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceDesaturationFactor));
		public double DesaturationFactor {
			get => (double)GetValue(DesaturationFactorProperty);
			set => SetValue(DesaturationFactorProperty, value);
		}

		private static object CoerceDesaturationFactor(DependencyObject d, object value) {
			GrayscaleEffect effect = (GrayscaleEffect)d;
			double newFactor = (double)value;

			if (newFactor < 0.0 || newFactor > 1.0) {
				return effect.DesaturationFactor;
			}

			return newFactor;
		}
	}
}
