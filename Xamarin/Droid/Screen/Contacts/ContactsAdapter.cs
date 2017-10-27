using System;
using System.Collections.ObjectModel;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using ObsidianMobile.Core.Extensions;
using ObsidianMobile.Core.Interfaces.Models;

namespace ObsidianMobile.Droid.Contacts
{
    public class ContactsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<EventArgs> ItemClick;

        readonly ObservableCollection<IContact> Contacts;

        public ContactsAdapter(ObservableCollection<IContact> contacts)
        {
            Contacts = contacts;
        }

        public override int ItemCount => Contacts.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var contact = Contacts[position];
            var contactsHolder = holder as ContactsHolder;
            contactsHolder.ContactName.Text = contact.Name;
            //contactsHolder.LastMessage.Text = contact.LastMessage;
            //contactsHolder.LastMessagedate.Text = contact.LastMessageDate.ToMessageStyleString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_contact, parent, false);
            return new ContactsHolder(position => OnItemClick(position), view);
        }

        void OnItemClick(int position)
        {
            EventArgs.Empty.Raise(this, ref ItemClick);
        }
    }

    public class ContactsHolder : RecyclerView.ViewHolder
    {
        [InjectView(Resource.Id.contact_name)]
        public TextView ContactName;

        [InjectView(Resource.Id.last_message_text)]
        public TextView LastMessage;

        [InjectView(Resource.Id.last_message_date)]
        public TextView LastMessagedate;

        public ContactsHolder(Action<int> itemClick, View view) : base(view)
        {
            Cheeseknife.Inject(this, ItemView);
            ItemView.Click += delegate { itemClick(AdapterPosition); };
        }
    }
}