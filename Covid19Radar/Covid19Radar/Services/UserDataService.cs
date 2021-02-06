using System;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
	public interface IUserDataService
	{
		public event EventHandler<UserDataModel?>? UserDataChanged;
		public       bool                          IsExistUserData { get; }
		public       Task<UserDataModel?>          RegisterUserAsync();
		public       UserDataModel?                Get();
		public       Task                          SetAsync(UserDataModel userData);
		public       Task                          ResetAllDataAsync();
	}

	/// <summary>
	///  This service registers, retrieves, stores, and automatically updates user data.
	/// </summary>
	public class UserDataService : IUserDataService
	{
		private readonly ILoggerService                _logger;
		private readonly IHttpDataService              _http_data;
		private          UserDataModel?                _current;
		public  event    EventHandler<UserDataModel?>? UserDataChanged;

		public UserDataService(IHttpDataService httpDataService, ILoggerService logger)
		{
			_logger    = logger;
			_http_data = httpDataService;
			_current   = this.Get();
		}

		public bool IsExistUserData => _current != null;

		public async Task<UserDataModel?> RegisterUserAsync()
		{
			_logger.StartMethod();
			var userData = await _http_data.PostRegisterUserAsync();
			if (userData == null)
			{
				_logger.Info("userData is null");
				_logger.EndMethod();
				return null;
			}
			_logger.Info("userData is not null");
			userData.StartDateTime = DateTime.UtcNow;
			userData.IsExposureNotificationEnabled = false;
			userData.IsNotificationEnabled = false;
			userData.IsOptined = false;
			userData.IsPolicyAccepted = false;
			userData.IsPositived = false;
			await this.SetAsync(userData);

			_logger.EndMethod();
			return userData;
		}

		public UserDataModel? Get()
		{
			_logger.StartMethod();
			if (Application.Current.Properties.TryGetValue(AppConstants.StorageKey.UserData, out object config)) {
				_logger.Info("The user data exists.");
				_logger.EndMethod();
				return JsonConvert.DeserializeObject<UserDataModel>(config.ToString());
			}
			_logger.Warning("The user data does not exists.");
			_logger.EndMethod();
			return null;
		}

		public Task SetAsync(UserDataModel userData)
		{
			_logger.StartMethod();

			string newdata     = JsonConvert.SerializeObject(userData);
			string currentdata = JsonConvert.SerializeObject(_current);
			if (currentdata.Equals(newdata))
			{
				_logger.Info("currentdata equals newdata");
				_logger.EndMethod();
				return Task.CompletedTask;
			}
			//await SecureStorage.SetAsync(AppConstants.StorageKey.UserData, newdata);
			_logger.Info("currentdata don't equals newdata");
			Application.Current.Properties[AppConstants.StorageKey.UserData] = newdata;
			_current = this.Get();
			UserDataChanged?.Invoke(this, _current);

			_logger.EndMethod();
			return Task.CompletedTask;
		}

		public async Task ResetAllDataAsync()
		{
			_logger.StartMethod();

			Application.Current.Properties.Remove(AppConstants.StorageKey.UserData);
			_current = null;
			await Application.Current.SavePropertiesAsync();

			//SecureStorage.Remove(AppConstants.StorageKey.UserData);
			//SecureStorage.Remove(AppConstants.StorageKey.Secret);

			_logger.EndMethod();
		}
	}
}
