using ObsidianMobile.Core.Interfaces.Api;
using ObsidianMobile.Core.Interfaces.ViewModels;

namespace ObsidianMobile.Core.ViewModels
{
    public class ChatDetailViewModel : BaseViewModel, IChatDetailViewModel
    {
        public int ChatId { get; set; }

        string _chatName;
        public string ChatName
        {
            get { return _chatName; }
            private set
            {
                _chatName = value;
                RaisePropertyChanged(nameof(ChatName));
            }
        }

        readonly IChatApi _chat;

        public ChatDetailViewModel(IChatApi chat)
        {
            _chat = chat;
        }

        public override void OnStart()
        {
            var contact = _chat.GetChatInfo(ChatId);
            if (contact == null)
            {
                return;
            }

            //ChatName = contact.Name;
        }
    }
}
