using GalaSoft.MvvmLight;
using ObsidianMobile.Core.Interfaces.ViewModels;

namespace ObsidianMobile.Core.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase, IBaseViewModel
    {
        public virtual void OnStart() { }
    }
}