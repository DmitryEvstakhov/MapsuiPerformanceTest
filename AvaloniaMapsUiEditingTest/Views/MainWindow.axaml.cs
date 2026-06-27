using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaMapsUiEditingTest.Editing;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Tiling;
using Mapsui.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using AvaloniaMapsUiEditingTest.ViewModels;
using Mapsui.UI.Avalonia;

namespace AvaloniaMapsUiEditingTest.Views
{
    public partial class MainWindow : Window
    {
        private EditManager _editManager;

        public MainWindowViewModel ViewModel => (DataContext as MainWindowViewModel)!;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MapControl_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!ViewModel.IsCustomFeaturePressed && !ViewModel.IsGeomFeaturePressed)
                return;

            var position = e.GetPosition((Visual)sender);
            var worldPos = MapControl.Map.Navigator.Viewport.ScreenToWorld(position.X, position.Y);
            var sp = new ScreenPosition(position.X, position.Y);
            

            if (e.ClickCount > 1)
            {
                var wea = new WidgetEventArgs(sp, worldPos, GestureType.DoubleTap, MapControl.Map, false, GetMapInfo, GetRemoteMapInfoAsync);
                EditManipulation.OnTapped(wea, _editManager);

                if (ViewModel.IsCustomFeaturePressed)
                    ViewModel.IsCustomFeaturePressed = false;
                if (ViewModel.IsGeomFeaturePressed)
                    ViewModel.IsGeomFeaturePressed = false;
            }
            else
            {
                var wea = new WidgetEventArgs(sp, worldPos, GestureType.SingleTap, MapControl.Map, false, GetMapInfo, GetRemoteMapInfoAsync);
                EditManipulation.OnTapped(wea, _editManager, ViewModel.IsCustomFeaturePressed);
            }
        }

        private void MapControl_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            EditManipulation.OnPointerReleased(_editManager);

        }

        private void MapControl_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var position = e.GetPosition((Visual)sender);
            var worldPos = MapControl.Map.Navigator.Viewport.ScreenToWorld(position.X, position.Y);
            var sp = new ScreenPosition(position.X, position.Y);
            var wea = new WidgetEventArgs(sp, worldPos, GestureType.Hover, MapControl.Map, false, GetMapInfo, GetRemoteMapInfoAsync);

            EditManipulation.OnPointerMoved(wea, _editManager);
        }

        private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
        {
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapControl.Map.Layers.Add(new WritableLayer() { Name = "EditLayer", Style = CreateEditLayerStyle(), });
            //MapControl.SetMapRenderer(new Mapsui.Experimental.Rendering.Skia.MapRenderer());

            var editManager = new EditManager
            {
                Layer = (WritableLayer)MapControl.Map.Layers.First(l => l.Name == "EditLayer")
            };
            _editManager = editManager;
            _editManager.EditMode = Mapsui.Nts.Editing.EditMode.AddPolygon;
        }

        private static StyleCollection CreateEditLayerStyle() => new()
        {
            Styles =
            {
                CreateEditLayerBasicStyle(),
                CreateSelectedStyle(),
                CreateStyleToShowTheVertices(),
            }
        };

        private static SymbolStyle CreateStyleToShowTheVertices() => new()
        {
            Outline = new Pen(Color.Gray, 1f),
            Fill = new Brush(Color.White),
            SymbolScale = 0.5
        };

        private static VectorStyle CreateEditLayerBasicStyle() => new()
        {
            Fill = new Brush(_editModeColor),
            Line = new Pen(_editModeColor, 3),
            Outline = new Pen(_editModeColor, 3)
        };

        private static ThemeStyle CreateSelectedStyle()
            => new(f => (bool?)f["Selected"] == true ? _selectedStyle : _disableStyle);

        private static readonly Color _editModeColor = new(124, 22, 111, 180);
        private static readonly Color _pointLayerColor = new(240, 240, 240, 240);
        private static readonly Color _lineLayerColor = new(150, 150, 150, 240);
        private static readonly Color _polygonLayerColor = new(20, 20, 20, 240);

        private static readonly SymbolStyle? _selectedStyle = new()
        {
            Fill = null,
            Outline = new Pen(Color.Red, 3),
            Line = new Pen(Color.Red, 3)
        };

        private static readonly SymbolStyle? _disableStyle = new() { Enabled = false };

        private Task<MapInfo?> GetRemoteMapInfoAsync(ScreenPosition screenposition, Viewport viewport, IEnumerable<ILayer> layers)
        {

            return new Task<MapInfo?>(() => null);
        }

        private MapInfo GetMapInfo(ScreenPosition screenposition, IEnumerable<ILayer> layers)
        {
            return MapControl.GetMapInfo(screenposition, layers);
        }
    }
}