using System;
namespace ObsidianMobile.Core.Extensions
{
    public static class DateTimeExtensions
    {
        const string MessageStyleFormat = "ddd H:MM tt";

        public static string ToMessageStyleString(this DateTime datetime)
        {
            return datetime.ToString(MessageStyleFormat);
        }
    }
}
