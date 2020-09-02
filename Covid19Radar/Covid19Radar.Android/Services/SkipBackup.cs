using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(SkipBackup))]
namespace Covid19Radar.Droid.Services
{
    public class SkipBackup : ISkipBackup
    {
        public void skipBackup(string fileName)
        {
            // only iOS , for android see Android.manifest
        }
    }
}