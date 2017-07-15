using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Prism.Events;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Applications.Models.Chat.MessageCollection;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels.Chat;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using ItemIndexRange = VisualCrypt.Applications.Models.Chat.MessageCollection.Framework.ItemIndexRange;

namespace VisualCrypt.UWP.Controls.Chat
{
    public class ItemsSource : IItemsRangeInfo, INotifyCollectionChanged, IList, IMessageDataSourceControl
    {
        readonly IFileService _fileService;
        readonly IVisualCrypt2Service _visualCrypt2Service;
        readonly IEncryptionService _encryptionService;
        readonly MessagesRepository _messagesRepository;
        readonly AbstractSettingsManager _settingsManager;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public PortableMessageDataSource PortableMessageDataSource { get; }

        public ItemsSource()
        {
            _fileService = Service.Get<IFileService>();
            _visualCrypt2Service = Service.Get<IVisualCrypt2Service>();
            _encryptionService = Service.Get<IEncryptionService>();
            _messagesRepository = Service.Get<MessagesRepository>();
            _settingsManager = Service.Get<AbstractSettingsManager>();

            PortableMessageDataSource = new PortableMessageDataSource();
            PortableMessageDataSource.CollectionChanged += async (s, e) =>
            {
                var handler = CollectionChanged;
                await Service.Get<IDispatcher>().RunAsync(() =>
                {
                    handler?.Invoke(s, e);
                });
            };
        }

        /// <summary>
        /// Updates the ranges of items in the data source that are visible in the list control 
        /// and that are tracked in the instance of the object that implements the IItemsRangeInfo interface
        /// </summary>
        /// <param name="visibleRange"></param>
        /// <param name="trackedItems"></param>
        public void RangesChanged(Windows.UI.Xaml.Data.ItemIndexRange visibleRange, IReadOnlyList<Windows.UI.Xaml.Data.ItemIndexRange> trackedItems)
        {
            // We know that the visible range is included in the broader range so don't need to hand it to the UpdateRanges call
            // Update the cache of items based on the new set of ranges. It will callback for additional data if required
            var count = trackedItems.Count;
            var portableTrackedItems = new ItemIndexRange[count];
            for (var i = 0; i < count; i++)
            {
                var itemIndexRange = trackedItems[i];
                var trackedItemDto = new ItemIndexRange(itemIndexRange.FirstIndex, itemIndexRange.Length);
                portableTrackedItems[i] = trackedItemDto;
            }

            PortableMessageDataSource.ItemCache.UpdateRanges(portableTrackedItems);
        }

        public void Dispose()
        {

            PortableMessageDataSource.Dispose();
        }


        #region IList Implementation

        public bool Contains(object value)
        {

            return IndexOf(value) != -1;
        }

        public int IndexOf(object value)
        {
            if (value == null)
            {
                Debug.WriteLine($"IndexOf NULL is always -1.");
                return -1;
            }
            var message = (MessageDisplay)value;
            var index = PortableMessageDataSource.ItemCache.IndexOf(((MessageDisplay)value).Message);
            Debug.WriteLine($"IndexOf Message '{message}' is {index}.");
            return index;
        }



        public object this[int index]
        {
            get
            {
                // ItemCache is null when we are switching single/two column view.
                if (PortableMessageDataSource.ItemCache == null)
                    return null;
                // The cache will return null if it doesn't have the item. 
                // Once the item is fetched it will fire a changed event 
                // so that we can inform the list control
                Message message = PortableMessageDataSource.ItemCache[index];
                if (message == null)
                {
                    Debug.WriteLine($"ItemsSource[{index}] is not in the cache.");
                    return null;
                }

                Debug.WriteLine($"ItemsSource[{index}] found in cache: '{message.Body}'");

                var messageDisplay = new MessageDisplay
                {
                    ID = message.ID,
                    Body = message.Body,
                    DateString = message.DateString,
                    MessageState = message.MessageState,
                    Side = message.Side,
                    PrevSide = message.PrevSide,
                    Message = message,
                    PlainTextBody = message.PlainTextBody
                };

                message.MessageDisplay = messageDisplay;

                // pretend the message has an image url
                message.ImagePath = @"Assets\App\DSCF0160.JPG";

                // Because we don't await, messageDisplay.BitmapImage is supposed to be still null
                // after this call which returns when the fist await of the subroutine is hit.
                //StartSetBitmapImageAsync(messageDisplay, message.ImagePath);
                //if (messageDisplay.BitmapImage != null)
                //    throw new Exception();

                if (messageDisplay.PlainTextBody == null && messageDisplay.Body != null)
                {
                    TryDecryptBody(messageDisplay);
                }
                else if (messageDisplay.PlainTextBody != null && messageDisplay.Body == null)
                {
                    EncryptPlainTextBody(messageDisplay);
                }
                else Debug.WriteLine("We lost some data!");


                return messageDisplay;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        async void TryDecryptBody(MessageDisplay messageDisplay)
        {
            _encryptionService.SetPassword("password");
            var cts = new CancellationTokenSource();
            var ep = new EncryptionProgress((p) =>
            {
                //Debug.WriteLine(p.Percent + " " + p.Message);
            });
            var lrop = new LongRunningOperationContext(cts.Token, ep);
            await CryptoSemaphore.WaitAsync(cts.Token);
            // DANGER, make sure the values are not asyncronously overwritten
            // copy the ID, Body to protect it from overwriting
            var messageID = messageDisplay.ID;
            var body = messageDisplay.Body;
            await Task.Run(() =>
            {
                

                var response = _encryptionService.DecryptForDisplay(
                    FileModel.Cleartext(null, null, null, Encoding.UTF8),
                    body, lrop);
                if (response.IsSuccess)
                {
                    var clearTextContents = response.Result.ClearTextContents;
                    _messagesRepository.CachePlaintext(messageID, clearTextContents);
                    //_messagesRepository.UpdateWithCiper(messageDisplay.ID, cipherText);
                    Service.Get<IDispatcher>().RunAsync(() =>
                    {
                        messageDisplay.PlainTextBody = clearTextContents;

                        //Debug.WriteLine(messageDisplay.Body);
                    });
                }
                else
                {
                    Service.Get<IDispatcher>().RunAsync(() =>
                    {
                        messageDisplay.PlainTextBody = response.Error;

                        //Debug.WriteLine(messageDisplay.Body);
                    });
                }
            });
            CryptoSemaphore.Release();
        }

        async void EncryptPlainTextBody(MessageDisplay messageDisplay)
        {
            _encryptionService.SetPassword("password");
            var cts = new CancellationTokenSource();
            var ep = new EncryptionProgress((p) =>
            {
                //Debug.WriteLine(p.Percent + " " + p.Message);
            });
            var lrop = new LongRunningOperationContext(cts.Token, ep);
            await CryptoSemaphore.WaitAsync(cts.Token);
            await Task.Run(() =>
            {
              

                var response = _encryptionService.EncryptForDisplay(
                    FileModel.Cleartext(messageDisplay.ID, "", messageDisplay.PlainTextBody, Encoding.UTF8),
                    messageDisplay.PlainTextBody, new RoundsExponent(_settingsManager.CryptographySettings.LogRounds), lrop);
                if (response.IsSuccess)
                {
                    var cipherText = response.Result.VisualCryptText;
                    _messagesRepository.UpdateWithCiper(messageDisplay.ID, cipherText);
                    Service.Get<IDispatcher>().RunAsync(() =>
                    {
                        messageDisplay.Body = cipherText;
                        
                        //Debug.WriteLine(messageDisplay.Body);
                    });
                }
            });
            CryptoSemaphore.Release();
           
        }


        static readonly SemaphoreSlim CryptoSemaphore = new SemaphoreSlim(1);

      
        async void StartSetBitmapImageAsync(MessageDisplay messageDisplay, string imagePath)
        {
            var thumbFromService = await _fileService.GetItemThumnailAsync(imagePath);

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync((StorageItemThumbnail)thumbFromService);

            messageDisplay.BitmapImage = bitmapImage;
        }


        public int Count
        {
            get
            {
               
                var count =  PortableMessageDataSource.Count;
                Debug.WriteLine($"Count is: {count}.");
                return count;
            }
        }


        #region Parts of IList Not Implemented


        public int Add(object value)
        {
            throw new NotImplementedException();
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
