using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace Covid19Radar.Droid.Services
{
	public class CloseApplication : ICloseApplication
	{
		void ICloseApplication.CloseApplication()
		{
			var activity = Platform.CurrentActivity;
			activity.FinishAffinity();
		}
	}
}
