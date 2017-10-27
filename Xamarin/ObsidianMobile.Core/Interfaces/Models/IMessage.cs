using System;
using ObsidianMobile.Core.Enums;

namespace ObsidianMobile.Core.Interfaces.Models
{
    public interface IMessage
    {
        int Id { get; }

        int ChatId { get; set; }

        MessageType Type { get; set; }

        DateTime Date { get; set; }

        int FromUserId { get; set; }

        int ToUserId { get; set; }

        string Text { get; set; }
    }
}