using System;
using ObsidianMobile.Core.Enums;
using ObsidianMobile.Core.Interfaces.Models;

namespace ObsidianMobile.Core.Models.Messages
{
    public class ImageMessage : IMessage
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        public MessageType Type { get; set; }

        public DateTime Date { get; set; }

        public int FromUserId { get; set; }

        public String ImageUrl { get; set; }

        public int ToUserId { get; set; }

        public string Text { get; set; }

        public ImageMessage(int userId, DateTime date, String imageUrl)
        {
            FromUserId = userId;
            Date = date;
            ImageUrl = imageUrl;
        }
    }
}
