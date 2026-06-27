using ReactiveUI;

namespace AvaloniaMapsUiEditingTest.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private bool _isGeomFeaturePressed;
        private bool _isCustomFeaturePressed;


        public bool IsGeomFeaturePressed
        {
            get => _isGeomFeaturePressed;
            set
            {
                this.RaiseAndSetIfChanged(ref _isGeomFeaturePressed, value);
                if (value)
                    IsCustomFeaturePressed = false;
            }
        }

        public bool IsCustomFeaturePressed
        {
            get => _isCustomFeaturePressed;
            set
            {
                this.RaiseAndSetIfChanged(ref _isCustomFeaturePressed, value);
                if (value)
                    IsGeomFeaturePressed = false;
            }
        }
    }
}
