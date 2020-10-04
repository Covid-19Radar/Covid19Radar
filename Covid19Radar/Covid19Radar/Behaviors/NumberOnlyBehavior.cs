using System.Linq;
using Xamarin.Forms;

namespace Covid19Radar.Behaviors
{
    public class NumberOnlyBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                char[] newTextArray = args.NewTextValue.ToCharArray();
                bool isNumberOnly = newTextArray.All(x => char.IsDigit(x));
                if (isNumberOnly)
                {
                    ((Entry)sender).Text = args.NewTextValue;
                } else
                {
                    ((Entry)sender).Text = args.OldTextValue;
                }
            }
        }
    }
}