using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ObsidianMobile.Core.Interfaces.Api;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.Core.Interfaces.ViewModels;

namespace ObsidianMobile.Core.ViewModels
{
    public class ContactListViewModel : BaseViewModel, IContactListViewModel
    {
        public RelayCommand<IContact> CreateContactCommand { get; private set; }
        public RelayCommand<IContact> UpdateContactCommand { get; private set; }
        public RelayCommand<IContact> NavigateToChatCommand { get; private set; }

        public RelayCommand NavigateToContactDetailsCommand { get; private set; }

        public ObservableCollection<IContact> Contacts { get; private set; }

        IChatApi _chat;
        IObsidianNavigationService _navigationService;

        public ContactListViewModel(IChatApi chat, IObsidianNavigationService navigationService)
        {
            _chat = chat;
            _navigationService = navigationService;

            CreateContactCommand = new RelayCommand<IContact>(_chat.CreateContact);
            UpdateContactCommand = new RelayCommand<IContact>(_chat.UpdateContact);
            NavigateToContactDetailsCommand = new RelayCommand(() => _navigationService.NavigateTo(NavigationKeys.ContactDetails));

            NavigateToChatCommand = new RelayCommand<IContact>((contact) =>
            {
                var contacts = new List<IContact>() { Server.CurrentUser, contact };
                var chatId = _chat.GetChatIdForContacts(contacts);

                _navigationService.NavigateTo(NavigationKeys.Chat, chatId);
            });
        }

        public void UpdateContacts()
        {
            Contacts = _chat.GetContacts();
        }

        public IMessage GetLastMessage(int contactId)
        {
            return _chat
                .GetMessagesForChat(contactId)
                .LastOrDefault(message => message.ToUserId == Server.DEFAULT_СOMPANION_ID);
        }
    }
}