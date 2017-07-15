using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Obsidian.UWP.Controls.Chat.Messages.Bubbles
{
    public class BubbleBase : UserControl
    {
        public string MessageID;

        public TextBlock TextBlockCipherTextBody;
        public TextBlock TextBlockPlainTextBody;
        public TextBlock TextBlockDateString;
        public TextBlock TextBlockSendMessageState;
        public ContentControl ContentView;


        public void RemoveBottomBubbleStyles()
        {
            this.Margin = new Thickness(0,0,0,0);
        }

        internal void AddBottomBubbleStyles()
        {
            this.Margin = new Thickness(0, 0, 0, 70);
        }
    }
}
