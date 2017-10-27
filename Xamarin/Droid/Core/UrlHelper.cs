using System;

using Android.App;
using AndroidUri = Android.Net.Uri;

using Android.Support.CustomTabs;
using Android.Content;

namespace ObsidianMobile.Droid.Core
{
    public static class UrlHelper
    {
        const int NoFlags = 0;
        const string ChromePackage = "com.android.chrome";
        const string HttpPrefix = "http://";
        const string HttpsPrefix = "https://";

        public static bool OpenUrl(Context context, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            url = PrependHttpSchemeIfNecessary(url);
            try
            {
                if (IsChromeCustomTabsSupported(context))
                {
                    new CustomTabsIntent.Builder().Build().LaunchUrl(context, AndroidUri.Parse(url));
                }
                else
                {
                    var intent = new Intent(Intent.ActionView, AndroidUri.Parse(url));
                    context.StartActivity(intent);
                }
            } catch (ActivityNotFoundException) {
                return false;
            }

            return true;
        }

        static bool IsChromeCustomTabsSupported(Context context)
        {
            var serviceIntent = new Intent(CustomTabsService.ActionCustomTabsConnection);
            serviceIntent.SetPackage(ChromePackage);
            var infos = context.PackageManager.QueryIntentServices(serviceIntent, NoFlags);
            return infos != null && infos.Count != 0;
        }

        static string PrependHttpSchemeIfNecessary(string url)
        {
            if (!url.StartsWith(HttpPrefix, StringComparison.Ordinal) ||
                    !url.StartsWith(HttpsPrefix, StringComparison.Ordinal))
            {
                url = url.Insert(0, HttpPrefix);
            }

            return url;
        }
    }
}
