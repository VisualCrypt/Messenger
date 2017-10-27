using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using ObsidianMobile.Core.Extensions;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Droid.Chat.Adapter;
using ObsidianMobile.Droid.Chat.Test;
using ObsidianMobile.Droid.Chat.Test.Model;

namespace ObsidianMobile.Droid.Chat
{
    public class ChatAdapterDelegate
    {
        public event EventHandler<MessageLinkClickEventArgs> LinkClick;

        readonly IItemViewTypeResolver ItemViewTypeResolver;

        readonly IDictionary<ChatItemViewType, IViewHolderModelCreator> ViewHolderCreators;

        readonly int CurrentUserId;

        public ChatAdapterDelegate(IItemViewTypeResolver itemViewTypeResolver, int currentUserId)
        {
            ItemViewTypeResolver = itemViewTypeResolver;
            CurrentUserId = currentUserId;

            ViewHolderCreators = new Dictionary<ChatItemViewType, IViewHolderModelCreator>
            {
                { ChatItemViewType.Text, ChatTextViewHolder.Creator.Instance },
                { ChatItemViewType.Image, ChatTextViewHolder.Creator.Instance }
            };
        }

        public void OnBindViewHolder(RecyclerView.ViewHolder holder, IMessage message, int position)
        {
            (holder as ChatTextViewHolder).Update(message, message.FromUserId == CurrentUserId);
        }

        public RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var creator = ViewHolderCreators[(ChatItemViewType)viewType];
            var view = LayoutInflater.From(parent.Context).Inflate(creator.LayoutResId, parent, false);

            //TODO use EventBus. Discuss after milestone.
            var holder = creator.Create(view) as ChatTextViewHolder;
            if (holder != null)
            {
                holder.LinkClick += (s, e) => e.Raise(this, ref LinkClick);
            }

            return holder;
        }

        public int GetItemViewType(int position)
        {
            return ItemViewTypeResolver.GetItemType(position);
        }
    }
}
