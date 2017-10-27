using Android.App;
using Android.OS;
using Android.Widget;

using Com.Lilarcor.Cheeseknife;

using GalaSoft.MvvmLight.Helpers;

using ObsidianMobile.Core.Interfaces.ViewModels;

namespace ObsidianMobile.Droid.Screen.AddContact
{
    [Activity(Label = "Add Contact")]
    public partial class AddContactActivity : BaseActivity<IContactDetailsViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_contact);
            Cheeseknife.Inject(this);
            SetSupportActionBar(Toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            AddContact.SetCommand(nameof(Button.Click), ViewModel.CreateContactCommand);
            Bindings.Add(this.SetBinding(() => ContactName.Text, () => ViewModel.ContactName, BindingMode.OneWay));
        }
    }
}
