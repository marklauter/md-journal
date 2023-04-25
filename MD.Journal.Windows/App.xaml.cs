
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
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
                Content = NavigationPage,
            };

            Navigate(typeof(GetStartedPage));
            this.window.Activate();
        }

        public static void Navigate(Type pageType)
        {
            NavigationPage.Navigate(pageType);
        }

        public static void Navigate(Type pageType, object parameter)
        {
            NavigationPage.Navigate(pageType, parameter);
        }

        private static readonly NavigationPage NavigationPage = new();
        private Window? window;
    }
}
