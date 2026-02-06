using System;
using Avalonia.Controls;
using AvaloniaMapsUiTest.ViewModels;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Tiling;
using Mapsui.UI.Avalonia;

namespace AvaloniaMapsUiTest.Views
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel => (DataContext as MainWindowViewModel)!;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
        {
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapControl.Map.Layers.Add(ViewModel.Layer);
        }
    }
}