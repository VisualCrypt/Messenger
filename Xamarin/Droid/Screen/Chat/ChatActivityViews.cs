using Android.Support.V7.Widget;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Droid.Screen;
using ObsidianMobile.Droid.Screen.Chat.Emoji;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ObsidianMobile.Droid.Chat
{
    public partial class ChatActivity : BaseActivity<IChatViewModel>
    {
        [InjectView(Resource.Id.send)]
        ImageButton SendButton;

        [InjectView(Resource.Id.chat_message_input)]
        EditText MessageInput;

        [InjectView(Resource.Id.recycler)]
        RecyclerView MessagesRecycler;

        [InjectView(Resource.Id.toolbar)]
        SupportToolbar Toolbar;

        [InjectView(Resource.Id.emoji_keyboard)]
        EmojiKeyboardView EmojiKeyboard;

        [InjectView(Resource.Id.show_emoji_keyboard)]
        ImageButton ShowEmoji;

        [InjectView(Resource.Id.chat_image)]
        ImageView ChatImage;

        [InjectView(Resource.Id.chat_name)]
        TextView ChatName;
    }
}
