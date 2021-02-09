namespace Covid19Radar.Services
{
	public interface IEssentialsService
	{
		// DeviceInfo
		public string Platform        { get; }
		public string PlatformVersion { get; }
		public string Model           { get; }
		public string DeviceType      { get; }

		// AppInfo
		public string AppVersion  { get; }
		public string BuildNumber { get; }
	}
}
