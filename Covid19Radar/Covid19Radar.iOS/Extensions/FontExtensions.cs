using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Covid19Radar.iOS.Extensions
{
	public static class FontExtensions
	{
		private static readonly string                                  _default_font_name = UIFont.SystemFontOfSize(12).Name;
		private static readonly Dictionary<ToNativeFontFontKey, UIFont> _to_ui_font        = new Dictionary<ToNativeFontFontKey, UIFont>();

		public static UIFont ToUIFont(this Font self)
		{
			return ToNativeFont(self);
		}

		internal static UIFont ToUIFont(this IFontElement element)
		{
			return ToNativeFont(element);
		}

		private static UIFont _ToNativeFont(string family, float size, FontAttributes attributes)
		{
			bool bold   = (attributes & FontAttributes.Bold)   != 0;
			bool italic = (attributes & FontAttributes.Italic) != 0;
			if (family != null && family != _default_font_name) {
				try {
					UIFont result = null;
					if (UIFont.FamilyNames.Contains(family)) {
						var descriptor = new UIFontDescriptor().CreateWithFamily(family);
						if (bold || italic) {
							var traits = ((UIFontDescriptorSymbolicTraits)(0));
							if (bold) {
								traits |= UIFontDescriptorSymbolicTraits.Bold;
							}
							if (italic) {
								traits |= UIFontDescriptorSymbolicTraits.Italic;
							}
							descriptor = descriptor.CreateWithTraits(traits);
							result     = UIFont.FromDescriptor(descriptor, size);
							if (result is not null) {
								return result;
							}
						}
					}
					string cleansedFont = CleanseFontName(family);
					result = UIFont.FromName(cleansedFont, size);
					if (family.StartsWith(".SFUI", StringComparison.InvariantCultureIgnoreCase)) {
						string fontWeight = family.Split('-').LastOrDefault();
						if (!string.IsNullOrWhiteSpace(fontWeight) && Enum.TryParse<UIFontWeight>(fontWeight, true, out var uIFontWeight)) {
							result = UIFont.SystemFontOfSize(size, uIFontWeight);
							return result;
						}
						result = UIFont.SystemFontOfSize(size, UIFontWeight.Regular);
						return result;
					}
					if (result is null) {
						result = UIFont.FromName(family, size);
					}
					if (result is not null) {
						return result;
					}
				} catch (Exception e) {
					Debug.WriteLine("Could not load font named: {0}", family);
					Debug.WriteLine(e.ToString());
				}
			}
			if (bold && italic) {
				var defaultFont = UIFont.SystemFontOfSize(size);
				var descriptor = defaultFont.FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Bold | UIFontDescriptorSymbolicTraits.Italic);
				return UIFont.FromDescriptor(descriptor, 0);
			}
			if (italic) {
				return UIFont.ItalicSystemFontOfSize(size);
			}
			if (bold) {
				return UIFont.BoldSystemFontOfSize(size);
			}
			return UIFont.SystemFontOfSize(size);
		}

		static string CleanseFontName(string fontName)
		{
			//First check Alias
			var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
			if (hasFontAlias) {
				return fontPostScriptName;
			}
			var fontFile = FontFile.FromString(fontName);
			if (!string.IsNullOrWhiteSpace(fontFile.Extension)) {
				var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
				if (hasFont) {
					return filePath ?? fontFile.PostScriptName;
				}
			} else {
				foreach (string ext in FontFile.Extensions) {
					string formated = fontFile.FileNameWithExtension(ext);
					var (hasFont, filePath) = FontRegistrar.HasFont(formated);
					if (hasFont) {
						return filePath;
					}
				}
			}
			return fontFile.PostScriptName;
		}

		internal static bool IsDefault(this Span self)
		{
			return self.FontFamily == null
				&& self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true)
				&& self.FontAttributes == FontAttributes.None;
		}

		private static UIFont ToNativeFont(this IFontElement element)
		{
			string fontFamily  = element.FontFamily;
			float fontSize     = unchecked((float)(element.FontSize));
			var fontAttributes = element.FontAttributes;
			return ToNativeFont(fontFamily, fontSize, fontAttributes, _ToNativeFont);
		}

		private static UIFont ToNativeFont(this Font self)
		{
			float size = unchecked((float)(self.FontSize));
			if (self.UseNamedSize) {
				size = self.NamedSize switch {
					NamedSize.Micro  => 12,
					NamedSize.Small  => 14,
					NamedSize.Medium => 17, // as defined by iOS documentation
					NamedSize.Large  => 22,
					_                => 17
				};
			}
			return ToNativeFont(self.FontFamily, size, self.FontAttributes, _ToNativeFont);
		}

		private static UIFont ToNativeFont(string family, float size, FontAttributes attributes, Func<string, float, FontAttributes, UIFont> factory)
		{
			var key = new ToNativeFontFontKey(family, size, attributes);
			lock (_to_ui_font) {
				if (_to_ui_font.TryGetValue(key, out var value)) {
					return value;
				}
			}
			var generatedValue = factory(family, size, attributes);
			lock (_to_ui_font) {
				if (!_to_ui_font.TryGetValue(key, out var value)) {
					_to_ui_font.Add(key, value = generatedValue);
				}
				return value;
			}
		}

		private readonly struct ToNativeFontFontKey
		{
#pragma warning disable 0414    // these are not called explicitly, but they are used to establish uniqueness. allow it!
#pragma warning disable IDE0052 // 読み取られていないプライベート メンバーを削除
			private readonly string         _family;
			private readonly float          _size;
			private readonly FontAttributes _attributes;
#pragma warning restore IDE0052 // 読み取られていないプライベート メンバーを削除
#pragma warning restore 0414

			internal ToNativeFontFontKey(string family, float size, FontAttributes attributes)
			{
				_family     = family;
				_size       = size;
				_attributes = attributes;
			}
		}
	}
}
