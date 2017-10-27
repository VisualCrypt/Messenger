using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ObsidianMobile.Core.Interfaces.Navigation;
using UIKit;

namespace ObsidianMobile.iOS.Navigation
{
    public class ObsidianNavigationService : IObsidianNavigationService, INotifyPropertyChanged
    {
        public bool CanGoBack
        {
            get
            {
                return _navigationController.ViewControllers.Count() > 1;
            }
        }

        UINavigationController _navigationController;

        readonly Dictionary<string, Type> _pagesByKey;

        string _currentPageKey;

        public string CurrentPageKey
        {
            get
            {
                return _currentPageKey;
            }

            private set
            {
                if (_currentPageKey == value)
                {
                    return;
                }

                _currentPageKey = value;
                OnPropertyChanged("CurrentPageKey");
            }
        }

        public object Parameter { get; private set; }

        public ObsidianNavigationService()
        {
            _pagesByKey = new Dictionary<string, Type>();
        }

        public void Initialize()
        {
            _navigationController = UIApplication.SharedApplication.KeyWindow.RootViewController as UINavigationController;
        }

        public void GoBack()
        {
            _navigationController.InvokeOnMainThread(() =>
            {
                _navigationController.PopViewController(true);

                var type = _navigationController.ViewControllers.Last().GetType();
                var key = _pagesByKey.First(x => x.Value == type).Key;
                CurrentPageKey = key;
            });
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public virtual void NavigateTo(string pageKey, object parameter)
        {
            Type type;
            if (_pagesByKey.TryGetValue(pageKey, out type))
            {
                var instance = parameter != null ? Activator.CreateInstance(type, parameter) : Activator.CreateInstance(type);
                var controller = instance as UIViewController;

                PushViewController(controller, true);
                CurrentPageKey = pageKey;
            }
        }

        void PushViewController(UIViewController controller, bool animated)
        {
            _navigationController.InvokeOnMainThread(() =>
            {
                _navigationController.PushViewController(controller, animated);
            });
        }

        public void Configure(string key, Type pageType)
        {
            if (!_pagesByKey.ContainsKey(key))
            {
                _pagesByKey.Add(key, pageType);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
