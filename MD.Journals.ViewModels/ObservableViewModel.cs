using System.ComponentModel;

namespace MD.Journals.ViewModels
{
    internal class ObservableViewModel
        : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
