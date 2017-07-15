using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.Models.Chat.MessageCollection.Framework
{
    // EventArgs class for the CacheChanged event 

    // Implements a relatively simple cache for items based on a set of ranges
    public class ItemCacheManager<T> : IDisposable
    {
        public delegate Task<T[]> FetchDataCallbackHandler(ItemIndexRange range, CancellationToken ct);

        // Callback that will be used to request data
        readonly FetchDataCallbackHandler _fetchDataCallback;
        // Maximum number of items that can be fetched in one batch
        readonly int _maxBatchFetchSize;

        // data structure to hold all the items that are in the ranges the cache manager is looking after
        List<CacheEntryBlock<T>> _cacheBlocks;

        // list of ranges for items that are present in the cache
        ItemIndexRangeList _cachedResults;
        // Used to be able to cancel outstanding requests
        CancellationTokenSource _cancelTokenSource;
        bool _isTimeDisposed;
        bool _isTimerRestarted;


        bool _isTimerRunning;
        // Range of items that is currently being requested
        ItemIndexRange _requestInProgress;

        // List of ranges for items that are not present in the cache
        ItemIndexRangeList _requests;

        public ItemCacheManager(FetchDataCallbackHandler callback, int batchsize = 50)
        {
            _cacheBlocks = new List<CacheEntryBlock<T>>();
            _requests = new ItemIndexRangeList();
            _cachedResults = new ItemIndexRangeList();
            _fetchDataCallback = callback;
            _maxBatchFetchSize = batchsize;


            // set up a timer that is used to delay fetching data so 
            // that we can catch up if the list is scrolling fast.
            _isTimerRunning = true;
            Task.Run(async () =>
            {
                // Timer to optimize the the fetching of data so we throttle requests if the list is still changing
                while (!_isTimeDisposed)
                {
                    if (_isTimerRunning)
                    {
                        if (_isTimerRestarted)
                        {
                            _isTimerRestarted = false;
                            await Task.Delay(new TimeSpan(20*10000));
                        }
                        if (!_isTimeDisposed)
                            FetchData();
                    }
                    await Task.Delay(new TimeSpan(20*10000));
                }
            });
        }


        /// <summary>
        ///     Indexer for access to the item cache
        /// </summary>
        /// <param name="index">Item Index</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                // iterates through the cache blocks to find the item
                foreach (var block in _cacheBlocks)
                {
                    if (index >= block.FirstIndex && index <= block.LastIndex)
                    {
                        return block.Items[index - block.FirstIndex];
                    }
                }
                return default(T);
            }
            set
            {
                // iterates through the cache blocks to find the right block
                for (var i = 0; i < _cacheBlocks.Count; i++)
                {
                    var block = _cacheBlocks[i];
                    if (index >= block.FirstIndex && index <= block.LastIndex)
                    {
                        block.Items[index - block.FirstIndex] = value;
                        //register that we have the result in the cache
                        if (value != null)
                        {
                            _cachedResults.Add((uint) index, 1);
                        }
                        return;
                    }
                    // We have moved past the block where the item is supposed to live
                    if (block.FirstIndex > index)
                    {
                        AddOrExtendBlock(index, value, i);
                        return;
                    }
                }
                // No blocks exist, so creating a new block
                AddOrExtendBlock(index, value, _cacheBlocks.Count);
            }
        }

        public void Dispose()
        {
            _isTimeDisposed = true;
        }

        /// <summary>
        ///     object, CacheChangedEventArgs T
        /// </summary>
        public event EventHandler<object> CacheChanged;

        // Extends an existing block if the item fits at the end, or creates a new block
        void AddOrExtendBlock(int index, T value, int insertBeforeBlock)
        {
            if (insertBeforeBlock > 0)
            {
                var block = _cacheBlocks[insertBeforeBlock - 1];
                if (block.LastIndex == index - 1)
                {
                    var newItems = new T[block.Length + 1];
                    Array.Copy(block.Items, newItems, (int) block.Length);
                    newItems[block.Length] = value;
                    block.Length++;
                    block.Items = newItems;
                    return;
                }
            }
            var newBlock = new CacheEntryBlock<T> {FirstIndex = index, Length = 1, Items = new[] {value}};
            _cacheBlocks.Insert(insertBeforeBlock, newBlock);
        }


        /// <summary>
        ///     Updates the desired item range of the cache, discarding items that are not needed, and figuring out which items
        ///     need to be requested. It will then kick off a fetch if required.
        /// </summary>
        /// <param name="ranges">New set of ranges the cache should hold</param>
        public void UpdateRanges(ItemIndexRange[] ranges)
        {
            //Normalize ranges to get a unique set of discontinuous ranges
            ranges = NormalizeRanges(ranges);

            // Fail fast if the ranges haven't changed
            if (!HasRangesChanged(ranges))
            {
                return;
            }

            //To make the cache update easier, we'll create a new set of CacheEntryBlocks
            var newCacheBlocks = new List<CacheEntryBlock<T>>();
            foreach (var range in ranges)
            {
                var newBlock = new CacheEntryBlock<T>
                {
                    FirstIndex = range.FirstIndex,
                    Length = range.Length,
                    Items = new T[range.Length]
                };
                newCacheBlocks.Add(newBlock);
            }


            //Copy over data to the new cache blocks from the old ones where there is overlap
            var lastTransferred = 0;
            for (var i = 0; i < ranges.Length; i++)
            {
                var newBlock = newCacheBlocks[i];
                var range = ranges[i];
                var j = lastTransferred;
                while (j < _cacheBlocks.Count && _cacheBlocks[j].FirstIndex <= ranges[i].LastIndex)
                {
                    ItemIndexRange overlap;
                    ItemIndexRange[] added, removed;
                    var oldBlock = _cacheBlocks[j];
                    var oldEntryRange = new ItemIndexRange(oldBlock.FirstIndex, oldBlock.Length);
                    var hasOverlap = oldEntryRange.DiffRanges(range, out overlap, out removed, out added);
                    if (hasOverlap)
                    {
                        Array.Copy(oldBlock.Items, overlap.FirstIndex - oldBlock.FirstIndex, newBlock.Items,
                            overlap.FirstIndex - range.FirstIndex, (int) overlap.Length);
#if TRACE_DATASOURCE
                        Debug.WriteLine("│ Transfering cache items " + overlap.FirstIndex + "->" + overlap.LastIndex);
#endif
                    }
                    j++;
                    if (ranges.Length > i + 1 && oldBlock.LastIndex < ranges[i + 1].FirstIndex)
                    {
                        lastTransferred = j;
                    }
                }
            }
            //swap over to the new cache
            _cacheBlocks = newCacheBlocks;

            //figure out what items need to be fetched because we don't have them in the cache
            _requests = new ItemIndexRangeList(ranges);
            var newCachedResults = new ItemIndexRangeList();

            // Use the previous knowlege of what we have cached to form the new list
            foreach (var range in ranges)
            {
                foreach (var cached in _cachedResults)
                {
                    ItemIndexRange overlap;
                    ItemIndexRange[] added, removed;
                    var hasOverlap = cached.DiffRanges(range, out overlap, out removed, out added);
                    if (hasOverlap)
                    {
                        newCachedResults.Add(overlap);
                    }
                }
            }
            // remove the data we know we have cached from the results
            foreach (var range in newCachedResults)
            {
                _requests.Subtract(range);
            }
            _cachedResults = newCachedResults;

            StartFetchData();

#if TRACE_DATASOURCE
            s = "└ Pending requests: ";
            foreach (ItemIndexRange range in this.requests)
            {
                s += range.FirstIndex + "->" + range.LastIndex + " ";
            }
            Debug.WriteLine(s);
#endif
        }

        // Compares the new ranges against the previous ones to see if they have changed
        bool HasRangesChanged(ItemIndexRange[] ranges)
        {
            if (ranges.Length != _cacheBlocks.Count)
            {
                return true;
            }
            for (var i = 0; i < ranges.Length; i++)
            {
                var r = ranges[i];
                var block = _cacheBlocks[i];
                if (r.FirstIndex != block.FirstIndex || r.LastIndex != block.LastIndex)
                {
                    return true;
                }
            }
            return false;
        }

        // Gets the first block of items that we don't have values for
        public ItemIndexRange GetFirstRequestBlock(int maxsize = 50)
        {
            if (_requests.Count > 0)
            {
                var range = _requests[0];
                if (range.Length > 50) range = new ItemIndexRange(range.FirstIndex, 50);
                return range;
            }
            return null;
        }


        // Throttling function for fetching data. Forces a wait of 20ms before making the request.
        // If another fetch is requested in that time, it will reset the timer, so we don't fetch data if the view is actively scrolling
        void StartFetchData()
        {
            // Verify if an active request is still needed
            if (_requestInProgress != null)
            {
                if (_requests.Intersects(_requestInProgress))
                {
                    return;
                }
                _cancelTokenSource.Cancel();
            }

            //Using a timer to delay fetching data by 20ms, if another range comes in that time, then the timer is reset.
            _isTimerRestarted = true;
            _isTimerRunning = true;
        }


        // Called by the timer to make a request for data
        async void FetchData()
        {
            //Stop the timer so we don't get fired again unless data is requested
            _isTimerRunning = false;

            if (_requestInProgress != null)
            {
                // Verify if an active request is still needed
                if (_requests.Intersects(_requestInProgress))
                {
                    return;
                }
                // Cancel the existing request
                Debug.WriteLine(">" + " Cancelling request: " + _requestInProgress.FirstIndex + "->" +
                                _requestInProgress.LastIndex);
                _cancelTokenSource.Cancel();
            }

            var nextRequest = GetFirstRequestBlock(_maxBatchFetchSize);
            if (nextRequest != null)
            {
                _cancelTokenSource = new CancellationTokenSource();
                var ct = _cancelTokenSource.Token;
                _requestInProgress = nextRequest;
                try
                {
                    // Use the callback to get the data, passing in a cancellation token
                    var data = await _fetchDataCallback(nextRequest, ct);

                    if (!ct.IsCancellationRequested)
                    {

                        for (var i = 0; i < data.Length; i++)
                        {
                            var cacheIndex = nextRequest.FirstIndex + i;

                            var oldItem = this[cacheIndex];
                            var newItem = data[i];

                            if (!newItem.Equals(oldItem))
                            {
                                this[cacheIndex] = newItem;

                                // Fire CacheChanged so that the datasource can fire its INCC event, and do other work based on the item having data
                                if (CacheChanged != null)
                                {
                                    CacheChanged(this,
                                        new CacheChangedEventArgs<T>
                                        {
                                            OldItem = oldItem,
                                            NewItem = newItem,
                                            ItemIndex = cacheIndex
                                        });
                                }
                            }
                        }
                        _requests.Subtract(new ItemIndexRange(nextRequest.FirstIndex, (uint) data.Length));
                    }
                }
                    // Try/Catch is needed as cancellation is via an exception
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _requestInProgress = null;
                    // Start another request if required
                    FetchData();
                }
            }
        }


        /// <summary>
        ///     Merges a set of ranges to form a new set of non-contiguous ranges
        /// </summary>
        /// <param name="ranges">The list of ranges to merge</param>
        /// <returns>A smaller set of merged ranges</returns>
        ItemIndexRange[] NormalizeRanges(ItemIndexRange[] ranges)
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
                if (!handled)
                {
                    results.Add(range);
                }
            }
            return results.ToArray();
        }


        // Sees if the value is in our cache if so it returns the index
        public int IndexOf(T value)
        {
            foreach (var entry in _cacheBlocks)
            {
                var index = Array.IndexOf(entry.Items, value);
                if (index != -1) return index + entry.FirstIndex;
            }
            return -1;
        }

        // Type for the cache blocks
        class CacheEntryBlock<TItemtype>
        {
            public int FirstIndex;
            public TItemtype[] Items;
            public uint Length;

            public int LastIndex
            {
                get { return FirstIndex + (int) Length - 1; }
            }
        }
    }
}