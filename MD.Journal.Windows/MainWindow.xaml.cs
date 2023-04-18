// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD.Journal.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.RecentRepositories = new ObservableCollection<string>();
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MDJournal");
            _ = Directory.CreateDirectory(path);
            this.gitRepository = new GitRepository();
            this.gitRepositoryHistory = new GitRepositoryHistory(path);
            this.ReadRecentRepositories();
        }

        private async void openRepositoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List,
            };

            folderPicker.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            this.journal = this.gitRepository.Open(folder.Path);
            await this.gitRepositoryHistory.AddRecentRepositoryAsync(folder.Path);
            var index = this.RecentRepositories.IndexOf(folder.Path);
            if (index > -1)
            {
                this.RecentRepositories.RemoveAt(index);
                this.RecentRepositories.Insert(0, folder.Path);
            }
            else
            {
                this.RecentRepositories.Insert(0, folder.Path);
            }
        }

        public ObservableCollection<string> RecentRepositories { get; }
        private Journal? journal;
        private readonly GitRepository gitRepository;
        private readonly GitRepositoryHistory gitRepositoryHistory;

        private async void ReadRecentRepositories()
        {
            if (this.RecentRepositories.Count == 0)
            {
                var repos = await this.gitRepositoryHistory.RecentRepositoriesAsync();
                foreach (var repo in repos)
                {
                    this.RecentRepositories.Add(repo.Path);
                }
            }
        }
    }
}
