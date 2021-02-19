using Covid19Radar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotifyOtherPage : ContentPage
	{
		public NotifyOtherPage()
		{
			this.InitializeComponent();
		}

		private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
		{
			if (sender is RadioButton rb && this.BindingContext is NotifyOtherPageViewModel vm) {
				vm.OnClickRadioButtonIsTrueCommand(rb.Text);
			}
		}
	}
}
