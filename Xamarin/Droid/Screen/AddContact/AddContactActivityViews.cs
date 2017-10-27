using Android.Widget;

using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using Com.Lilarcor.Cheeseknife;

using ObsidianMobile.Core.Interfaces.ViewModels;

namespace ObsidianMobile.Droid.Screen.AddContact
{
    public partial class AddContactActivity : BaseActivity<IContactDetailsViewModel>
    {
        [InjectView(Resource.Id.toolbar)]
        SupportToolbar Toolbar;

        [InjectView(Resource.Id.contact_name)]
        EditText ContactName;

        [InjectView(Resource.Id.add_contact)]
        Button AddContact;
    }
}
