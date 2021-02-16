using System;
using System.IO;
using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(SkipBackup))]

namespace Covid19Radar.iOS.Services
{
	public class SkipBackup : ISkipBackup
	{
		void ISkipBackup.SkipBackup(string fileName)
		{
			// fileName
			string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string filePath  = Path.Combine(documents, fileName);

			if (!(File.Exists(filePath) && NSFileManager.GetSkipBackupAttribute(filePath))) {
				NSFileManager.SetSkipBackupAttribute(filePath, true);
			}
		}
	}
}
