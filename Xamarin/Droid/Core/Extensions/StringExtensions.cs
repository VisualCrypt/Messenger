using System;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using ObsidianMobile.Core.Utils;
using ObsidianMobile.Droid.Core.Widget;

namespace ObsidianMobile.Droid.Core.Extensions
{
    public static class StringExtensions
    {
        public static ISpannable ToSpannable(this string text, Color linkColor, Color userColor, Color tagColor, Action<View, string> linkClick)
        {
            var regex = new ActiveItemRegex();
            var items = regex.GetAllElements(text);

            var spannableText = new SpannableString(text);

            foreach (var item in items)
            {
                switch (item.Type)
                {
                    case RegexType.Url:
                        AddLinkSpan(item, spannableText, linkColor, linkClick);
                        break;
                    case RegexType.Username:
                        AddColorSpan(item, spannableText, userColor);
                        break;
                    case RegexType.Hashtag:
                        AddColorSpan(item, spannableText, tagColor);
                        break;
                }
            }

            return spannableText;
        }

        static void AddLinkSpan(ActiveItem item, SpannableString spannableText, Color color, Action<View, string> linkClick)
        {
            var span = new SimpleClickableSpan(item.Text, linkClick);
            spannableText.SetSpan(span, item.StartIndex, item.LastIndex, SpanTypes.InclusiveInclusive);
            AddColorSpan(item, spannableText, color);
        }

        static void AddColorSpan(ActiveItem item, SpannableString spannableText, Color color)
        {
            spannableText.SetSpan(new ForegroundColorSpan(color), item.StartIndex, item.LastIndex, SpanTypes.InclusiveInclusive);
        }
    }
}
