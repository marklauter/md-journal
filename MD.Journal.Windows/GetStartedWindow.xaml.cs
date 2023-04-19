// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MD.Journal.Windows.ViewModels;
using Microsoft.UI.Xaml;
using System;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD.Journal.Windows
{
    public sealed partial class GetStartedWindow : Window
    {
        public GetStartedWindow()
        {
            this.InitializeComponent();
            this.Resize(900, 700);
            this.Center();
        }

        public GetStartedViewModel ViewModel { get; } = new GetStartedViewModel();

        private async void OpenJournalButtonClickAsync(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List,
            };

            openFolderDialog.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openFolderDialog, hwnd);

            var folder = await openFolderDialog.PickSingleFolderAsync();
            if (folder is not null)
            {
                _ = await this.ViewModel.OpenJournalAsync(folder.Path);
            }
        }

        private void CreateJournalButtonClick(object sender, RoutedEventArgs e)
        {
            // todo: add create folder dialog
            throw new NotImplementedException();
        }

        private void RecentJournalsListViewItemClickAsync(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem is RecentJournalEntry entry)
            {
                _ = this.ViewModel.OpenJournalAsync(entry.Path);
            }
        }
    }
}
