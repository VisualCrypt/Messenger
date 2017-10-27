using System;
using Android.Support.V7.Widget;
using Android.Views;

namespace ObsidianMobile.Droid.Chat.Adapter
{
    public abstract class ViewHolderModel<T> : RecyclerView.ViewHolder
    {
        public ViewHolderModel(View view) : base(view)
        {
        }

        public abstract void Update(T data, bool isCurrentUser);
    }
}
