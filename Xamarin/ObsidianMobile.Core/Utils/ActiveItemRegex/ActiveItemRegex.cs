using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ObsidianMobile.Core.Utils
{
    public class ActiveItemRegex
    {
        string hashtagPattern = @"(?:^|\s|$)#[\p{L}0-9_]*";
        string mentionPattern = @"(?:^|\s|$|[.])@[\p{L}0-9_]*";
        string urlPattern = @"((http|ftp|https):\/\/)*[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

        private List<ActiveItem> RegexDeclaration (string text, string pattern, RegexType type)
        {
            var regex = new Regex(pattern);
            var items = regex.Matches(text).Cast<Match>().ToList(); ;

            var listOfItems = new List<ActiveItem>();

            foreach (var item in items)
            {
                var startIndex = item.Index;
                var lastIndex = item.Index + item.Length;

                listOfItems.Add(new ActiveItem(item.Value, type, startIndex, lastIndex));
            }

            return listOfItems;
        }

        public List<ActiveItem> GetAllElements(string text)
        {
            var activeItems = new List<ActiveItem>();

            activeItems.AddRange(RegexDeclaration(text,hashtagPattern,RegexType.Hashtag));
            activeItems.AddRange(RegexDeclaration(text, mentionPattern, RegexType.Username));
            activeItems.AddRange(RegexDeclaration(text,urlPattern,RegexType.Url));

            return activeItems;
        }
    }
}
