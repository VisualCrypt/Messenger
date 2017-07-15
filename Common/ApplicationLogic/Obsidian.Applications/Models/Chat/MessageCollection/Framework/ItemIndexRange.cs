
using System;

namespace Obsidian.Applications.Models.Chat.MessageCollection.Framework
{
    public class ItemIndexRange
    {
        public readonly int FirstIndex;
        public readonly int Length;

        public ItemIndexRange(int firstIndex, int length)
        {
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException("firstIndex");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            FirstIndex = firstIndex;
            Length = length;
        }
    }
}
