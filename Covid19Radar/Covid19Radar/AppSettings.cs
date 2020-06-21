﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;


namespace Covid19Radar
{
    public class AppSettings
    {
        static AppSettings instance;

        public static AppSettings Instance
            => instance ??= new AppSettings();

        public AppSettings()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var file = assembly.GetManifestResourceStream("Covid19Radar.settings.json");
            using var sr = new StreamReader(file);
            var json = sr.ReadToEnd();
            var j = JObject.Parse(json);

            AppVersion = j.Value<string>("appVersion");
            LicenseUrl = j.Value<string>("licenseUrl");
            ShareAppUrl = j.Value<string>("shareAppUrl");
            ApiUrlBase = j.Value<string>("apiUrlBase");
            ApiSecret = j.Value<string>("apiSecret");
            CdnUrlBase = j.Value<string>("cdnUrlBase");
            BlobStorageContainerName = j.Value<string>("blobStorageContainerName");
            SupportedRegions = j.Value<string>("supportedRegions").ToUpperInvariant().Split(';', ',', ':');
            AndroidSafetyNetApiKey = j.Value<string>("androidSafetyNetApiKey");
            SupportEmail = j.Value<string>("supportEmail");

        }
        public string SupportEmail { get; }
        public string AppVersion { get; }
        public string LicenseUrl { get; }
        public string ApiUrlBase { get; }
        public string ApiSecret { get; }
        public string ShareAppUrl { get; }
        public string CdnUrlBase { get; }


        public string[] SupportedRegions { get; }

        public string BlobStorageContainerName { get; }

        public string AndroidSafetyNetApiKey { get; }

        internal Dictionary<string, ulong> GetDefaultDefaultBatch() =>
            Instance.SupportedRegions.ToDictionary(r => r, r => (ulong)0);
    }
}
