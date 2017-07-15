using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Obsidian.UWP.Styles
{
	static class MoreColors
	{
		public static SolidColorBrush BackgroundGridDisabledColorBrush => new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x00, 0x00));

		public static SolidColorBrush AccentColorBrush => new SolidColorBrush(Color.FromArgb(0xff, 0x01, 0x16, 0x1e));
		public static SolidColorBrush AccentColorBrush2 => new SolidColorBrush(Color.FromArgb(0xCC, 0x12, 0x45, 0x56));

		public static SolidColorBrush AccentColorBrush5 => new SolidColorBrush(Color.FromArgb(0xff, 0xef, 0xf6, 0xe0));

	}
}
