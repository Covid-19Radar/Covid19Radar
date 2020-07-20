using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Foundation;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SkipBackup))]
namespace Covid19Radar.iOS.Services
{
    public class SkipBackup : ISkipBackup
    {
        public void skipBackup(string fileName)
        {
            // fileName
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documents, fileName);

            if (!(File.Exists(filePath) && NSFileManager.GetSkipBackupAttribute(filePath)))
            {
                NSFileManager.SetSkipBackupAttribute(filePath, true);
            }
        }
    }
}