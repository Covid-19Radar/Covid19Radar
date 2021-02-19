using Covid19Radar.Controls;
using Covid19Radar.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]

namespace Covid19Radar.iOS.Renderers
{
	public class CustomDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);
			if (!(e.NewElement is null) && !(this.Control is null) &&
				this.Control.InputView is UIDatePicker datePicker) {
				datePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
			}
		}
	}
}
