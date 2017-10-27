using GalaSoft.MvvmLight.Command;
using ObsidianMobile.Core.Interfaces.Api;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Core.Models;

namespace ObsidianMobile.Core.ViewModels
{
    public class ContactDetailsViewModel : BaseViewModel, IContactDetailsViewModel
    {
        public RelayCommand CreateContactCommand { get; private set; }
        public RelayCommand<IContact> UpdateContactCommand { get; private set; }

        public string ContactName { get; set; }

        readonly IChatApi _chat;
        readonly IObsidianNavigationService _navigationService;

        public ContactDetailsViewModel(IChatApi chat, IObsidianNavigationService navigationService)
        {
            _chat = chat;
            _navigationService = navigationService;

            CreateContactCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(ContactName.Trim()))
                {
                    var contact = new Contact
                    {
                        Name = this.ContactName
                    };

                    _chat.CreateContact(contact);
                    _navigationService.GoBack();
                }
            });

            UpdateContactCommand = new RelayCommand<IContact>((contact) =>
            {
                contact.Name = ContactName;

                _chat.UpdateContact(contact);
                _navigationService.GoBack();
            });
        }
    }
}