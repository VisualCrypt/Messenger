using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ObsidianMobile.Core.Enums;
using ObsidianMobile.Core.Interfaces.Api;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Core.Interfaces.Views;
using ObsidianMobile.Core.Models.Messages;

namespace ObsidianMobile.Core.ViewModels
{
    public class ChatViewModel : BaseViewModel, IChatViewModel
    {
        public RelayCommand NaviagteToChatCommand { get; private set; }

        public RelayCommand<IContact> NavigateToContactDetailsCommand { get; private set; }

        public int ChatId { get; set; }

        string currentMessage;
        public string CurrentMessage
        {
            get { return currentMessage; }
            set
            {
                currentMessage = value;
                RaisePropertyChanged(nameof(CurrentMessage));
            }
        }

        public string ChatName
        {
            get { return RecipientContact.Name; }
        }

        IContact _recipientContact;
        public IContact RecipientContact
        {
            get { return _recipientContact; }
            private set
            {
                _recipientContact = value;
                RaisePropertyChanged(nameof(_recipientContact));
            }
        }

        public ObservableCollection<IMessage> Messages { get; private set; }

        public RelayCommand SendTextMessageCommand { get; private set; }

        public IChatView View { get; set; }

        readonly IChatApi _chat;

        readonly IObsidianNavigationService _navigation;

        public ChatViewModel(IChatApi chat, IObsidianNavigationService navigation)
        {
            _chat = chat;
            _navigation = navigation;
            _chat.OnAnswer = (message) => { Messages.Add(message); };

            NavigateToContactDetailsCommand = new RelayCommand<IContact>((contact) => _navigation.NavigateTo(NavigationKeys.ContactDetails, contact));

            NaviagteToChatCommand = new RelayCommand(() =>
            {
                _navigation.NavigateTo(NavigationKeys.ChatsList, ChatId);
            });

            SendTextMessageCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(CurrentMessage.Trim()))
                {
                    var message = new TextMessage
                    {
                        Type = MessageType.Outgoing,
                        FromUserId = Server.CURRENT_USER_ID,
                        ToUserId = RecipientContact.Id,
                        Date = DateTime.Now,
                        Text = CurrentMessage,
                        ChatId = ChatId
                    };

                    Messages.Add(message);
                    _chat.SendMessage(message);

                    CurrentMessage = string.Empty;
                }
            });
        }

        public override void OnStart()
        {
            Messages = _chat.GetMessagesForChat(ChatId);
            View?.OnChatLoaded();

            UpdateRecipient();
        }

        public void UpdateRecipient()
        {
            var chat = _chat.GetChatInfo(ChatId);

            //TODO only for direct chats

            var con = chat.Contacts.Where(c => c.Id != Server.CURRENT_USER_ID);
            var recipientContact = chat.Contacts.Where(c => c.Id != Server.CURRENT_USER_ID).FirstOrDefault();

            if (recipientContact != null)
            {
                RecipientContact = recipientContact;
            }
        }
    }
}