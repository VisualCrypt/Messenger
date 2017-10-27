using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core.Interfaces.Api;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Core.ViewModels;

namespace ObsidianMobile.Core.DependencyResolution
{
    public static class Resolver
    {
        public static void RegisterTypes()
        {
            SimpleIoc.Default.Register<IChatApi, Server>();
            SimpleIoc.Default.Register<IChatViewModel, ChatViewModel>();
            SimpleIoc.Default.Register<IContactListViewModel, ContactListViewModel>();
            SimpleIoc.Default.Register<IContactDetailsViewModel, ContactDetailsViewModel>();
            SimpleIoc.Default.Register<IChatDetailViewModel, ChatDetailViewModel>();
        }

        public static void RegisterNavigaionService(IObsidianNavigationService navigationService)
        {
            SimpleIoc.Default.Register(() => navigationService);
        }
    }
}