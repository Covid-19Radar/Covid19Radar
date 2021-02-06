using System;
using Xamarin.Forms;

namespace Covid19Radar.Renderers
{
	public class CustomEntry : Entry
	{
		public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
			nameof(BorderColor),
			typeof(Color),
			typeof(CustomEntry),
			Color.Default
		);

		/// <summary>
		///  Border Color
		/// </summary>
		public Color BorderColor
		{
			get => (this.GetValue(BorderColorProperty) as Color?) ?? default;
			set => this.SetValue(BorderColorProperty, value);
		}

		public event EventHandler? DeleteClicked;

		public void TriggerDeleteClicked()
		{
			this.OnDeleteClicked(EventArgs.Empty);
		}

		protected virtual void OnDeleteClicked(EventArgs e)
		{
			this.DeleteClicked?.Invoke(this, e);
		}
	}
}
