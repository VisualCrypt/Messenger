using Foundation;
using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core;
using ObsidianMobile.Core.DependencyResolution;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.iOS.Navigation;
using ObsidianMobile.iOS.Views;
using UIKit;

namespace ObsidianMobile.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Resolver.RegisterTypes();

            SetupNavigation();

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            var nav = new UINavigationController();
            Window.RootViewController = nav;

            Window.MakeKeyAndVisible();

            var navigator = SimpleIoc.Default.GetInstance<IObsidianNavigationService>();
            navigator.Initialize();

            navigator.NavigateTo(NavigationKeys.ContactsList);

            return true;
        }

        void SetupNavigation()
        {
            var navigationService = new ObsidianNavigationService();

            navigationService.Configure(NavigationKeys.Chat, typeof(ChatViewController));
            navigationService.Configure(NavigationKeys.ContactsList, typeof(ContactsListController));
            navigationService.Configure(NavigationKeys.ContactDetails, typeof(ContactDetailsViewController));

            Resolver.RegisterNavigaionService(navigationService);
        }
    }
}