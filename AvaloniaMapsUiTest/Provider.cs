using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapsui.Styles;

namespace AvaloniaMapsUiTest
{
    public class Provider : IProvider
    {
        private readonly Dictionary<int, VectorStyle> _stylesCache = new();
        private readonly Random _rnd = new();

        public int ObjectsCount { get; set; }

        public Provider()
        {
            ObjectsCount = 100;

            for (var i = 0; i < 10; i++)
            {
                _stylesCache.Add(i, CreateVectorStyle());
            }
        }

        private Color CreateRandomColor()
        {
            var rgb = new byte[3];
            _rnd.NextBytes(rgb);
            return new Color(rgb[0], rgb[1], rgb[2]);
        }

        private VectorStyle CreateVectorStyle()
        {
            return new VectorStyle
            {
                Line = new Pen(CreateRandomColor(), _rnd.Next(1, 5)),
                Outline = null
            };
        }

        private VectorStyle GetStyle()
        {
            return _stylesCache[_rnd.Next(10)];
        }

        public MRect GetExtent()
        {
            return new MRect(0, 0);
        }

        public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
        {
            return await Task.Run(() =>
            {
                var features = new List<IFeature>();
                for (var i = 0; i < ObjectsCount; i++)
                {
                    features.Add(CreateLineFeature(fetchInfo.Extent));
                }
                return features;
            }).ConfigureAwait(false);
        }

        public string? CRS { get; set; }

        private IFeature CreateLineFeature(MRect extent)
        {
            var f = new GeometryFeature(CreateLineString(extent));
            f.Styles.Add(GetStyle());
            return f;
        }

        private LineString CreateLineString(MRect extent)
        {
            var points = new List<Coordinate>();

            for (var i = 0; i < _rnd.Next(2, 30); i++)
                points.Add(CreateCoordinate(extent));

            return new LineString(points.ToArray());
        }

        private Coordinate CreateCoordinate(MRect extent)
        {
            return new Coordinate(_rnd.Next((int)extent.MinX, (int)extent.MaxX), _rnd.Next((int)extent.MinY, (int)extent.MaxY));
        }
    }
}
