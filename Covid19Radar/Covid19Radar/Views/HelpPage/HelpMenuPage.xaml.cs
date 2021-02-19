using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HelpMenuPage : ContentPage
	{
		public HelpMenuPage()
		{
			this.InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			list_view.SelectedItem = null;
		}
	}
}
