using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

namespace ObsidianMobile.Droid.Core.Extensions
{
    public static class KeyboardExtensions
    {
        const int NoFlags = 0;

        public static void ShowKeyboard(this View focusedView)
        {
            var manager = GetInputMethodManager(focusedView);
            if (!manager.IsActive) { return; }
            manager.ShowSoftInput(focusedView, ShowFlags.Implicit);
        }

        public static void HideKeyboard(this View focusedView)
        {
            var manager = GetInputMethodManager(focusedView);
            if (!manager.IsActive) { return; }
            manager.HideSoftInputFromWindow(focusedView.WindowToken, NoFlags);
        }

        public static bool IsKeyboardShown(this View probablyFocusedView)
        {
            var manager = GetInputMethodManager(probablyFocusedView);
            return manager.InvokeIsActive(probablyFocusedView);
        }

        static InputMethodManager GetInputMethodManager(View view)
        {
            return view.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
        }
    }
}
