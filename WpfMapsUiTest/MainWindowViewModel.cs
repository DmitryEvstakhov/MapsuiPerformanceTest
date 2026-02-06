using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfMapsUiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _objectsCount;

        public ObservableCollection<string> Collection1 { get; set; }

        public ObservableCollection<string> Collection2 { get; set; }

        public Provider Provider { get; set; }

        public SyncLayer Layer { get; set; }

        public int ObjectsCount
        {
            get => _objectsCount;
            set
            {
                SetField(ref _objectsCount, value);

                Provider.ObjectsCount = value;
                Layer.Update();
            }
        }

        public MainWindowViewModel()
        {
            Provider = new Provider();
            Layer = new SyncLayer(Provider);
            ObjectsCount = 100;

            Collection1 = new ObservableCollection<string> { "item1", "item2", "item3", "item4", "item5", "item6" };
            Collection2 = new ObservableCollection<string> { "item2_1", "item2_2", "item2_3", "item2_4", "item2_5", "item2_6" };
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
