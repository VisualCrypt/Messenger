using System;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using ObsidianMobile.Core.Extensions;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Models.Messages;
using ObsidianMobile.Droid.Core.Extensions;

namespace ObsidianMobile.Droid.Chat.Adapter
{
    public class ChatTextViewHolder : ViewHolderModel<IMessage>
    {
        public event EventHandler<MessageLinkClickEventArgs> LinkClick;

        [InjectView(Resource.Id.interlocutor_message)]
        TextView InterlocutorMessage;

        [InjectView(Resource.Id.current_user_message)]
        TextView CurrentUserMessage;

        [InjectView(Resource.Id.current_user_container)]
        View CurrentUserContainer;

        [InjectView(Resource.Id.interlocutor_container)]
        View InterlocutorContainer;

        [InjectView(Resource.Id.interlocutor_message_time)]
        TextView InterlocutorMessageTime;

        [InjectView(Resource.Id.current_user_message_time)]
        TextView CurrentUserMessageTime;

        Color LinkSpanColor;
        Color UserSpanColor;
        Color TagSpanColor;

        public ChatTextViewHolder(View view) : base(view)
        {
            Cheeseknife.Inject(this, ItemView);

            LinkSpanColor = Color.Green;
            UserSpanColor = Color.DarkRed;
            TagSpanColor = Color.Blue;

            CurrentUserMessage.MovementMethod = LinkMovementMethod.Instance;
            InterlocutorMessage.MovementMethod = LinkMovementMethod.Instance;
        }

        public override void Update(IMessage data, bool isCurrentUser)
        {
            InterlocutorContainer.Visibility = isCurrentUser ? ViewStates.Gone : ViewStates.Visible;
            CurrentUserContainer.Visibility = isCurrentUser ? ViewStates.Visible : ViewStates.Gone;

            var message = data as TextMessage;
            SetMessage(message.Text, isCurrentUser);
            SetTime(message.Date.ToMessageStyleString(), isCurrentUser);
        }

        void SetMessage(string message, bool isCurrentUser)
        {
            var spannableMessage = message.ToSpannable(LinkSpanColor, UserSpanColor, TagSpanColor, OnLinkClick);
            if (isCurrentUser)
            {
                CurrentUserMessage.TextFormatted = spannableMessage;
            }
            else
            {
                InterlocutorMessage.TextFormatted = spannableMessage;
            }
        }

        void SetTime(string time, bool isCurrentUser)
        {
            if (isCurrentUser)
            {
                CurrentUserMessageTime.Text = time;
            }
            else
            {
                InterlocutorMessageTime.Text = time;
            }
        }

        void OnLinkClick(View v, string link)
        {
            new MessageLinkClickEventArgs(link).Raise(this, ref LinkClick);
        }

        public class Creator : IViewHolderModelCreator
        {
            public static readonly Creator Instance = new Creator();

            public int LayoutResId => Resource.Layout.adapter_chatmessage_text;

            public RecyclerView.ViewHolder Create(View view)
            {
                return new ChatTextViewHolder(view);
            }
        }
    }

    public class MessageLinkClickEventArgs : EventArgs
    {
        public string Link { get; private set; }

        public MessageLinkClickEventArgs(string link)
        {
            Link = link;
        }
    }
}
