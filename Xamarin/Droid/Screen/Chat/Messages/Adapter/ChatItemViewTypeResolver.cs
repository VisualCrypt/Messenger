using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Models.Messages;

namespace ObsidianMobile.Droid.Chat.Test.Model
{
    public class ChatItemViewTypeResolver : IItemViewTypeResolver
    {
        readonly ObservableCollection<IMessage> Messages;
        readonly IDictionary<Type, int> MessageTypesToItemViewType;

        public ChatItemViewTypeResolver(ObservableCollection<IMessage> messages)
        {
            Messages = messages;
            MessageTypesToItemViewType = new Dictionary<Type, int>
            {
                { typeof(TextMessage), (int)ChatItemViewType.Text },
                { typeof(ImageMessage), (int)ChatItemViewType.Image }
            };
        }

        public int GetItemType(int position)
        {
            var message = Messages[position];
            return MessageTypesToItemViewType[message.GetType()];
        }
    }

    public enum ChatItemViewType
    {
        Text,
        Image,
    }
}
