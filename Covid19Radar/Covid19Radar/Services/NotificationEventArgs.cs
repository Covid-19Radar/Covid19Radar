using System;

namespace Covid19Radar.Services
{
	public class NotificationEventArgs : EventArgs
	{
		public string Title   { get; set; }
		public string Message { get; set; }

		public NotificationEventArgs()
		{
			this.Title   = string.Empty;
			this.Message = string.Empty;
		}
	}
}
