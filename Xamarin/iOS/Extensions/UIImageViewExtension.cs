using UIKit;

namespace ObsidianMobile.iOS.Extensions
{
    public static class UIImageViewExtension
    {
        public static void MakeRoundImageView(this UIImageView imageView)
        {
            imageView.Layer.CornerRadius = imageView.Frame.Size.Width / 2;
            imageView.ClipsToBounds = true;
        }
    }
}