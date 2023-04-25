// Licensed under the MIT License.

using MD.Journal.Recents;
using MD.Journal.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MD.Journal.Windows
{
    public sealed partial class GetStartedPage : Page
    {
        public GetStartedPage()
        {
            this.InitializeComponent();

            var userAppDataPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MDJournal");

            this.ViewModel = new GetStartedViewModel(userAppDataPath);
        }

        public GetStartedViewModel ViewModel { get; }

        private void OpenJournal(Journals.Journal journal)
        {
            App.Navigate(typeof(JournalPage), journal, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private async void OpenJournalButtonClickAsync(object sender, RoutedEventArgs e)
        {
            var folder = await this.PickFolderAsync("Open journal");
            if (folder is not null)
            {
                var journal = await this.ViewModel.OpenJournalAsync(folder.Path);
                this.OpenJournal(journal);
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
                    var journal = await this.ViewModel.OpenJournalAsync(folder.Path);
                    this.OpenJournal(journal);
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

        private async void RecentJournalsListViewItemClickAsync(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem is RecentItem entry)
            {
                var journal = await this.ViewModel.OpenJournalAsync(entry.Key);
                this.OpenJournal(journal);
            }
        }
    }
}
