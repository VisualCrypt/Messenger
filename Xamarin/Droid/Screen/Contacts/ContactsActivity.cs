using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Com.Lilarcor.Cheeseknife;
using GalaSoft.MvvmLight.Helpers;
using ObsidianMobile.Core.Extensions;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Droid.Screen;
using ObsidianMobile.Droid.Screen.Contacts;

namespace ObsidianMobile.Droid.Contacts
{
    [Activity(Label = "Obsidian", Theme = "@style/AppTheme", MainLauncher = true)]
    public partial class ContactsActivity : BaseActivity<IContactListViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_contacts);
            Cheeseknife.Inject(this);
            SetSupportActionBar(Toolbar);

            ViewModel.UpdateContacts();

            var adapter = ViewModel.Contacts.GetRecyclerAdapter((holder, contact, index) =>
            {
                var lastMessage = ViewModel.GetLastMessage(contact.Id);
                holder.ContactName.Text = contact.Name;

                var visibility = lastMessage == null ? ViewStates.Gone : ViewStates.Visible;
                holder.LastMessage.Visibility = visibility;
                holder.LastMessageDate.Visibility = visibility;

                if (lastMessage == null)
                {
                    return;
                }

                holder.LastMessage.Text = lastMessage.Text;
                holder.LastMessageDate.Text = lastMessage.Date.ToMessageStyleString();
            },
            (parent, i) => ContactsViewHolder.Create(parent, index => ViewModel.NavigateToChatCommand.Execute(ViewModel.Contacts[index])));

            ContactsRecycler.SetAdapter(adapter);

            AddContact.SetCommand(nameof(FloatingActionButton.Click), ViewModel.NavigateToContactDetailsCommand);
        }
    }
}
