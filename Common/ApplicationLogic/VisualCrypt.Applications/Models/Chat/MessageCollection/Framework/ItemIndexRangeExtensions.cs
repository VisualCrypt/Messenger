using System;
using System.Collections.Generic;

namespace VisualCrypt.Applications.Models.Chat.MessageCollection.Framework
{
    public static class ItemIndexRangeExtensions
    {
        public static bool Equals(this ItemIndexRange rangeA, ItemIndexRange rangeB)
        {
            return rangeA.FirstIndex == rangeB.FirstIndex && rangeA.Length == rangeB.Length;
        }

        public static bool ContiguousOrOverlaps(this ItemIndexRange This, ItemIndexRange range)
        {
            return (range.FirstIndex >= This.FirstIndex && range.FirstIndex <= This.LastIndex + 1) ||
                   (range.LastIndex + 1 >= This.FirstIndex && range.LastIndex <= This.LastIndex);
        }

        public static bool Intersects(this ItemIndexRange This, ItemIndexRange range)
        {
            return (range.FirstIndex >= This.FirstIndex && range.FirstIndex <= This.LastIndex) ||
                   (range.LastIndex >= This.FirstIndex && range.LastIndex <= This.LastIndex);
        }

        public static bool Intersects(this ItemIndexRange This, int firstIndex, uint length)
        {
            var lastIndex = firstIndex + (int) length - 1;
            return (firstIndex >= This.FirstIndex && firstIndex <= This.LastIndex) ||
                   (lastIndex >= This.FirstIndex && lastIndex <= This.LastIndex);
        }

        public static ItemIndexRange Combine(this ItemIndexRange This, ItemIndexRange range)
        {
            var start = Math.Min(This.FirstIndex, range.FirstIndex);
            var end = Math.Max(This.LastIndex, range.LastIndex);

            return new ItemIndexRange(start, 1 + (uint) Math.Abs(end - start));
        }

        public static bool DiffRanges(this ItemIndexRange rangeA, ItemIndexRange rangeB,
            out ItemIndexRange inBothAandB, out ItemIndexRange[] onlyInRangeA,
            out ItemIndexRange[] onlyInRangeB)
        {
            var exA = new List<ItemIndexRange>();
            var exB = new List<ItemIndexRange>();
            int i, j;
            i = Math.Max(rangeA.FirstIndex, rangeB.FirstIndex);
            j = Math.Min(rangeA.LastIndex, rangeB.LastIndex);

            if (i <= j)
            {
                // Ranges intersect
                inBothAandB = new ItemIndexRange(i, (uint) (1 + j - i));
                if (rangeA.FirstIndex < i)
                    exA.Add(new ItemIndexRange(rangeA.FirstIndex, (uint) (i - rangeA.FirstIndex)));
                if (rangeA.LastIndex > j) exA.Add(new ItemIndexRange(j + 1, (uint) (rangeA.LastIndex - j)));
                if (rangeB.FirstIndex < i)
                    exB.Add(new ItemIndexRange(rangeB.FirstIndex, (uint) (i - rangeB.FirstIndex)));
                if (rangeB.LastIndex > j) exB.Add(new ItemIndexRange(j + 1, (uint) (rangeB.LastIndex - j)));
                onlyInRangeA = exA.ToArray();
                onlyInRangeB = exB.ToArray();
                return true;
            }
            inBothAandB = default(ItemIndexRange);
            onlyInRangeA = new[] {rangeA};
            onlyInRangeB = new[] {rangeB};
            return false;
        }

        public static ItemIndexRange Overlap(this ItemIndexRange rangeA, ItemIndexRange rangeB)
        {
            int i, j;
            i = Math.Max(rangeA.FirstIndex, rangeB.FirstIndex);
            j = Math.Min(rangeA.LastIndex, rangeB.LastIndex);

            if (i <= j)
            {
                // Ranges intersect
                return new ItemIndexRange(i, (uint) (1 + j - i));
            }
            return null;
        }
    }
}