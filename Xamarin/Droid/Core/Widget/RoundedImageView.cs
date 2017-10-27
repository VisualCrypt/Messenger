using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;

namespace ObsidianMobile.Droid.Core.Widget
{
    public class RoundedImageView : AppCompatImageView
    {
        public RoundedImageView(Context context) :
            base(context)
        {
            Initialize();
        }

        public RoundedImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public RoundedImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            SetBackgroundResource(Resource.Drawable.image_rounded);
            ClipToOutline = true;
        }
    }
}
