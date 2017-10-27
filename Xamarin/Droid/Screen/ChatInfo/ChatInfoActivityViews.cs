using Android.Widget;

using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using Com.Lilarcor.Cheeseknife;

using ObsidianMobile.Core.Interfaces.ViewModels;

namespace ObsidianMobile.Droid.Screen.ChatInfo
{
    public partial class ChatInfoActivity : BaseActivity<IChatDetailViewModel>
    {
        [InjectView(Resource.Id.toolbar)]
        SupportToolbar Toolbar;

        [InjectView(Resource.Id.chat_name)]
        TextView ChatName;

        [InjectView(Resource.Id.back)]
        ImageButton Back;
    }
}
