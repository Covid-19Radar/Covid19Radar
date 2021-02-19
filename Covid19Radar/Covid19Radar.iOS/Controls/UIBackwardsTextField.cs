using System;
using UIKit;

#nullable disable

namespace Covid19Radar.iOS.Controls
{
	public class UIBackwardsTextField : UITextField
	{
		// A delegate type for hooking up change notifications.
		public delegate void DeleteBackwardEventHandler(object sender, EventArgs e);

		// An event that clients can use to be notified whenever the
		// elements of the list change.
		public event DeleteBackwardEventHandler OnDeleteBackward;

		public void OnDeleteBackwardPressed()
		{
			this.OnDeleteBackward?.Invoke(null, null);
		}

		public UIBackwardsTextField()
		{
			this.BorderStyle   = UITextBorderStyle.RoundedRect;
			this.ClipsToBounds = true;
		}

		public override void DeleteBackward()
		{
			base.DeleteBackward();
			this.OnDeleteBackwardPressed();
		}
	}
}
