using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace AvaloniaMapsUiEditingTest.Models
{
    public class CustomFeature : GeometryFeature, ICustomFeature
    {
        public string Name { get; set; }
        public new long Id { get; private set; }

        public CustomFeature(Geometry geometry)
        {
            Geometry = geometry;
        }
    }
}
