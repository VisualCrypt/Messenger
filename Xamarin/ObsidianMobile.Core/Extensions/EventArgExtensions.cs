using System;
using System.Threading;

namespace ObsidianMobile.Core.Extensions
{
    public static class EventArgExtensions
    {
        public static void Raise<TEventArgs>(this TEventArgs e, 
                                             object sender, 
                                             ref EventHandler<TEventArgs> eventDelegate) 
            where TEventArgs : EventArgs
        {
            Volatile.Read(ref eventDelegate)?.Invoke(sender, e);
        }
    }
}
