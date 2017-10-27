using Android.App;
using Android.OS;
using Android.Views;

using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using Com.Lilarcor.Cheeseknife;

using ObsidianMobile.Core.Interfaces.ViewModels;
using GalaSoft.MvvmLight.Helpers;

namespace ObsidianMobile.Droid.Screen.ChatInfo
{
    [Activity(Label = "ChatInfoActivity")]
    public partial class ChatInfoActivity : BaseActivity<IChatDetailViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_chat_info);
            Cheeseknife.Inject(this);
            SetSupportActionBar(Toolbar);

            if (Intent.Extras != null)
            {
                ViewModel.ChatId = Intent.Extras.GetInt(DroidNavigationService.ParameterKey);
            }

            Back.Click += (s, e) => Finish();

            Bindings.Add(this.SetBinding(() => ViewModel.ChatName, () => ChatName.Text, BindingMode.OneWay));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_chat_info, menu);
            return true;
        }
    }
}
