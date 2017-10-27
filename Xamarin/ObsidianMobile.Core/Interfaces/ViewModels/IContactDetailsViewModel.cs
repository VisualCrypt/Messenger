using GalaSoft.MvvmLight.Command;
using ObsidianMobile.Core.Interfaces.Models;

namespace ObsidianMobile.Core.Interfaces.ViewModels
{
    public interface IContactDetailsViewModel : IBaseViewModel
    {
        RelayCommand CreateContactCommand { get; }
        RelayCommand<IContact> UpdateContactCommand { get; }

        string ContactName { get; set; }
    }
}