using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using ObsidianMobile.Core;
using ObsidianMobile.Core.Interfaces.Navigation;
using ObsidianMobile.Droid.Chat;
using ObsidianMobile.Droid.Contacts;
using ObsidianMobile.Droid.Screen.AddContact;
using ObsidianMobile.Droid.Screen.ChatInfo;

namespace ObsidianMobile.Droid
{
    public class DroidNavigationService : IObsidianNavigationService
    {
        public bool CanGoBack => false;

        public const string ParameterKey = nameof(ParameterKey);

        IDictionary<string, Type> _keysToActivityTypes;

        IDictionary<Type, Action<Bundle, object, string>> _typesToBundleFilling;

        public static ActivitiesStack Activities;

        public object Parameter { get; private set; }

        public string CurrentPageKey { get; private set; }

        public void GoBack()
        {
            var activity = Activities.Peek();
            if (activity == null)
            {
                return;
            }
            activity.Finish();
        }

        public void Initialize()
        {
            _keysToActivityTypes = new Dictionary<string, Type>
            {
                { NavigationKeys.ContactDetails, typeof(AddContactActivity) },
                { NavigationKeys.ContactsList, typeof(ContactsActivity) },
                { NavigationKeys.ChatsList, typeof(ChatInfoActivity) },
                { NavigationKeys.Chat, typeof(ChatActivity) },
            };

            _typesToBundleFilling = new Dictionary<Type, Action<Bundle, object, string>>
            {
                { typeof(int), (bundle, param, key) => bundle.PutInt(key, (int)param) }
            };

            Activities = new ActivitiesStack();
        }

        public void NavigateTo(string pageKey)
        {
            if (!_keysToActivityTypes.ContainsKey(pageKey))
            {
                return;
            }

            Activity activity = null;
            if ((activity = Activities.Peek()) != null)
            {
                activity.StartActivity(_keysToActivityTypes[pageKey]);
            }
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            if (!_keysToActivityTypes.ContainsKey(pageKey))
            {
                return;
            }

            Activity activity = null;
            if ((activity = Activities.Peek()) != null)
            {
                var bundle = new Bundle();

                var paramType = parameter.GetType();
                if (_typesToBundleFilling.ContainsKey(paramType))
                {
                    _typesToBundleFilling[paramType].Invoke(bundle, parameter, ParameterKey);
                }

                var intent = new Intent(activity, _keysToActivityTypes[pageKey]).PutExtras(bundle);
                activity.StartActivity(intent);
            }
        }

        public static void PushActivty(Activity activity)
        {
            Activities.Push(activity);
        }

        public static void PopActivty(Activity activity)
        {
            if (Activities.Peek().GetType() == activity.GetType())
            {
                Activities.Pop();
            }
            Activities.Push(activity);
        }
    }
}
