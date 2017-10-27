using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Com.Lilarcor.Cheeseknife;

namespace ObsidianMobile.Droid.Screen.Contacts
{
    public class ContactsViewHolder : RecyclerView.ViewHolder
    {
        [InjectView(Resource.Id.contact_name)]
        public TextView ContactName;

        [InjectView(Resource.Id.last_message_text)]
        public TextView LastMessage;

        [InjectView(Resource.Id.last_message_date)]
        public TextView LastMessageDate;

        public ContactsViewHolder(View view, Action<int> click) : base(view)
        {
            Cheeseknife.Inject(this, ItemView);
            ItemView.Click += (s, e) => click?.Invoke(AdapterPosition);
        }

        public static ContactsViewHolder Create(ViewGroup parent, Action<int> click)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_contact, parent, false);
            return new ContactsViewHolder(view, click);
        }
    }
}
