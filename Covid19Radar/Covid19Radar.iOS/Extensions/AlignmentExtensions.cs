using UIKit;
using Xamarin.Forms;

namespace Covid19Radar.iOS.Extensions
{
	internal static class AlignmentExtensions
	{
		internal static UITextAlignment ToNativeTextAlignment(this TextAlignment alignment, EffectiveFlowDirection flowDirection)
		{
			bool isLtr = flowDirection.IsLeftToRight();
			return alignment switch {
				TextAlignment.Center => UITextAlignment.Center,
				TextAlignment.End    => isLtr ? UITextAlignment.Right : UITextAlignment.Left,
				_                    => isLtr ? UITextAlignment.Left  : UITextAlignment.Right
			};
		}

		internal static UIControlContentVerticalAlignment ToNativeTextAlignment(this TextAlignment alignment)
		{
			return alignment switch {
				TextAlignment.Center => UIControlContentVerticalAlignment.Center,
				TextAlignment.End    => UIControlContentVerticalAlignment.Bottom,
				_                    => UIControlContentVerticalAlignment.Top,
			};
		}
	}
}
