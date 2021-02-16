using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

namespace Covid19Radar.iOS.Extensions
{
	public static class FormattedStringExtensions
	{
		internal static NSAttributedString ToAttributed(
			this FormattedString formattedString,
			     BindableObject  owner,
			     Color           defaultForegroundColor,
			     TextAlignment   textAlignment = TextAlignment.Start,
			     double          lineHeight    = -1.0D)
		{
			if (formattedString is null) {
				return null;
			}
			var attributed = new NSMutableAttributedString();
			int count = formattedString.Spans.Count;
			for (int i = 0; i < count; i++) {
				var span = formattedString.Spans[i];
				var attributedString = span.ToAttributed(owner, defaultForegroundColor, textAlignment, lineHeight);
				if (attributedString == null) {
					continue;
				}
				attributed.Append(attributedString);
			}
			return attributed;
		}

		internal static NSAttributedString ToAttributed(
			this Span           span,
			     BindableObject owner,
			     Color          defaultForegroundColor,
			     TextAlignment  textAlignment,
			     double         lineHeight = -1.0)
		{
			if (span is null) {
				return null;
			}
			string text = span.Text;
			if (text is null) {
				return null;
			}
			var style = new NSMutableParagraphStyle();
			lineHeight = span.LineHeight >= 0 ? span.LineHeight : lineHeight;
			if (lineHeight >= 0) {
				style.LineHeightMultiple = new nfloat(lineHeight);
			}
			style.Alignment = textAlignment switch {
				TextAlignment.Start  => UITextAlignment.Left,
				TextAlignment.Center => UITextAlignment.Center,
				TextAlignment.End    => UITextAlignment.Right,
				_                    => UITextAlignment.Left
			};
			UIFont targetFont;
			if (span.IsDefault()) {
				targetFont = ((IFontElement)(owner)).ToUIFont();
			} else {
				targetFont = span.ToUIFont();
			}
			var fgcolor = span.TextColor;
			if (fgcolor.IsDefault) {
				fgcolor = defaultForegroundColor;
				if (fgcolor.IsDefault) {
					fgcolor = UIColor.Black.ToColor();
				}
			}
			var  spanFgColor      = fgcolor.ToUIColor();
			var  spanBgColor      = span.BackgroundColor.ToUIColor();
			bool hasUnderline     = false;
			bool hasStrikethrough = false;
			if (span.IsSet(Span.TextDecorationsProperty)) {
				var textDecorations = span.TextDecorations;
				hasUnderline        = (textDecorations & TextDecorations.Underline) != 0;
				hasStrikethrough    = (textDecorations & TextDecorations.Strikethrough) != 0;
			}
			return new NSAttributedString(
				text, targetFont, spanFgColor, spanBgColor,
				underlineStyle    : hasUnderline ? NSUnderlineStyle.Single : NSUnderlineStyle.None,
				strikethroughStyle: hasStrikethrough ? NSUnderlineStyle.Single : NSUnderlineStyle.None,
				paragraphStyle    : style,
				kerning           : ((float)(span.CharacterSpacing))
			);
		}
	}
}
