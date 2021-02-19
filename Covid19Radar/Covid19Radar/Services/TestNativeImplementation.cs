using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
	public class TestNativeImplementation : INativeImplementation
	{
		private static readonly Random _random = new Random();

		private Task WaitRandom()
		{
			return Task.Delay(_random.Next(100, 2500));
		}

		public async Task StartAsync()
		{
			await this.WaitRandom();
			Preferences.Set("fake_enabled", true);
		}

		public async Task StopAsync()
		{
			await this.WaitRandom();
			Preferences.Set("fake_enabled", true);
		}

		public async Task<bool> IsEnabledAsync()
		{
			await this.WaitRandom();
			return Preferences.Get("fake_enabled", true);
		}

		public async Task<IEnumerable<TemporaryExposureKey>> GetSelfTemporaryExposureKeysAsync()
		{
			var keys = new List<TemporaryExposureKey>();
			for (int i = 1; i < 14; ++i) {
				keys.Add(GenerateRandomKey(i));
			}
			await this.WaitRandom();
			return keys;
		}

		public Task<Status> GetStatusAsync()
		{
			return Task.FromResult(Preferences.Get("fake_enabled", true) ? Status.Active : Status.Disabled);
		}

		public Task<(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getInfo)> DetectExposuresAsync(IEnumerable<string> files)
		{
			return Task.FromResult<(ExposureDetectionSummary, Func<Task<IEnumerable<ExposureInfo>>>)>((
				new ExposureDetectionSummary(10, 2, 5),
				() => Task.FromResult<IEnumerable<ExposureInfo>>(new List<ExposureInfo>() {
					new ExposureInfo(DateTime.UtcNow.AddDays(-10), TimeSpan.FromMinutes(15), 65, 5, RiskLevel.Medium),
					new ExposureInfo(DateTime.UtcNow.AddDays(-11), TimeSpan.FromMinutes(5),  40, 3, RiskLevel.Low),
				})
			));
		}

		private static TemporaryExposureKey GenerateRandomKey(int daysAgo)
		{
			byte[] buffer = new byte[16];
			_random.NextBytes(buffer);
			return new(
				buffer,
				DateTimeOffset.UtcNow.AddDays(-1 * daysAgo),
				TimeSpan.FromMinutes(_random.Next(5, 120)),
				((RiskLevel)(_random.Next(1, 8)))
			);
		}
	}
}
