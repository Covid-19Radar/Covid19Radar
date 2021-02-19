using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Extensions
{
	public static class Extensions
	{
		internal static NSMutableAttributedString AddCharacterSpacing(this NSAttributedString attributedString, string text, double characterSpacing)
		{
			if (attributedString is null && characterSpacing == 0) {
				return null;
			}
			NSMutableAttributedString mutableAttributedString;
			if (attributedString is null || attributedString.Length == 0) {
				mutableAttributedString = text == null ? new NSMutableAttributedString() : new NSMutableAttributedString(text);
			} else {
				mutableAttributedString = new NSMutableAttributedString(attributedString);
			}
			AddKerningAdjustment(mutableAttributedString, text, characterSpacing);
			return mutableAttributedString;
		}

		internal static void AddKerningAdjustment(NSMutableAttributedString mutableAttributedString, string text, double characterSpacing)
		{
			if (!string.IsNullOrEmpty(text)) {
				if (characterSpacing == 0 && !mutableAttributedString.HasCharacterAdjustment()) {
					return;
				}
				mutableAttributedString.AddAttribute(
					UIStringAttributeKey.KerningAdjustment,
					NSObject.FromObject(characterSpacing),
					new NSRange(0, text.Length - 1)
				);
			}
		}

		internal static bool HasCharacterAdjustment(this NSMutableAttributedString mutableAttributedString)
		{
			if (mutableAttributedString is null) {
				return false;
			}
			var attributes = mutableAttributedString.GetAttributes(0, out _);
			for (uint i = 0; i < attributes.Count; i++) {
				if (attributes.Keys[i] is NSString nSString && nSString == UIStringAttributeKey.KerningAdjustment) {
					return true;
				}
			}
			return false;
		}
	}
}
