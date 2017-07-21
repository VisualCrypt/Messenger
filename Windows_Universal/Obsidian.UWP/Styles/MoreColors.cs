using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Obsidian.UWP.Styles
{
	static class MoreColors
	{
		/*
			<Color x:Key="Spaceblue">#FF1f4a5c</Color>
			<Color x:Key="ObsidianDark">#FF438e8b</Color>
			<Color x:Key="ObsidianMain">#FF34cdca</Color>
			<Color x:Key="ObsidianWhite">#FFffffff</Color>
			 
			 */

		public static SolidColorBrush ObsidianBlackBrush => new SolidColorBrush(Color.FromArgb(0xff, 0x11, 0x16, 0x1e));
		public static SolidColorBrush SpaceblueBrush => new SolidColorBrush(Color.FromArgb(0xff, 0x1f, 0x4a, 0x5c));
		public static SolidColorBrush ObsidianDarkBrush => new SolidColorBrush(Color.FromArgb(0xCC, 0x43, 0x8e, 0x8b));
		public static SolidColorBrush ObsidianMainBrush => new SolidColorBrush(Color.FromArgb(0xff, 0x34, 0xcd, 0xca));
		public static SolidColorBrush ObsidianWhiteBrush => new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff));
		public static SolidColorBrush DebugBrush => new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x00, 0xff));

	}
}
