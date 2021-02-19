using System.Linq;
using Xamarin.Forms;

namespace Covid19Radar.Behaviors
{
	public class NumberOnlyBehavior : Behavior<Entry>
	{
		protected override void OnAttachedTo(Entry entry)
		{
			base.OnAttachedTo(entry);
			entry.TextChanged += OnEntryTextChanged;
		}

		protected override void OnDetachingFrom(Entry entry)
		{
			base.OnDetachingFrom(entry);
			entry.TextChanged -= OnEntryTextChanged;
		}

		private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
		{
			if (!string.IsNullOrWhiteSpace(args.NewTextValue) && sender is Entry entry) {
				char[] newTextArray = args.NewTextValue.ToCharArray();
				bool   isNumberOnly = newTextArray.All(x => char.IsDigit(x));
				if (isNumberOnly) {
					entry.Text = args.NewTextValue;
				} else {
					entry.Text = args.OldTextValue;
				}
			}
		}
	}
}
