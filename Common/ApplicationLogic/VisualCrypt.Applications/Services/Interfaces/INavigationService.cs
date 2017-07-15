using VisualCrypt.Applications.Models;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public  interface INavigationService
    {
      
        void NavigateToHelpPage();
        void NavigateToSettingsPage();
        void NavigateToChatPage();
    }
}