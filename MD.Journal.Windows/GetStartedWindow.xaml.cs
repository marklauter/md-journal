// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MD.Journal.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Storage;
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

            var userAppDataPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MDJournal");

            this.ViewModel = new GetStartedViewModel(userAppDataPath);
        }

        public GetStartedViewModel ViewModel { get; }

        private async void OpenJournalButtonClickAsync(object sender, RoutedEventArgs e)
        {
            var folder = await this.PickFolderAsync("Open journal");
            if (folder is not null)
            {
                _ = await this.ViewModel.OpenJournalAsync(folder.Path);
            }
        }

        private async Task<StorageFolder?> PickFolderAsync(string commitButtonText)
        {
            var openFolderDialog = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List,
                CommitButtonText = commitButtonText,
            };

            openFolderDialog.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openFolderDialog, hwnd);

            return await openFolderDialog.PickSingleFolderAsync();
        }

        private async void CreateJournalButtonClickAsync(object sender, RoutedEventArgs e)
        {
            var folder = await this.PickFolderAsync("Select journal location");
            if (folder is not null)
            {
                var journalName = await this.ShowCreateJournalDialogAsync("Create new journal");
                if (!String.IsNullOrEmpty(journalName))
                {
                    folder = await folder.CreateFolderAsync(journalName);
                    _ = await this.ViewModel.OpenJournalAsync(folder.Path);
                }
            }
        }

        private async Task<string> ShowCreateJournalDialogAsync(string title)
        {
            var journalNameTextBox = new TextBox
            {
                Margin = new Thickness(20, 20, 20, 20),
                AcceptsReturn = false,
                TextWrapping = TextWrapping.NoWrap,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 32,
                FontSize = 14,
                PlaceholderText = "Journal name",
                Style = Application.Current.Resources["DefaultTextBoxStyle"] as Style
            };

            var dialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = title,
                Content = journalNameTextBox, //new NewJournalPage(),
                PrimaryButtonText = "Create journal",
                CloseButtonText = "Cancel"
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary
                ? journalNameTextBox.Text
                : String.Empty;
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
