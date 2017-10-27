using System;

using Android.Support.V7.Widget;
using Android.Views;

namespace ObsidianMobile.Droid.Chat.Adapter
{
    public interface IViewHolderModelCreator
    {
        int LayoutResId { get; }

        RecyclerView.ViewHolder Create(View view);
    }
}
