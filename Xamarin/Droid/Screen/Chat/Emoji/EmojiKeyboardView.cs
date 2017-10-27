using System;
using System.Linq;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using ObsidianMobile.Core.Extensions;

namespace ObsidianMobile.Droid.Screen.Chat.Emoji
{
    public class EmojiKeyboardView : FrameLayout
    {
        const int PageOffscreenLimit = 4;

        public event EventHandler<EmojiKeyClickEventArgs> EmojiClick;

        public EmojiKeyboardView(Context context) : base(context)
        {
            Initialize();
        }

        public EmojiKeyboardView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public EmojiKeyboardView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize();
        }

        void Initialize()
        {
            var adapter = new EmojiPagerAdapter(EmojiPageInfo.All);
            adapter.EmojiClick += (s, e) => OnEmojiClick(e);

            var pager = new ViewPager(Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent
                ),
                Adapter = adapter
            };
            AddView(pager);
            pager.OffscreenPageLimit = PageOffscreenLimit;

            var tabs = new TabLayout(Context);
            pager.AddView(tabs);
            var layoutParams = tabs.LayoutParameters as ViewPager.LayoutParams;
            layoutParams.Height = ViewGroup.LayoutParams.WrapContent;
            layoutParams.Gravity = (int)GravityFlags.Top;
            tabs.LayoutParameters = layoutParams;
            tabs.SetupWithViewPager(pager);
            tabs.SetSelectedTabIndicatorColor(ContextCompat.GetColor(Context, Resource.Color.chat_emojikeyboard_tabicon_selected));
            foreach (var i in Enumerable.Range(0, EmojiPageInfo.All.Count))
            {
                tabs.GetTabAt(i).SetIcon(EmojiPageInfo.All[i].TabIconResId);
            }
        }

        void OnEmojiClick(EmojiKeyClickEventArgs e)
        {
            e.Raise(this, ref EmojiClick);
        }
    }
}
