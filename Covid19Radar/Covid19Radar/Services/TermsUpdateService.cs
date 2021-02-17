using System;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
	public enum TermsType
	{
		TermsOfService,
		PrivacyPolicy
	}

	public interface ITermsUpdateService
	{
		public ValueTask<TermsUpdateInfoModel> GetTermsUpdateInfo();

		public bool IsReAgree(TermsType privacyType, TermsUpdateInfoModel privacyUpdateInfo);

		public ValueTask SaveLastUpdateDateAsync(TermsType privacyType, DateTime updateDate);
	}

	public class TermsUpdateService : ITermsUpdateService
	{
		private const    string                      TermsOfServiceLastUpdateDateKey = "TermsOfServiceLastUpdateDateTime";
		private const    string                      PrivacyPolicyLastUpdateDateKey  = "PrivacyPolicyLastUpdateDateTime";
		private readonly ILoggerService              _logger;
		private readonly IApplicationPropertyService _app_prop;

		public TermsUpdateService(ILoggerService loggerService, IApplicationPropertyService applicationPropertyService)
		{
			_logger   = loggerService;
			_app_prop = applicationPropertyService;
		}

		public async ValueTask<TermsUpdateInfoModel> GetTermsUpdateInfo()
		{
			_logger.StartMethod();
			string uri = AppResources.UrlTermsUpdate;
			using (var client = new HttpClient()) {
				try {
					string json = await client.GetStringAsync(uri);
					_logger.Info($"URI: {uri}");
					_logger.Info($"TermsUpdateInfo: {json}");
					var obj = JsonConvert.DeserializeObject<TermsUpdateInfoModel>(json);
					_logger.EndMethod();
					return obj;
				} catch (Exception e) {
					_logger.Exception("Failed to get a terms update info.", e);
					_logger.EndMethod();
					return new();
				}
			}
		}

		public bool IsReAgree(TermsType privacyType, TermsUpdateInfoModel termsUpdateInfo)
		{
			_logger.StartMethod();
			TermsUpdateInfoModel.Detail? info = null;
			string                       key  = string.Empty;
			switch (privacyType) {
			case TermsType.TermsOfService:
				info = termsUpdateInfo.TermsOfService;
				key  = TermsOfServiceLastUpdateDateKey;
				break;
			case TermsType.PrivacyPolicy:
				info = termsUpdateInfo.PrivacyPolicy;
				key  = PrivacyPolicyLastUpdateDateKey;
				break;
			}
			if (info is null) {
				_logger.EndMethod();
				return false;
			}
			var lastUpdate = new DateTime();
			if (_app_prop.ContainsKey(key)) {
				lastUpdate = ((DateTime)(_app_prop.GetProperties(key)));
			}
			_logger.Info($"The privacy type: {privacyType}, the last update: {lastUpdate}, the update: {info.UpdateDateTime}");
			_logger.EndMethod();
			return lastUpdate < info.UpdateDateTime;
		}

		public async ValueTask SaveLastUpdateDateAsync(TermsType termsType, DateTime updateDate)
		{
			_logger.StartMethod();
			string key = termsType == TermsType.TermsOfService ? TermsOfServiceLastUpdateDateKey : PrivacyPolicyLastUpdateDateKey;
			await _app_prop.SavePropertiesAsync(key, updateDate);
			_logger.EndMethod();
		}
	}
}
