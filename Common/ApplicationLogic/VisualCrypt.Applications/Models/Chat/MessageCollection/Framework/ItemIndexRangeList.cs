﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace VisualCrypt.Applications.Models.Chat.MessageCollection.Framework
{
    /// <summary>
    ///     Represents a sorted collection of discontiguous ItemIndexRanges.
    /// </summary>
    public class ItemIndexRangeList : IList<ItemIndexRange>
    {
        readonly List<ItemIndexRange> _ranges;


        public ItemIndexRangeList()
        {
            _ranges = new List<ItemIndexRange>();
        }

        public ItemIndexRangeList(List<ItemIndexRange> ranges)
        {
            _ranges = NormalizeRanges(ranges);
        }

        public ItemIndexRangeList(ItemIndexRange[] ranges)
        {
            _ranges = NormalizeRanges(ranges);
        }

        /// <summary>
        ///     Merges the range into the rangelist, combining with existing ranges if necessary
        /// </summary>
        /// <param name="newrange">Range to merge into the collection</param>
        public void Add(ItemIndexRange newrange)
        {
            for (var i = 0; i < _ranges.Count; i++)
            {
                var existing = _ranges[i];
                if (newrange.ContiguousOrOverlaps(existing))
                {
                    existing = existing.Combine(newrange);
                    for (var j = i + 1; j < _ranges.Count; j++)
                    {
                        var next = _ranges[j];
                        if (existing.ContiguousOrOverlaps(next))
                        {
                            existing = existing.Combine(next);
                            _ranges.RemoveAt(i + 1);
                        }
                    }
                    _ranges[i] = existing;
                    return;
                }
                if (newrange.LastIndex < existing.FirstIndex)
                {
                    _ranges.Insert(i, newrange);
                    return;
                }
            }
            _ranges.Add(newrange);
        }

        public List<ItemIndexRange> ToList()
        {
            return _ranges;
        }

        public ItemIndexRange[] ToArray()
        {
            return _ranges.ToArray();
        }

        /// <summary>
        ///     Merges the range into the rangelist, combining with existing ranges if necessary
        /// </summary>
        public void Add(uint FirstIndex, uint Length)
        {
            Add(new ItemIndexRange((int) FirstIndex, Length));
        }

        /// <summary>
        ///     Removes a range from the collection, splitting existing ranges if necessary
        /// </summary>
        public void Subtract(ItemIndexRange range)
        {
            for (var idx = 0; idx < _ranges.Count; idx++)
            {
                var existing = _ranges[idx];
                if (existing.FirstIndex > range.LastIndex) return;

                int i, j;
                i = Math.Max(existing.FirstIndex, range.FirstIndex);
                j = Math.Min(existing.LastIndex, range.LastIndex);

                if (i <= j)
                {
                    if (existing.FirstIndex < i && existing.LastIndex > j)
                    {
                        //range is in the middle of existing range, so split existing into two
                        _ranges[idx] = new ItemIndexRange(existing.FirstIndex, (uint) (i - existing.FirstIndex));
                        _ranges.Insert(idx + 1, new ItemIndexRange(j + 1, (uint) (existing.LastIndex - j)));
                        return;
                    }
                    if (existing.LastIndex > j)
                    {
                        //range ends before existing so trim existing to be the remainder
                        _ranges[idx] = new ItemIndexRange(j + 1, (uint) (existing.LastIndex - j));
                        return;
                    }
                    if (existing.FirstIndex < i)
                    {
                        //range starts after existing so trim existing to the part before range
                        _ranges[idx] = new ItemIndexRange(existing.FirstIndex, (uint) (i - existing.FirstIndex));
                    }
                    else
                    {
                        //existing is overlapped by range, so remove it.
                        _ranges.RemoveAt(idx);
                    }
                    //trim the subtracted range to the remainder, and exit if complete
                    if (range.LastIndex > j)
                    {
                        range = new ItemIndexRange(j + 1, (uint) (range.LastIndex - j));
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public void Subtract(uint FirstIndex, uint Length)
        {
            Subtract(new ItemIndexRange((int) FirstIndex, Length));
        }

        public bool Intersects(ItemIndexRange range)
        {
            foreach (var r in _ranges)
            {
                if (r.Intersects(range))
                {
                    return true;
                }
            }
            return false;
        }

        // Merges contiguous or overlapping ranges to ensure the collection is discontiguous
        // Also sorts the ranges so they start in index order
        List<ItemIndexRange> NormalizeRanges(IEnumerable<ItemIndexRange> ranges)
        {
            var results = new List<ItemIndexRange>();
            foreach (var range in ranges)
            {
                var handled = false;
                for (var i = 0; i < results.Count; i++)
                {
                    var existing = results[i];
                    if (range.ContiguousOrOverlaps(existing))
                    {
                        results[i] = existing.Combine(range);
                        handled = true;
                        break;
                    }
                    if (range.FirstIndex < existing.FirstIndex)
                    {
                        results.Insert(i, range);
                        handled = true;
                        break;
                    }
                }
                if (!handled) results.Add(range);
            }
            return results;
        }

        #region IList implementation

        public int Count
        {
            get { return ((IList<ItemIndexRange>) _ranges).Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList<ItemIndexRange>) _ranges).IsReadOnly; }
        }

        public ItemIndexRange this[int index]
        {
            get { return ((IList<ItemIndexRange>) _ranges)[index]; }
            set { ((IList<ItemIndexRange>) _ranges)[index] = value; }
        }


        public IEnumerator<ItemIndexRange> GetEnumerator()
        {
            return ((IEnumerable<ItemIndexRange>) _ranges).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ItemIndexRange>) _ranges).GetEnumerator();
        }

        public int IndexOf(ItemIndexRange item)
        {
            return ((IList<ItemIndexRange>) _ranges).IndexOf(item);
        }

        public void Insert(int index, ItemIndexRange item)
        {
            ((IList<ItemIndexRange>) _ranges).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ItemIndexRange>) _ranges).RemoveAt(index);
        }

        public void Clear()
        {
            ((IList<ItemIndexRange>) _ranges).Clear();
        }

        public bool Contains(ItemIndexRange item)
        {
            return ((IList<ItemIndexRange>) _ranges).Contains(item);
        }

        public void CopyTo(ItemIndexRange[] array, int arrayIndex)
        {
            ((IList<ItemIndexRange>) _ranges).CopyTo(array, arrayIndex);
        }

        public bool Remove(ItemIndexRange item)
        {
            return ((IList<ItemIndexRange>) _ranges).Remove(item);
        }

        #endregion
    }
}