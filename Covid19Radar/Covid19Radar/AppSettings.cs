using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Covid19Radar
{
	public sealed class AppSettings
	{
		public static AppSettings Instance                 { get; } = new AppSettings();
		public        string      AppVersion               { get; }
		public        string      ApiSecret                { get; }
		public        string      ApiKey                   { get; }
		public        string      ApiUrlBase               { get; }
		public        string[]    SupportedRegions         { get; }
		public        string      BlobStorageContainerName { get; }
		public        string      AndroidSafetyNetApiKey   { get; }
		public        string      CdnUrlBase               { get; }
		public        string      AppStoreUrl              { get; }
		public        string      GooglePlayUrl            { get; }
		public        string      SupportEmail             { get; }
		public        string      LicenseUrl               { get; }
		public        string      LogStorageEndpoint       { get; }
		public        string      LogStorageContainerName  { get; }
		public        string      LogStorageAccountName    { get; }

		private AppSettings()
		{
			JObject json;
			using (var file = Assembly.GetExecutingAssembly().GetManifestResourceStream("Covid19Radar.settings.json"))
			using (var sr   = new StreamReader(file)) {
				json = JObject.Parse(sr.ReadToEnd());
			}
			this.AppVersion               = json.Value<string>("appVersion");
			this.ApiSecret                = json.Value<string>("apiSecret");
			this.ApiKey                   = json.Value<string>("apiKey");
			this.ApiUrlBase               = json.Value<string>("apiUrlBase");
			this.SupportedRegions         = json.Value<string>("supportedRegions").ToUpperInvariant().Split(';', ',', ':');
			this.BlobStorageContainerName = json.Value<string>("blobStorageContainerName");
			this.AndroidSafetyNetApiKey   = json.Value<string>("androidSafetyNetApiKey");
			this.CdnUrlBase               = json.Value<string>("cdnUrlBase");
			this.LicenseUrl               = json.Value<string>("licenseUrl");
			this.AppStoreUrl              = json.Value<string>("appStoreUrl");
			this.GooglePlayUrl            = json.Value<string>("googlePlayUrl");
			this.SupportEmail             = json.Value<string>("supportEmail");
			this.LogStorageEndpoint       = json.Value<string>("logStorageEndpoint");
			this.LogStorageContainerName  = json.Value<string>("logStorageContainerName");
			this.LogStorageAccountName    = json.Value<string>("logStorageAccountName");
		}

		internal Dictionary<string, ulong> GetDefaultBatch()
		{
			return this.SupportedRegions.ToDictionary(r => r, _ => 0UL);
		}
	}
}
