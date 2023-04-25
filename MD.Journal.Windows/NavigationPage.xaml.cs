
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD.Journal.Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        public NavigationPage()
        {
            this.InitializeComponent();
        }

        public void Navigate(Type pageType)
        {
            _ = this.ContentFrame.Navigate(pageType);
        }

        public void Navigate(Type pageType, object parameter)
        {
            _ = this.ContentFrame.Navigate(pageType, parameter);
        }
    }
}
