using System;

namespace ObsidianMobile.Core.Utils
{
    public class ActiveItem
    {
        public RegexType Type { get; set; }
        public string Text { get; set; }
        public int StartIndex { get; set; }
        public int LastIndex { get; set; }

        public ActiveItem(string text, RegexType type, int startIndex, int lastIndex)
        {
            Text = text;
            Type = type;
            StartIndex = startIndex;
            LastIndex = lastIndex;
        }

        public void Select()
        {
            switch (Type)
            {
                case RegexType.Hashtag:
                    HashtagAction();
                    break;
                case RegexType.Username:
                    UserNameAction();
                    break;
                case RegexType.Url:
                    UrlAction();
                    break;
            }
        }

        void HashtagAction ()
        {
            //Console.WriteLine("#");
            //TODO
        }

        void UrlAction()
        {
            //Console.WriteLine("https://...");
            //TODO
        }

        void UserNameAction()
        {
            //Console.WriteLine("@");
            //TODO
        }
    }
}
