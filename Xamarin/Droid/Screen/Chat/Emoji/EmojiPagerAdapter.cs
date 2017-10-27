using System;
using System.Collections.Generic;
using Android.Support.V4.View;
using Android.Views;
using ObsidianMobile.Core.Extensions;
using JavaObject = Java.Lang.Object;

namespace ObsidianMobile.Droid.Screen.Chat.Emoji
{
    public class EmojiPagerAdapter : PagerAdapter
    {
        public event EventHandler<EmojiKeyClickEventArgs> EmojiClick;

        readonly IList<EmojiPageInfo> PageInfos;
        readonly View[] Pages;

        public EmojiPagerAdapter(IList<EmojiPageInfo> pageInfos)
        {
            PageInfos = pageInfos;
            Pages = new View[PageInfos.Count];
        }

        public override int Count => PageInfos.Count;

        public override bool IsViewFromObject(View view, JavaObject @object)
        {
            return ReferenceEquals(view, @object);
        }

        public override JavaObject InstantiateItem(ViewGroup container, int position)
        {
            var view = new EmojiListView(container.Context, PageInfos[position].Items)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
            };
            view.Init();
            view.EmojiClick += (s, e) => OnEmojiClick(e);

            Pages[position] = view;
            container.AddView(view);

            return view;
        }

        public override void DestroyItem(ViewGroup container, int position, JavaObject @object)
        {
            var view = Pages[position];
            (view as EmojiListView).EmojiClick += (s, e) => OnEmojiClick(e);
            container.RemoveView(view);
            Pages[position] = null;
        }

        void OnEmojiClick(EmojiKeyClickEventArgs e)
        {
            e.Raise(this, ref EmojiClick);
        }
    }
}
