using GalaSoft.MvvmLight.Views;

namespace ObsidianMobile.Core.Interfaces.Navigation
{
    public interface IObsidianNavigationService : INavigationService
    {
        object Parameter { get; }
        void Initialize();

        bool CanGoBack { get; }
    }
}