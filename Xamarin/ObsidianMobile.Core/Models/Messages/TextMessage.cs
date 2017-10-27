using System;
using ObsidianMobile.Core.Enums;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Utils;

namespace ObsidianMobile.Core.Models.Messages
{
    public class TextMessage : IMessage
    {
        public int Id { get; }

        public int ChatId { get; set; }

        public MessageType Type { get; set; }

        public DateTime Date { get; set; }

        public int FromUserId { get; set; }

        public string Text { get; set; }

        public int ToUserId { get; set; }

        public string FromUserName { get; set; }

        public TextMessage()
        {

        }

        public TextMessage(int chatId, MessageType type, int fromUserId, int toUserId, DateTime time, string text)
        {
            ChatId = chatId;
            FromUserId = fromUserId;
            ToUserId = toUserId;
            Date = time;
            Text = text;
            Type = type;
            Id = RandomGenerator.GenerateId();
        }
    }
}