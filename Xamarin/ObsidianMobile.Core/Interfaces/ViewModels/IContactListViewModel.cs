using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ObsidianMobile.Core.Interfaces.Models;

namespace ObsidianMobile.Core.Interfaces.ViewModels
{
    public interface IContactListViewModel : IBaseViewModel
    {
        RelayCommand<IContact> NavigateToChatCommand { get; }
        RelayCommand NavigateToContactDetailsCommand { get; }

        ObservableCollection<IContact> Contacts { get; }

        IMessage GetLastMessage(int contactId);

        void UpdateContacts();
    }
}