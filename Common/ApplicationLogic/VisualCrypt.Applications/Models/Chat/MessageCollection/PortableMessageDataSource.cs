using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models.Chat.MessageCollection.Framework;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels.Chat;

namespace VisualCrypt.Applications.Models.Chat.MessageCollection
{
    public class PortableMessageDataSource : INotifyCollectionChanged, IList, IDisposable
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        const int BatchSize = 10;
        readonly IDispatcher _dispatcher;
        readonly GetMessagesController _getMessagesController;
        readonly SendMessagesController _sendMessagesController;
        public PortableMessageDataSource()
        {
            _dispatcher = Service.Get<IDispatcher>();
            _getMessagesController = Service.Get<GetMessagesController>();

            _sendMessagesController = Service.Get<SendMessagesController>();
            _sendMessagesController.OnOutgoingMessagePersisted += (s, e) =>
            {
                Add(e);
            };
        }

        public ItemCacheManager<Message> ItemCache { get; private set; }

        public async Task SetTarget(string currentProfileIDOrNull, string currentContactIDOrNull)
        {
            Count = await _getMessagesController.GetMessageCountAsync(currentProfileIDOrNull, currentContactIDOrNull);
            await RecycleCache();
            NotifyCollection_CacheRecycled();
        }

        async Task RecycleCache()
        {
            if (!_dispatcher.HasThreadAccess())
                await _dispatcher.RunAsync(DoRecycleCache);
            DoRecycleCache();
        }

        void DoRecycleCache()
        {
            if (ItemCache != null)
            {
                ItemCache.CacheChanged -= NotifyCollection_CacheChanged;
                ItemCache.Dispose();
            }
            ItemCache = new ItemCacheManager<Message>(_getMessagesController.FetchDataCallback, BatchSize);
            ItemCache.CacheChanged += NotifyCollection_CacheChanged;
        }

        void NotifyCollection_CacheChanged(object sender, object args)
        {
            var cacheChangedEventArgs = (CacheChangedEventArgs<Message>)args;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace,
                cacheChangedEventArgs.OldItem,
                cacheChangedEventArgs.NewItem,
                cacheChangedEventArgs.ItemIndex));
        }

        void NotifyCollection_CacheRecycled()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Dispose()
        {
            ItemCache.Dispose();
            ItemCache = null;
        }

        #region IList Implementation

        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        public int IndexOf(object value)
        {
            return (value != null) ? ItemCache.IndexOf((Message)value) : -1;
        }



        public object this[int index]
        {
            get
            {
                // The cache will return null if it doesn't have the item. 
                // Once the item is fetched it will fire a changed event 
                // so that we can inform the list control
                return ItemCache[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count { get; private set;  }



        #region Parts of IList Not Implemented

        /// <summary>
        /// My custom Add implementation, always adds at the end of the List.
        /// </summary>
        /// <param name="value">The message that was added to the underlying database.</param>
        /// <returns>The position into which the new element was inserted, 
        /// or -1 to indicate that the item was not inserted into the collection.</returns>
        public int Add(object value)
        {
            Count += 1;
            var insertIndex = Count - 1;

            
            var handler = CollectionChanged;
            handler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, insertIndex));

            return insertIndex; 
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
