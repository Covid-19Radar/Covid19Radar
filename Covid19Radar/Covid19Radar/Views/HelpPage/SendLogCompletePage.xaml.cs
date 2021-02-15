using Covid19Radar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SendLogCompletePage : ContentPage
	{
		public SendLogCompletePage()
		{
			this.InitializeComponent();
		}

		protected override bool OnBackButtonPressed()
		{
			if (this.BindingContext is SendLogCompletePageViewModel model) {
				model.OnBackButtonPressedCommand.Execute(null);
			}
			return true;
		}
	}
}
