using Xamarin.Forms;

namespace Covid19Radar.Services.Logs
{
	public interface ILogPeriodicDeleteService
	{
		public void Init();
	}

	public class LogPeriodicDeleteService : ILogPeriodicDeleteService
	{
		public void Init()
		{
			DependencyService.Get<ILogPeriodicDeleteService>().Init();
		}
	}
}
