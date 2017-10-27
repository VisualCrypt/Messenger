using System.Collections.Generic;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Ioc;
using ObsidianMobile.Core.Interfaces.ViewModels;
using ObsidianMobile.Core.Interfaces.Views;

namespace ObsidianMobile.Droid.Screen
{
    public class BaseActivity<TViewModel> : AppCompatActivity, IBaseView 
        where TViewModel : IBaseViewModel
    {
        protected TViewModel ViewModel;

        protected IList<Binding> Bindings;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DroidNavigationService.PushActivty(this);
            ViewModel = SimpleIoc.Default.GetInstance<TViewModel>();
            Bindings = new List<Binding>();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            ViewModel.OnStart();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DroidNavigationService.PopActivty(this);
        }
    }
}
