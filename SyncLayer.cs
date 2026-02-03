using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaMapsUiTest
{
    public class SyncLayer : Layer, IFetchableSource
    {
        private long _syncCount;

        public event EventHandler<FetchRequestedEventArgs>? FetchRequested;

        private readonly LatestMailbox<FetchInfo> _latestFetchInfo = new();

        private IProvider _provider;

        public IProvider Provider
        {
            get => _provider;
            set
            {
                if (_provider == value)
                    return;
                _provider = value;
                ClearCache();
                OnPropertyChanged(nameof(DataSource));
                Extent = DataSource?.GetExtent();
                OnFetchRequested();
            }
        }

        public SyncLayer(IProvider provider)
        {
            Provider = provider;
        }

        public void Update()
        {
            OnFetchRequested();
        }

        public FetchJob[] GetFetchJobs(int activeFetches, int availableFetchSlots)
        {
            if (!Enabled)
                return [];

            if (activeFetches > 0)
                return [];

            if (_latestFetchInfo.TryTake(out var fetchInfo))
            {
                return [new FetchJob(Id, () => FetchAsync(fetchInfo))];
            }

            return [];
        }

        private async Task FetchAsync(FetchInfo fetchInfo)
        {
            if (fetchInfo.ChangeType == ChangeType.Continuous)
                throw new NotSupportedException("Continuous changes are not supported.");

            var dataSource = Provider;
            if (dataSource is null)
                return;

            await FetchAsync(fetchInfo, ++_syncCount, dataSource, DateTime.Now.Ticks);
        }

        private async Task FetchAsync(FetchInfo fetchInfo, long refreshCounter, IProvider dataSource, long timeRequested)
        {
            try
            {
                var f = await dataSource.GetFeaturesAsync(fetchInfo).ConfigureAwait(false);
                _cache = f.ToArray();

                if (_syncCount == refreshCounter)
                    Busy = false;
                OnDataChanged(new DataChangedEventArgs(Name));
            }
            catch (Exception ex)
            {
                if (_syncCount == refreshCounter)
                    Busy = false;
                OnDataChanged(new DataChangedEventArgs(ex, Name));
            }
        }

        private IFeature[] _cache = [];


        private long _index;

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            _index++;
            Debug.WriteLine($"GetFeatures: {_index} {DateTime.Now:HH:mm:ss.fff}");

            return _cache;
        }

        public new void ViewportChanged(FetchInfo fetchInfo)
        {
            Busy = true;
            _latestFetchInfo.Overwrite(fetchInfo);
        }

        protected virtual void OnFetchRequested()
        {
            FetchRequested?.Invoke(this, new FetchRequestedEventArgs(ChangeType.Discrete));
        }
    }
}
