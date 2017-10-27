using System;
using Android.Text.Style;
using Android.Views;

namespace ObsidianMobile.Droid.Core.Widget
{
    public class SimpleClickableSpan : ClickableSpan
    {
        readonly Action<View, string> Click;

        readonly string Link;

        public SimpleClickableSpan(string link, Action<View, string> click)
        {
            Click = click;
            Link = link;
        }

        public override void OnClick(View widget)
        {
            Click?.Invoke(widget, Link);
        }
    }
}
