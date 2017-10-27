using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ObsidianMobile.Core.Factories;
using ObsidianMobile.Core.Interfaces.Api;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Models;
using ObsidianMobile.Core.Models.Messages;
using ObsidianMobile.Core.Utils;

namespace ObsidianMobile.Core
{
    public class Server : IChatApi
    {
        public const int DEFAULT_СOMPANION_ID = -1;

        public static IContact CurrentUser
        {
            get
            {
                return new Contact
                {
                    Id = CURRENT_USER_ID,
                    Name = "User"
                };
            }
        }

        public const int CURRENT_USER_ID = 1;

        ObservableCollection<IContact> _contacts { get; set; }
        ObservableCollection<IMessage> _messages { get; set; }
        ObservableCollection<IChat> _chats { get; set; }

        public Action<IMessage> OnAnswer { get; set; }

        public Server()
        {
            _contacts = DummyContactsFactory.GetContacts();  //new ObservableCollection<IContact>();
            _messages = DummyMessagesFactory.GetMessages();  //new ObservableCollection<IMessage>();

            _chats = DummyChatsFactory.GetChats(); //new ObservableCollection<IChats>();
        }

        #region contacts

        public IChat GetChatInfo(int chatId)
        {
            return _chats.Where(c => c.Id == chatId).FirstOrDefault();
        }

        public ObservableCollection<IContact> GetContacts()
        {
            return _contacts;
        }

        public void CreateContact(IContact contact)
        {
            _contacts.Add(contact);
        }

        public void UpdateContact(IContact contact)
        {
            var user = _contacts.Where(c => c.Id == contact.Id).FirstOrDefault();

            if (user != null)
            {
                user.Name = contact.Name;
            }
        }

        #endregion region

        #region messages

        public ObservableCollection<IMessage> GetAllMessages()
        {
            return _messages;
        }

        public ObservableCollection<IMessage> GetMessagesForChat(int chatID)
        {
            var result = _messages.Where(m => m.ChatId == chatID);

            return new ObservableCollection<IMessage>(result);
        }

        public void SendMessage(IMessage message)
        {
            _messages.Add(message);

            SendAnswer(message, message.ToUserId);
        }

        void SendAnswer(IMessage previousMessage, int fromUserID)
        {
            var message = new TextMessage
            {
                FromUserId = fromUserID,
                ToUserId = CURRENT_USER_ID,
                Text = "response for \"" + previousMessage.Text + "\"",
                ChatId = previousMessage.ChatId
            };

            var deleay = RandomGenerator.Generate(2, 4);

            Task.Delay(TimeSpan.FromSeconds(deleay)).ContinueWith((arg) =>
            {
                message.Date = DateTime.Now;
                _messages.Add(message);
                OnAnswer?.Invoke(message);
            });
        }
        #endregion

        #region chat

        private int CreateChat(List<IContact> contacts)
        {
            var chat = new Chat()
            {
                Contacts = new ObservableCollection<IContact>(contacts)
            };

            _chats.Add(chat);

            return chat.Id;
        }

        public int GetChatIdForContacts(List<IContact> contacts)
        {
            var selectedChat = _chats.Where(chat => contacts.All(contact => chat.Contacts.Any(chatContact => chatContact.Id == contact.Id))).FirstOrDefault();

            return (selectedChat == null) ? CreateChat(contacts) : selectedChat.Id;
        }
        #endregion
    }
}