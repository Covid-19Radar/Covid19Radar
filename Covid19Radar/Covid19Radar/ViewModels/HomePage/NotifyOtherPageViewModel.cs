using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class NotifyOtherPageViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;
		private          string?          _diagnosis_uid;
		private          bool             _is_enabled;
		private          bool             _is_visible_with_symptoms_layout;
		private          bool             _is_visible_no_symptoms_layout;
		private          DateTime         _diagnosis_date;
		private          int              _error_count;
		private          int              _max_error_count;

		public string? DiagnosisUid
		{
			get => _diagnosis_uid;
			set
			{
				this.SetProperty(ref _diagnosis_uid, value);
				this.IsEnabled = this.CheckRegisterButtonEnable();
			}
		}

		public bool IsEnabled
		{
			get => _is_enabled;
			set => this.SetProperty(ref _is_enabled, value);
		}

		public bool IsVisibleWithSymptomsLayout
		{
			get => _is_visible_with_symptoms_layout;
			set
			{
				this.SetProperty(ref _is_visible_with_symptoms_layout, value);
				this.IsEnabled = this.CheckRegisterButtonEnable();
			}
		}

		public bool IsVisibleNoSymptomsLayout
		{
			get => _is_visible_no_symptoms_layout;
			set
			{
				this.SetProperty(ref _is_visible_no_symptoms_layout, value);
				this.IsEnabled = this.CheckRegisterButtonEnable();
			}
		}

		public DateTime DiagnosisDate
		{
			get => _diagnosis_date;
			set => this.SetProperty(ref _diagnosis_date, value);
		}

		public Command OnClickRegister => new Command(async () => {
			_logger.StartMethod();
			if (!await UserDialogs.Instance.ConfirmAsync(
				AppResources.NotifyOtherPageDiag1Message,
				AppResources.NotifyOtherPageDiag1Title,
				AppResources.ButtonAgree,
				AppResources.ButtonCancel)) {
				await UserDialogs.Instance.AlertAsync(
					AppResources.NotifyOtherPageDiag2Message,
					string.Empty,
					AppResources.ButtonOk
				);
				_logger.Info($"Cancelled by user.");
				_logger.EndMethod();
				return;
			}
			UserDialogs.Instance.ShowLoading(AppResources.LoadingTextRegistering);

			// Check helthcare authority positive api check here!!
			if (_error_count >= _max_error_count) {
				await UserDialogs.Instance.AlertAsync(
					AppResources.NotifyOtherPageDiagAppClose,
					AppResources.NotifyOtherPageDiagErrorTitle,
					AppResources.ButtonOk
				);
				UserDialogs.Instance.HideLoading();
				DependencyService.Get<ICloseApplication>().CloseApplication();
				_logger.Error($"Tried {_error_count + 1} times but all failed. The max attempts count is {_max_error_count}.");
				_logger.EndMethod();
				return;
			}

			_logger.Info($"The count of attempts to submit a diagnostic number is {_error_count + 1} of {_max_error_count}.");
			if (_error_count > 0) {
				await UserDialogs.Instance.AlertAsync(AppResources.NotifyOtherPageDiag3Message,
					AppResources.NotifyOtherPageDiag3Title + $"{_error_count + 1}/{_max_error_count}",
					AppResources.ButtonOk
				);
				await Task.Delay(_error_count * 5000);
			}

			// Init Dialog
			if (string.IsNullOrEmpty(_diagnosis_uid)) {
				await UserDialogs.Instance.AlertAsync(
					AppResources.NotifyOtherPageDiag4Message,
					AppResources.NotifyOtherPageDiagErrorTitle,
					AppResources.ButtonOk
				);
				++_error_count;
				if (_user_data is null) {
					_logger.Warning("The user data is null.");
				} else {
					await _user_data_service.SetAsync(_user_data);
				}
				UserDialogs.Instance.HideLoading();
				_logger.Error($"No diagnostic number entered.");
				_logger.EndMethod();
				return;
			}

			var regex = new Regex(AppConstants.PositiveRegex);
			if (!regex.IsMatch(_diagnosis_uid)) {
				await UserDialogs.Instance.AlertAsync(
					AppResources.NotifyOtherPageDiag5Message,
					AppResources.NotifyOtherPageDiagErrorTitle,
					AppResources.ButtonOk
				);
				++_error_count;
				if (_user_data is null) {
					_logger.Warning("The user data is null.");
				} else {
					await _user_data_service.SetAsync(_user_data);
				}
				UserDialogs.Instance.HideLoading();
				_logger.Error("The entered diagnostic number does not match the format.");
				_logger.EndMethod();
				return;
			}

			// Submit the UID
			try {
				// EN Enabled Check
				if (!await ExposureNotification.IsEnabledAsync()) {
					await UserDialogs.Instance.AlertAsync(
						AppResources.NotifyOtherPageDiag6Message,
						AppResources.NotifyOtherPageDiag6Title,
						AppResources.ButtonOk
					);
					UserDialogs.Instance.HideLoading();
					var task2 = this.NavigationService?.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
					if (!(task2 is null)) {
						await task2;
					}
					_logger.Warning($"The exposure notification is disabled.");
					_logger.EndMethod();
					return;
				}

				if (_user_data is null) {
					++_error_count;
					UserDialogs.Instance.Alert(
						AppResources.NotifyOtherPageDialogExceptionText,
						AppResources.ButtonFailed,
						AppResources.ButtonOk
					);
					_logger.Error("The user data is null!");
					_logger.EndMethod();
					return;
				}
				if (this.ExposureNotificationService is null) {
					++_error_count;
					UserDialogs.Instance.Alert(
						AppResources.NotifyOtherPageDialogExceptionText,
						AppResources.ButtonFailed,
						AppResources.ButtonOk
					);
					_logger.Error("The exposure notification service is null.");
					_logger.EndMethod();
					return;
				}

				// Set the submitted UID
				_user_data.AddDiagnosis(_diagnosis_uid, new DateTimeOffset(DateTime.Now));
				await _user_data_service.SetAsync(_user_data);

				_logger.Info("Submitting the diagnostic number...");

				// Submit our diagnosis
				this.ExposureNotificationService.DiagnosisDate = _diagnosis_date;
				await ExposureNotification.SubmitSelfDiagnosisAsync();
				UserDialogs.Instance.HideLoading();
				await UserDialogs.Instance.AlertAsync(
					AppResources.NotifyOtherPageDialogSubmittedText,
					AppResources.ButtonComplete,
					AppResources.ButtonOk
				);
				var task = this.NavigationService?.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
				if (!(task is null)) {
					await task;
				}
				_logger.Info($"Submitted the diagnostic number successfully");
			} catch (InvalidDataException ide) {
				++_error_count;
				UserDialogs.Instance.Alert(
					AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFound,
					AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFoundTitle,
					AppResources.ButtonOk
				);
				_logger.Exception("Failed to submit UID with invalid data.", ide);
			} catch (Exception e) {
				++_error_count;
				UserDialogs.Instance.Alert(
					AppResources.NotifyOtherPageDialogExceptionText,
					AppResources.ButtonFailed,
					AppResources.ButtonOk
				);
				_logger.Exception("Failed to the submit UID.", e);
			} finally {
				UserDialogs.Instance.HideLoading();
				_logger.EndMethod();
			}
		});

		public NotifyOtherPageViewModel(
			INavigationService          navigationService,
			ExposureNotificationService exposureNotificationService,
			ILoggerService              logger,
			IUserDataService            userDataService)
			: base(navigationService, exposureNotificationService)
		{
			_logger            = logger;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
			_error_count       = 0;
			_max_error_count   = AppConstants.MaxErrorCount;
			this.Title         = AppResources.TitleUserStatusSettings;
			this.DiagnosisUid  = string.Empty;
			this.DiagnosisDate = DateTime.Today;
		}

		public void OnClickRadioButtonIsTrueCommand(string text)
		{
			_logger.StartMethod();
			this.IsVisibleWithSymptomsLayout = AppResources.NotifyOtherPageRadioButtonYes == text;
			this.IsVisibleNoSymptomsLayout   = AppResources.NotifyOtherPageRadioButtonYes != text && AppResources.NotifyOtherPageRadioButtonNo == text;
			_logger.Info($"Is visible with symptoms layout: {this.IsVisibleWithSymptomsLayout}");
			_logger.Info($"Is visible no symptoms layout: {this.IsVisibleNoSymptomsLayout}");
			_logger.EndMethod();
		}

		public bool CheckRegisterButtonEnable()
		{
			return _diagnosis_uid?.Length == AppConstants.MaxDiagnosisUidCount
				&& (_is_visible_with_symptoms_layout || _is_visible_no_symptoms_layout);
		}
	}
}
