
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD.Journal.Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            this.window = new MainWindow
            {
                Title = "MD.Journal",
                Content = RootPage,
                //ExtendsContentIntoTitleBar = true,
            };

            //this.window.SetTitleBar(RootPage.TitleBar);
            Navigate(typeof(GetStartedPage));
            this.window.Activate();
        }

        public static void Navigate(Type pageType)
        {
            RootPage.Navigate(pageType);
        }

        public static void Navigate(Type pageType, object parameter)
        {
            RootPage.Navigate(pageType, parameter);
        }

        public static void Navigate(Type pageType, object? parameter, NavigationTransitionInfo transition)
        {
            RootPage.Navigate(pageType, parameter, transition);
        }

        private static readonly NavigationPage RootPage = new();
        private Window? window;
    }
}
