using System;
using Android.Views;

namespace ObsidianMobile.Droid.Core.Extensions
{
    public static class ViewExtensions
    {
        public static void MakeGone(this View view)
        {
            view.Visibility = ViewStates.Gone;
        }

        public static void MakeVisible(this View view)
        {
            view.Visibility = ViewStates.Visible;
        }

        public static void MakeInvisible(this View view)
        {
            view.Visibility = ViewStates.Invisible;
        }

        public static bool IsVisible(this View view)
        {
            return view.Visibility == ViewStates.Visible;
        }

        public static bool IsInvisible(this View view)
        {
            return view.Visibility == ViewStates.Invisible;
        }

        public static void SetHeight(this View view, int height)
        {
            var layoutParameters = view.LayoutParameters;
            layoutParameters.Height = height;
            view.LayoutParameters = layoutParameters;
        }
    }
}
