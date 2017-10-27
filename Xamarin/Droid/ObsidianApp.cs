using System;
using Android.App;
using Android.Runtime;
using Android.Support.Text.Emoji;
using Android.Support.Text.Emoji.Bundled;
using ObsidianMobile.Core.DependencyResolution;

namespace ObsidianMobile.Droid
{
    [Application]
    public class ObsidianApp : Application
    {
        public ObsidianApp(IntPtr handle, JniHandleOwnership transer)
        : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Resolver.RegisterTypes();

            var navigation = new DroidNavigationService();
            navigation.Initialize();
            Resolver.RegisterNavigaionService(navigation);

            var emojiConfig = new BundledEmojiCompatConfig(this);
            EmojiCompat.Init(emojiConfig);
        }
    }
}
