using System.Linq;
using System.Text;
using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Cryptography.Api.Implementations
{
    static class SanitizationStringExtensions
    {
       

        public static string FilterNonWhitespaceControlCharacters(this string unprunedPassword)
        {
            Guard.NotNull(unprunedPassword);

            var charArray =
                unprunedPassword
                    .ToCharArray()
                    .Where(c => !(char.IsControl(c) && !char.IsWhiteSpace(c)))
                    .ToArray();

            return new string(charArray);
        }

        public static string CondenseAndNormalizeWhiteSpace(this string unpruned)
        {
            Guard.NotNull(unpruned);

            var sb = new StringBuilder();

            bool whiteSpaceWritten = false;
            foreach (var ch in unpruned)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (!whiteSpaceWritten)
                    {
                        sb.Append(' ');  // this turns 
                        whiteSpaceWritten = true;
                    }
                }
                else
                {
                    sb.Append(ch);
                    whiteSpaceWritten = false;
                }
            }

            return sb.ToString();

        }
    }
}