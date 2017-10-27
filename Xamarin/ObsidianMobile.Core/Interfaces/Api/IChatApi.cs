using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ObsidianMobile.Core.Interfaces.Models;

namespace ObsidianMobile.Core.Interfaces.Api
{
    public interface IChatApi
    {
        ObservableCollection<IContact> GetContacts();

        IChat GetChatInfo(int chatId);

        void CreateContact(IContact contact);

        void UpdateContact(IContact contact);

        ObservableCollection<IMessage> GetAllMessages();

        ObservableCollection<IMessage> GetMessagesForChat(int chatId);

        void SendMessage(IMessage message);

        Action<IMessage> OnAnswer { get; set; }

        int GetChatIdForContacts(List<IContact> contacts);
    }
}
