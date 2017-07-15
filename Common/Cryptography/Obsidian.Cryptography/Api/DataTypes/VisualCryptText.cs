using System;

namespace Obsidian.Cryptography.Api.DataTypes
{
	public sealed class VisualCryptText
	{
		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public string Text
		{
			get { return _text; }
		}

		readonly string _text;

		public VisualCryptText(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			_text = text;
		}
	}
}