using System.Collections.Generic;

namespace ObsidianMobile.Droid.Screen.Chat.Emoji
{
    public class EmojiPageInfo
    {
        public readonly static EmojiPageInfo Poeple = new EmojiPageInfo(Resource.Drawable.ic_smile, new List<string>
        {
            "😀", "😁", "😂", "🤣", "😃", "😄", "😅", "😆", "😉", "😊", "😋", "😎", "😍", "😘", "😗", "😙", "😚", "🙂", "🤗", "🤔", 
            "😐", "😑", "😶", "🙄", "😏", "😣", "😥", "😮", "🤐", "😯", "😪", "😫", "😴", "😌", "😛", "😜", "😝", "🤤", "😒", "😓", "😔", 
            "😕", "🙃", "🤑", "😲", "🙁", "😖", "😞", "😟", "😤", "😢", "😭", "😦", "😧", "😨", "😩", "😬", "😰", "😱", "😳", "😵", "😡", 
            "😠", "😷", "🤒", "🤕", "🤢", "🤧", "😇", "🤠", "🤡", "🤥", "🤓", "😈", "👿",
        });
        public readonly static EmojiPageInfo AnimalsAndNature = new EmojiPageInfo(Resource.Drawable.ic_animal, new List<string>
        {
            "🙈","🙉","🙊","💥","💦","💨","💫","🐵","🐒","🦍","🐶","🐕","🐩","🐺","🦊","🐱","🐈","🦁","🐯","🐅","🐆","🐴","🐎","🦄","🐮",
            "🐂","🐃","🐄", "🐷","🐖","🐗","🐽","🐏","🐑","🐐","🐪","🐫","🐘","🦏","🐭","🐁","🐀","🐹","🐰","🐇", "🐿","🦇","🐻","🐨","🐼",
            "🦃","🐔","🐓","🐣","🐤","🐥","🐦","🐧","🕊","🦅","🦆","🦉","🐸","🐊","🐢",",🐍","🐲","🐉","🐳","🐋","🐬","🐟","🐠","🐡","🦈",
            "🐙","🐚","🦀","🦐","🦑","🐌","🦋","🐛","🐜","🐝","🐞","🕷","🕸","🦂","💐","🌸","💮","🏵","🌹","🥀","🌺", "🌻", "🌼", "🌷", "🌱", 
            "🌲", "🌳", "🌴", "🌵", "🌾", "🌿", "☘", "🍀", "🍁", "🍂", "🍃", "🍄", "🌰", "🌍", "🌎", "🌏", "🌐", "🌑", "🌒","🌓", "🌔", "🌕", 
            "🌖", "🌗", "🌘", "🌙", "🌚", "🌛", "🌜", "☀ ", "🌝", "🌞", "⭐", "🌟", "🌠", "☁", "⛅", "⛈", "🌤", "🌥", "🌦", "🌧", "🌨", "🌩", 
            "🌪", "🌫", "🌬", "🌈", "☂ ", "☔ ", "⚡ ", "❄ ", "☃ ", "⛄", "☄ ", "🔥", "💧", "🌊", "🎄", "✨", "🎋", "🎍"
        });
        public readonly static EmojiPageInfo FoodAndDrink = new EmojiPageInfo(Resource.Drawable.ic_food, Poeple.Items);
        public readonly static EmojiPageInfo Activity = new EmojiPageInfo(Resource.Drawable.ic_activity, AnimalsAndNature.Items);
        public readonly static EmojiPageInfo Travels = new EmojiPageInfo(Resource.Drawable.ic_transport, Poeple.Items);
        public readonly static EmojiPageInfo Objects = new EmojiPageInfo(Resource.Drawable.ic_objects, AnimalsAndNature.Items);
        public readonly static EmojiPageInfo Symbols = new EmojiPageInfo(Resource.Drawable.ic_symbols, Poeple.Items);
        public readonly static EmojiPageInfo Flags = new EmojiPageInfo(Resource.Drawable.ic_flag, AnimalsAndNature.Items);

        public readonly static IList<EmojiPageInfo> All = new List<EmojiPageInfo>
        {
            Poeple,
            AnimalsAndNature,
            FoodAndDrink,
            Activity,
            Travels,
            Objects,
            Symbols,
            Flags
        };

        public readonly int TabIconResId;

        public readonly IList<string> Items;

        EmojiPageInfo(int tabIconResId, IList<string> items)
        {
            TabIconResId = tabIconResId;
            Items = items;
        }
    }
}
