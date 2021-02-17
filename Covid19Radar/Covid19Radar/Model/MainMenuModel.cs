using System.ComponentModel;

namespace Covid19Radar.Model
{
	public class MainMenuModel : INotifyPropertyChanged
	{
		private       string                       _icon_color;
		private       string                       _text_color;
		public  event PropertyChangedEventHandler? PropertyChanged;

		public string? Title    { get; set; }
		public string? Icon     { get; set; }
		public string? PageName { get; set; }

		public string IconColor
		{
			get => _icon_color;
			set
			{
				_icon_color ??= string.Empty;
				if (_icon_color != value) {
					_icon_color = value;
					this.OnPropertyChanged(nameof(this.IconColor));
				}
			}
		}

		public string TextColor
		{
			get => _text_color;
			set
			{
				_text_color ??= string.Empty;
				if (_text_color != value) {
					_text_color = value;
					this.OnPropertyChanged(nameof(this.TextColor));
				}
			}
		}

		public MainMenuModel()
		{
			_icon_color = string.Empty;
			_text_color = string.Empty;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new(propertyName));
		}
	}
}
