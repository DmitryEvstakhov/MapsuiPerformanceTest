using ReactiveUI;
using System.Collections.ObjectModel;
using System.Timers;

namespace AvaloniaMapsUiTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _index;
        private int _index2;
        private readonly Timer _timer;
        private readonly Timer _timer2;

        private string _labelText = string.Empty;
        private string _labelText2 = string.Empty;

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
                this.RaiseAndSetIfChanged(ref _objectsCount, value);
                Provider.ObjectsCount = (int)value;
                Layer.Update();
            }
        }

        public string LabelText
        {
            get => _labelText;
            set => this.RaiseAndSetIfChanged(ref _labelText, value);
        }

        public string LabelText2
        {
            get => _labelText2;
            set => this.RaiseAndSetIfChanged(ref _labelText2, value);
        }

        public MainWindowViewModel()
        {
            Provider = new Provider();
            Layer = new SyncLayer(Provider);
            ObjectsCount = 0;

            Collection1 = new ObservableCollection<string> { "item1", "item2", "item3", "item4", "item5", "item6" };
            Collection2 = new ObservableCollection<string> { "item2_1", "item2_2", "item2_3", "item2_4", "item2_5", "item2_6" };

            LabelText = _index.ToString();
            _timer = new Timer(5000);
            _timer.AutoReset = true;
            _timer.Elapsed += (_, _) =>
            {
                LabelText = _index++.ToString();
            };
            _timer.Start();

            LabelText2 = _index2.ToString();
            _timer2 = new Timer(1000);
            _timer2.AutoReset = true;
            _timer2.Elapsed += (_, _) =>
            {
                LabelText2 = _index2++.ToString();
            };
            _timer2.Start();
        }
    }
}
