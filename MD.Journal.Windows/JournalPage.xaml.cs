// Licensed under the MIT License.

using MD.Journal.Journals;
using MD.Journal.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Journals.Journal journal)
            {
                this.OpenJournal(journal);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public JournalViewModel? ViewModel { get; private set; }

        public void OpenJournal(Journals.Journal journal)
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

        private void MDJournalLogoTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Navigate(typeof(GetStartedPage), null, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromLeft });
        }

        private void TagsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.FirstOrDefault();
            if (this.ViewModel is not null && item is not null and string tag)
            {
                this.ViewModel.SearchAsync(tag);
            }
        }
    }
}
