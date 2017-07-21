using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Language.Strings;
using Obsidian.UWP.Core.Data;
using Obsidian.UWP.Core.Services;
using Obsidian.UWP.Pages;
using Obsidian.UWP.Services;
using Obsidian.UWP.Styles;

namespace Obsidian.UWP
{
    public sealed partial class App
    {
        public Container Container { get; set; }

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            BootstrapperUWP.Run(this);
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            try
            {
                if (args?.ShareOperation?.Data == null)
                    return;
                if (!args.ShareOperation.Data.Contains(StandardDataFormats.Text))
                {
                    args.ShareOperation.ReportError("VisualCrypt can only handle text data at this time.");
                    return;
                }

                var textReceived = await args.ShareOperation.Data.GetTextAsync();
                BootstrapperUWP.StopMeasureStartupTime(Container);
                
                var rootFrame = await GetOrCreateRootFrame();

                await rootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Window.Current.Content = rootFrame;
                    Container.Get<INavigationService>().NavigateToChatPage();
                    Window.Current.Activate();
                });

            }
            catch (Exception e)
            {
                args?.ShareOperation?.ReportError("VisualCrypt: " + e.Message);
            }
        }

        IStorageItem _file;
        BasicProperties _props;
        Frame _rootFrame;
        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            try
            {
                _file = args.Files[0];
                _props = await _file.GetBasicPropertiesAsync();
                BootstrapperUWP.StopMeasureStartupTime(Container);
                _rootFrame = await GetOrCreateRootFrame();
                Window.Current.Content = _rootFrame;
                Window.Current.Activate();

                var timer = new DispatcherTimer();
                timer.Tick += WorkAroundLayoutBugWithMultipleScreens;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                timer.Start();
            }
            catch (Exception)
            {

            }
        }

        async void WorkAroundLayoutBugWithMultipleScreens(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();
            await _rootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var fileservice = (FileService)Container.Get<IFileService>();
                string fileToken = StorageApplicationPermissions.FutureAccessList.Add(_file, _file.Path);
                fileservice.AccessTokens[_file.Path] = fileToken; // add or replace

                Container.Get<INavigationService>().NavigateToChatPage();

            });
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await FStoreInitializer.InitFStore();
            var rootFrame = await GetOrCreateRootFrame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(ChatMasterPage), e.Arguments);
            Window.Current.Activate();
            BootstrapperUWP.StopMeasureStartupTime(Container);
        }


        static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var frame = Window.Current.Content as Frame ?? new Frame();
            frame.Navigate(typeof(ChatMasterPage));
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

         async Task<Frame> GetOrCreateRootFrame()
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                Current.DebugSettings.EnableFrameRateCounter = false;
#endif

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null)
                return rootFrame;

            rootFrame = new Frame();

            SetLanguage(rootFrame);

            await ConfigureUI();
            rootFrame.NavigationFailed -= OnNavigationFailed;
            rootFrame.NavigationFailed += OnNavigationFailed;
            return rootFrame;
        }

        void SetLanguage(Frame rootFrame)
        {
            var resourceWrapper = Container.Get<ResourceWrapper>();

            var languageResources = "en";
            var language = ApplicationLanguages.Languages[0];

            foreach (var bcp47LanguageTag in ApplicationLanguages.Languages)
            {
                var twoLetterISO = new CultureInfo(bcp47LanguageTag).TwoLetterISOLanguageName.ToLowerInvariant();
                if (!resourceWrapper.Info.AvailableCultures.Contains(twoLetterISO))
                    continue;

                languageResources = twoLetterISO;
                language = bcp47LanguageTag;
                break;
            }
            rootFrame.Language = language;
            resourceWrapper.Info.SwitchCulture(languageResources);
        }

        public static bool IsLandscape(WindowSizeChangedEventArgs args)
        {
            return args.Size.Width > args.Size.Height;
        }

        static async Task ConfigureUI()
        {
            // Window Control
            //ApplicationView.PreferredLaunchViewSize = new Size { Height = 550, Width = 360 };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size { Width = 320, Height = 400 });


            // Code for programmatic runtime resize
            // ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = 1000, Height = 550 });


            // TitleBar Control
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = MoreColors.ObsidianBlackBrush.Color;
            titleBar.InactiveBackgroundColor = MoreColors.ObsidianBlackBrush.Color;
            titleBar.ButtonBackgroundColor = MoreColors.ObsidianBlackBrush.Color;
            titleBar.ButtonInactiveBackgroundColor = MoreColors.ObsidianBlackBrush.Color;

            titleBar.InactiveForegroundColor = MoreColors.ObsidianWhiteBrush.Color;
            titleBar.ForegroundColor = MoreColors.ObsidianWhiteBrush.Color;
            titleBar.ButtonForegroundColor = MoreColors.ObsidianWhiteBrush.Color;
            titleBar.ButtonHoverForegroundColor = MoreColors.ObsidianWhiteBrush.Color;
            titleBar.ButtonPressedForegroundColor = MoreColors.ObsidianWhiteBrush.Color;
            titleBar.ButtonInactiveForegroundColor = MoreColors.ObsidianWhiteBrush.Color;

            titleBar.ButtonHoverBackgroundColor = MoreColors.ObsidianBlackBrush.Color;
            titleBar.ButtonPressedBackgroundColor = MoreColors.SpaceblueBrush.Color;


            if (IsPhone())
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (Window.Current.Bounds.Width > Window.Current.Bounds.Height)
                    await statusBar.HideAsync();
                else
                    await statusBar.ShowAsync();
                //applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                statusBar.BackgroundColor = MoreColors.SpaceblueBrush.Color;
                statusBar.ForegroundColor = MoreColors.ObsidianWhiteBrush.Color;
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.Text = "thepitext";
                statusBar.ProgressIndicator.ProgressValue = 0;
                await statusBar.ProgressIndicator.ShowAsync();
                Window.Current.SizeChanged += async (sender, args) =>
                {
                    if (IsLandscape(args))
                    {
                        await statusBar.HideAsync();
                    }
                    else
                    {
                        await statusBar.ShowAsync();
                    }
                };
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }

        public static bool IsPhone()
        {
            return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
        }



    }
}
