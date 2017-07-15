using System;

namespace VisualCrypt.Applications.Models.Chat.MessageCollection.Framework
{
    public class CacheChangedEventArgs<T> : EventArgs
    {
        public T OldItem { get; set; }
        public T NewItem { get; set; }
        public int ItemIndex { get; set; }
    }
}