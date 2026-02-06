using System.Diagnostics;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;

namespace WpfMapsUiTest
{
    public class SyncLayer : Layer
    {
        public SyncLayer(IProvider provider)
        {
            base.DataSource = provider;
        }

        private long _index;

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            _index++;
            Debug.WriteLine($"GetFeatures: {_index} {DateTime.Now:HH:mm:ss.fff}");

            return base.GetFeatures(box, resolution);
        }

        public void Update()
        {
            OnFetchRequested();
        }
    }
}
