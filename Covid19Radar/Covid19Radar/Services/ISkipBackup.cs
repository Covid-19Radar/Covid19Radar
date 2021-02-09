namespace Covid19Radar.Services
{
	public interface ISkipBackup
	{
		/// <remarks>only iOS</remarks>
		public void SkipBackup(string fileName);
	}
}
