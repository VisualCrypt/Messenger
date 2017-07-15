using System;
using System.Globalization;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;

namespace Obsidian.Applications.Models.Chat
{
   
    public class Message : XMessage, IId
    {
        public string YourId { get; set; }
        public MessageSide Side { get; set; }
        public MessageSide PrevSide { get; set; }
        public SendMessageState SendMessageState { get; set; }
        public LocalMessageState LocalMessageState { get; set; }
        public byte[] EncryptedE2EEncryptionKey { get; set; }

        // Ignore on Saving
        public string ImageCipherSnippet { get; set; }
        // Ignore on Saving
        public string ThreadText { get; set; }
        // Ignore on Saving
        public byte[] ThreadMedia { get; set; }
        // Ignore on Saving
        public string ImageImportPath { get; set; }
      
    }


    public static class Helpers
    {
        public static void SetPreviousSide(this Message messageToSet, Message previousMessage)
        {
            if(messageToSet.Side ==MessageSide.NotSet)
                throw new InvalidOperationException();
           
            if (previousMessage == null) // if there is no previous message, set the inverse as PrevSide, so that the bubbles with arrows appear.
                messageToSet.PrevSide = messageToSet.Side == MessageSide.Me ? MessageSide.You : MessageSide.Me;
            else
                messageToSet.PrevSide = previousMessage.Side == MessageSide.Me ? MessageSide.Me : MessageSide.You;
        }

        public static string GetDateString(this Message message)
        {
            var date = message.EncryptedDateUtc;
            return date.ToLocalTime().ToString(CultureInfo.CurrentCulture);
        }

        public static string GetCacheKey(this Message message)
        {
            if (message.Id == null || message.RecipientId == null || message.SenderId == null)
                throw new InvalidOperationException();
            return String.Concat(message.Id, message.RecipientId, message.SenderId);
        }

        public static string GetVisualCryptPreview(this Message message)
        {
            if (message.ImageCipher == null)
                return null;
            return VisualCrypt2Formatter.CreateVisualCryptText(message.ImageCipher, 1000, 46).Text;
        }
    }
}
