// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MD.Journal.Journals;
using MD.Journal.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MD.Journal.Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JournalPage
        : Page
        , INotifyPropertyChanged
    {
        public JournalPage()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public JournalViewModel? ViewModel { get; private set; }

        public void Navigate(Journals.Journal journal)
        {
            this.ViewModel = new JournalViewModel(journal);
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(this.ViewModel)));
        }

        private void JournalEntriesListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.FirstOrDefault();
            if (this.ViewModel is not null && item is not null and JournalEntry journalEntry)
            {
                this.ViewModel.CurrentJournalEntry = journalEntry;
            }
        }

        private void TagsGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.FirstOrDefault();
            if (this.ViewModel is not null && item is not null and string tag)
            {
                this.ViewModel.SearchAsync(tag);
            }
        }
    }
}
