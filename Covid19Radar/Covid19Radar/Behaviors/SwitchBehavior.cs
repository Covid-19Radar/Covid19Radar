using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Covid19Radar.Behaviors
{
	public class SwitchBehavior : Behavior<Switch>
	{
		/// <summary>
		/// The command property.
		/// </summary>
		public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SwitchBehavior), null);

		/// <summary>
		/// Gets or sets the command.
		/// </summary>
		/// <value>The command.</value>
		public ICommand? Command
		{
			get => this.GetValue(CommandProperty) as ICommand;
			set => this.SetValue(CommandProperty, value);
		}

		/// <summary>
		/// Gets the bindable.
		/// </summary>
		/// <value>The bindable.</value>
		public Switch? Bindable { get; private set; }

		/// <summary>
		/// On attached to.
		/// </summary>
		/// <param name="bindable">The bindable component.</param>
		/// <exception cref="System.ArgumentNullException"/>
		protected override void OnAttachedTo(Switch bindable)
		{
			if (bindable is null) {
				throw new ArgumentNullException(nameof(bindable));
			}
			base.OnAttachedTo(bindable);
			this.Bindable = bindable;
			this.Bindable.BindingContextChanged += this.Bindable_BindableOnBindingContextChanged;
			this.Bindable.Toggled               += this.Bindable_OnSwitchToggled;
		}

		/// <summary>
		/// Ons the binding context changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The event.</param>
		private void Bindable_BindableOnBindingContextChanged(object sender, EventArgs e)
		{
			this.OnBindingContextChanged();
			object? bc = this.Bindable?.BindingContext;
			if (!(bc is null)) {
				this.BindingContext = bc;
			}
		}

		/// <summary>
		/// Ons the switch toggled.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The event.</param>
		private void Bindable_OnSwitchToggled(object sender, ToggledEventArgs e)
		{
			this.Command?.Execute(e.Value);
		}

		/// <summary>
		/// On detaching from.
		/// </summary>
		/// <param name="bindable">The bindable component.</param>
		/// <exception cref="System.ArgumentNullException"/>
		/// <exception cref="System.ArgumentException"/>
		protected override void OnDetachingFrom(Switch bindable)
		{
			if (bindable is null) {
				throw new ArgumentNullException(nameof(bindable));
			}
			if (this.Bindable != bindable) {
				throw new ArgumentException($"this.{nameof(this.Bindable)} is not {bindable}.", nameof(bindable));
			}
			base.OnDetachingFrom(bindable);
			this.Bindable.BindingContextChanged -= this.Bindable_BindableOnBindingContextChanged;
			this.Bindable.Toggled               -= this.Bindable_OnSwitchToggled;
			this.Bindable = null;
		}
	}
}
