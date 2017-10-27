using System;

using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace ObsidianMobile.Droid.Core
{
    public class KeyboardHeightObserver : PopupWindow
    {
        public event EventHandler<KeyboardHeightEventArgs> KeyboardHeightMeasured;

        public int KeyboardHeight { get; private set; }

        public bool IsKeyboardRevealed => KeyboardHeight > 0;

        View _windowRootView;
        View _popupView;
        int _windowHeight;

        public KeyboardHeightObserver(Activity activity)
        {
            _popupView = new FrameLayout(activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent)
            };
            ContentView = _popupView;
            Height = ViewGroup.LayoutParams.MatchParent;
            Width = 0;

            SoftInputMode = SoftInput.AdjustResize | SoftInput.StateAlwaysVisible;
            InputMethodMode = InputMethod.Needed;

            _windowRootView = activity.Window.DecorView;

            _popupView.ViewTreeObserver.GlobalLayout += OnGlovalLayoutChange;

            var windowPoint = new Point();
            activity.WindowManager.DefaultDisplay.GetSize(windowPoint);
            _windowHeight = windowPoint.Y;
        }

        public void Start()
        {
            if (!IsShowing && _windowRootView.WindowToken != null)
            {
                ShowAtLocation(_windowRootView, GravityFlags.NoGravity, 0, 0);
            }
        }

        void OnGlovalLayoutChange(object sender, EventArgs e)
        {
            var contentRect = new Rect();
            _popupView.GetWindowVisibleDisplayFrame(contentRect);

            KeyboardHeight = _windowHeight - contentRect.Bottom;
            OnKeyboardHeightMeasured(KeyboardHeight);
        }

        void OnKeyboardHeightMeasured(int height)
        {
            var handler = KeyboardHeightMeasured;
            if (handler != null)
            {
                KeyboardHeightMeasured(this, new KeyboardHeightEventArgs(height));
            }
        }
    }

    public class KeyboardHeightEventArgs : EventArgs
    {
        public int Height { get; private set; }

        public KeyboardHeightEventArgs(int height)
        {
            Height = height;
        }
    }
}
