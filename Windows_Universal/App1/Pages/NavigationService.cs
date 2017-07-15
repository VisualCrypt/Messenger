using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.UWP.Pages;

namespace Obsidian.UWP.Services
{
    class NavigationService : INavigationService
    {
        // Guidelines for page transition animations
        // https://msdn.microsoft.com/en-us/library/windows/apps/jj635239.aspx

        Frame CurrentFrame => Window.Current.Content as Frame ?? new Frame();

        //public void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs)
        //{
        //    CurrentFrame.Navigate(typeof(MainPagePhone), filesPageCommandArgs, new DrillInNavigationTransitionInfo());
        //}

       

        //public void NavigateToHelpPage()
        //{
        //    CurrentFrame.Navigate(typeof(HelpPage), new DrillInNavigationTransitionInfo());
        //}

        //public void NavigateToSettingsPage()
        //{
        //    CurrentFrame.Navigate(typeof(SettingsPage), new DrillInNavigationTransitionInfo());
        //}

        public void NavigateToChatPage()
        {
            CurrentFrame.Navigate(typeof(ChatMasterPage), new DrillInNavigationTransitionInfo());
        }

        public void NavigateToHelpPage()
        {
            throw new NotImplementedException();
        }

        public void NavigateToSettingsPage()
        {
            throw new NotImplementedException();
        }
    }
}
