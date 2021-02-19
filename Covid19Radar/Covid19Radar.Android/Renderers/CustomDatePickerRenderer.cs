using Android.Content;
using Covid19Radar.Controls;
using Covid19Radar.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]

namespace Covid19Radar.Droid.Renderers
{
	public class CustomDatePickerRenderer : DatePickerRenderer
	{
		public CustomDatePickerRenderer(Context context) : base(context) { }
	}
}
