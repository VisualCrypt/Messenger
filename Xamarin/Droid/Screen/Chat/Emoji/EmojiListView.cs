using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using ObsidianMobile.Core.Extensions;

namespace ObsidianMobile.Droid.Screen.Chat.Emoji
{
    public class EmojiListView : RecyclerView
    {
        public event EventHandler<EmojiKeyClickEventArgs> EmojiClick;

        const int SpansCount = 9;

        public IList<string> Emojis { get; set; }

        public EmojiListView(Context context, IList<string> emojis) : this(context)
        {
            Emojis = emojis;
        }

        public EmojiListView(Context context) : base(context) { }

        public EmojiListView(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public EmojiListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        public void Init()
        {
            SetLayoutManager(new GridLayoutManager(Context, SpansCount));
            var adapter = new EmojiListViewAdapter(Emojis);
            adapter.EmojiClick += (s, e) => OnEmojiClick(e);
            base.SetAdapter(adapter);
        }

        void OnEmojiClick(EmojiKeyClickEventArgs e)
        {
            e.Raise(this, ref EmojiClick);
        }
    }
}
