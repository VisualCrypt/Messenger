using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ObsidianMobile.Core.Extensions;

namespace ObsidianMobile.Droid.Screen.Chat.Emoji
{
    public class EmojiListViewAdapter : RecyclerView.Adapter
    {
        /// <summary>
        /// Occurs when emoji is clickd. 
        /// Consumers of this event mus subscribe to EmojiKeyboardView.EmojiClick event.
        /// </summary>
        public event EventHandler<EmojiKeyClickEventArgs> EmojiClick;

        readonly IList<string> Emojis;

        public EmojiListViewAdapter(IList<string> emojis)
        {
            Emojis = emojis;
        }

        public override int ItemCount => Emojis.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as EmojiViewHolder).Emoji.Text = Emojis[position];
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = new EmojiKeyView(parent.Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent),
            };
			view.EmojiClick += (s, e) => OnEmojiClick(e);
            var viewHolder = new EmojiViewHolder(view);

            return viewHolder;
        }

        void OnEmojiClick(EmojiKeyClickEventArgs e)
        {
            e.Raise(this, ref EmojiClick);
        }
    }

    public class EmojiViewHolder : RecyclerView.ViewHolder
    {
        public TextView Emoji;

        public EmojiViewHolder(View view) : base (view)
        {
            Emoji = (ItemView as ViewGroup).GetChildAt(0) as TextView;
        }
    }
}
