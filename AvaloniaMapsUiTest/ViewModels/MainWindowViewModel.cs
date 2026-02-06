using System.Collections.ObjectModel;

namespace AvaloniaMapsUiTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private decimal _objectsCount;

        public ObservableCollection<string> Collection1 { get; set; }

        public ObservableCollection<string> Collection2 { get; set; }

        public Provider Provider { get; set; }

        public SyncLayer Layer { get; set; }

        public decimal ObjectsCount
        {
            get => _objectsCount;
            set
            {
                _objectsCount = value;
                Provider.ObjectsCount = (int)value;
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

        
    }
}
