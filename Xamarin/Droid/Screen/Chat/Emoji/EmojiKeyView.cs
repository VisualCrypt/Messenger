using System;
using Android.Content;
using Android.Graphics;
using Android.Support.Text.Emoji.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using ObsidianMobile.Core.Extensions;

namespace ObsidianMobile.Droid.Screen.Chat.Emoji
{
    public class EmojiKeyView : FrameLayout
    {
        public event EventHandler<EmojiKeyClickEventArgs> EmojiClick;

        const int EmojiDpSize = 32;

        public TextView Emoji { get; private set; }

        public EmojiKeyView(Context context) :
            base(context)
        {
            Initialize();
        }

        public EmojiKeyView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public EmojiKeyView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            SetPadding(0, 0, 0, (int)(Resources.DisplayMetrics.Density * 4));
            SetBackgroundResource(Resource.Drawable.ripple_contacts);
            Clickable = true;
            Focusable = true;

            Emoji = new EmojiAppCompatTextView(Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            AddView(Emoji);
            var layoutParams = Emoji.LayoutParameters as LayoutParams;
            layoutParams.Gravity = GravityFlags.Center;
            Emoji.LayoutParameters = layoutParams;

            Emoji.SetTextSize(ComplexUnitType.Dip, EmojiDpSize);
            Emoji.SetTextColor(Color.Black);

            Click += (s, e) => OnEmojiClick(new EmojiKeyClickEventArgs(Emoji.Text));
        }

        void OnEmojiClick(EmojiKeyClickEventArgs e)
        {
            e.Raise(this, ref EmojiClick);
        }
    }

    public class EmojiKeyClickEventArgs : EventArgs
    {
        public string EmojiText { get; private set; }

        public EmojiKeyClickEventArgs(string emojiText)
        {
            EmojiText = emojiText;
        }
    }
}
