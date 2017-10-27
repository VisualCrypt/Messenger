using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ObsidianMobile.Core.Interfaces.Models;
using ObsidianMobile.Core.Interfaces.Views;

namespace ObsidianMobile.Core.Interfaces.ViewModels
{
    public interface IChatViewModel : IBaseViewModel
    {
        ObservableCollection<IMessage> Messages { get; }

        RelayCommand<IContact> NavigateToContactDetailsCommand { get; }

        RelayCommand SendTextMessageCommand { get; }

        RelayCommand NaviagteToChatCommand { get; }

        int ChatId { get; set; }

        string CurrentMessage { get; set; }

        string ChatName { get; }

        IContact RecipientContact { get; }

        IChatView View { get; set; }
    }
}