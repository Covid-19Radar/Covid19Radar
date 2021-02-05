﻿using System;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
	public interface IUserDataService
	{
		event EventHandler<UserDataModel?>? UserDataChanged;

		bool IsExistUserData { get; }

		Task<UserDataModel?> RegisterUserAsync();
		UserDataModel? Get();
		Task SetAsync(UserDataModel userData);
		Task ResetAllDataAsync();
	}

	/// <summary>
	/// This service registers, retrieves, stores, and automatically updates user data.
	/// </summary>
	public class UserDataService : IUserDataService
	{
		private readonly ILoggerService loggerService;
		private readonly IHttpDataService httpDataService;
		private UserDataModel? current;
		public event EventHandler<UserDataModel?>? UserDataChanged;

		public UserDataService(IHttpDataService httpDataService, ILoggerService loggerService)
		{
			this.httpDataService = httpDataService;
			this.loggerService = loggerService;
			current = this.Get();
		}

		public bool IsExistUserData => current != null;

		public async Task<UserDataModel?> RegisterUserAsync()
		{
			loggerService.StartMethod();
			var userData = await httpDataService.PostRegisterUserAsync();
			if (userData == null)
			{
				loggerService.Info("userData is null");
				loggerService.EndMethod();
				return null;
			}
			loggerService.Info("userData is not null");
			userData.StartDateTime = DateTime.UtcNow;
			userData.IsExposureNotificationEnabled = false;
			userData.IsNotificationEnabled = false;
			userData.IsOptined = false;
			userData.IsPolicyAccepted = false;
			userData.IsPositived = false;
			await this.SetAsync(userData);

			loggerService.EndMethod();
			return userData;
		}

		public UserDataModel? Get()
		{
			loggerService.StartMethod();

			bool existsUserData = Application.Current.Properties.ContainsKey(AppConstants.StorageKey.UserData);
			loggerService.Info($"existsUserData: {existsUserData}");
			if (existsUserData)
			{
				loggerService.EndMethod();
				return Utils.DeserializeFromJson<UserDataModel>(Application.Current.Properties[AppConstants.StorageKey.UserData].ToString());
			}

			loggerService.EndMethod();
			return null;
		}

		public async Task SetAsync(UserDataModel userData)
		{
			loggerService.StartMethod();

			string newdata     = Utils.SerializeToJson(userData);
			string currentdata = Utils.SerializeToJson(current ?? new UserDataModel());
			if (currentdata.Equals(newdata))
			{
				loggerService.Info("currentdata equals newdata");
				loggerService.EndMethod();
				return;
			}
			//await SecureStorage.SetAsync(AppConstants.StorageKey.UserData, newdata);
			loggerService.Info("currentdata don't equals newdata");
			Application.Current.Properties[AppConstants.StorageKey.UserData] = newdata;
			current = this.Get();
			UserDataChanged?.Invoke(this, current);

			loggerService.EndMethod();
		}

		public async Task ResetAllDataAsync()
		{
			loggerService.StartMethod();

			Application.Current.Properties.Remove(AppConstants.StorageKey.UserData);
			current = null;
			await Application.Current.SavePropertiesAsync();

			//SecureStorage.Remove(AppConstants.StorageKey.UserData);
			//SecureStorage.Remove(AppConstants.StorageKey.Secret);

			loggerService.EndMethod();
		}
	}

}
