using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace Covid19Radar.Droid.Services
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.FinishAffinity();
        }
    }
}