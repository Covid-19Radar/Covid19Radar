namespace Covid19Radar.Services.Logs
{
	public interface ILogPathDependencyService
	{
		public string LogsDirPath            { get; }
		public string LogUploadingTmpPath    { get; }
		public string LogUploadingPublicPath { get; }
	}
}
