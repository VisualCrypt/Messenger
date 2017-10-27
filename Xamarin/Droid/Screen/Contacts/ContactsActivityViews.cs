using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Droid.Screen;

using Com.Lilarcor.Cheeseknife;

namespace ObsidianMobile.Droid.Contacts
{
    public partial class ContactsActivity : BaseActivity<IContactListViewModel>
    {
        [InjectView(Resource.Id.recycler)]
        RecyclerView ContactsRecycler;

        [InjectView(Resource.Id.toolbar)]
        SupportToolbar Toolbar;

        [InjectView(Resource.Id.add_contact)]
        FloatingActionButton AddContact;
    }
}
