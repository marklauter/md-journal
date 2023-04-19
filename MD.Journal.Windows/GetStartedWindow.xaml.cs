// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MD.Journal.Windows.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD.Journal.Windows
{
    public static class WindowExtensions
    {
        public static void Resize(this Window window, int width, int height)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var size = new SizeInt32
            {
                Width = width,
                Height = height
            };

            appWindow.Resize(size);
        }

        public static void Center(this Window window)
        {
            var hWnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            var CenteredPosition = appWindow.Position;
            CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

            appWindow.Move(CenteredPosition);
        }
    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStartedWindow : Window
    {
        public GetStartedWindow()
        {
            this.InitializeComponent();
            this.Resize(900, 700);
            this.Center();
        }

        public GetStartedViewModel ViewModel { get; } = new GetStartedViewModel();

        private void OpenJournalButtonClickAsync(object sender, RoutedEventArgs e)
        {

        }

        private void CreateJournalButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void RecentJournalsListViewItemClickAsync(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {

        }
    }
}
