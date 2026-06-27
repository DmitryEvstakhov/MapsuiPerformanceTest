using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Editing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaMapsUiEditingTest.Models;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;

namespace AvaloniaMapsUiEditingTest.Editing
{
    public class EditManager
    {
        public WritableLayer? Layer { get; set; }

        private readonly DragInfo _dragInfo = new();
        private readonly AddInfo _addInfo = new();

        public EditMode EditMode { get; set; }

        public int VertexRadius { get; set; } = 12;
        public bool SelectMode { get; set; }

        public bool EndEdit()
        {
            if (_addInfo.Feature is null) return false;
            if (_addInfo.Vertices is null) return false;

            if (EditMode == EditMode.DrawingLine)
            {
                _addInfo.Vertices.RemoveAt(_addInfo.Vertices.Count - 1); // Remove the last vertex, because it is the hover vertex

                if (_addInfo.Vertices.Count < 2) // If there are not enough vertices, do not add the feature
                {
                    // And reset it back to the previous state.
                    Layer?.TryRemove(_addInfo.Feature);
                    _addInfo.Reset();
                    EditMode = EditMode.AddLine;
                    return false;
                }

                _addInfo.Feature.Geometry = new LineString(_addInfo.Vertices.ToArray());

                _addInfo.Feature = null;
                _addInfo.Vertex = null;
                EditMode = EditMode.AddLine;
            }
            else if (EditMode == EditMode.DrawingPolygon)
            {
                var polygon = _addInfo.Feature.Geometry as Polygon;
                if (polygon == null) return false;

                _addInfo.Vertices.RemoveAt(_addInfo.Vertices.Count - 1); // Remove the last vertex, because it is the hover vertex
                var linearRing = _addInfo.Vertices.ToList();
                linearRing.Add(linearRing[0].Copy()); // Add first coordinate at end to close the ring.
                _addInfo.Feature.Geometry = new Polygon(new LinearRing(linearRing.ToArray()));

                _addInfo.Feature.Modified(); // You need to clear the cache to see changes.
                _addInfo.Feature = null;
                _addInfo.Vertex = null;
                EditMode = EditMode.AddPolygon;
                Layer?.DataHasChanged();
            }

            return false;
        }

        public void HoveringVertex(MapInfo mapInfo)
        {
            if (_addInfo.Vertex != null)
            {
                _addInfo.Vertex.SetXY(mapInfo.WorldPosition);
                _addInfo.Feature?.Modified();
                Layer?.DataHasChanged();
            }
        }

        public bool AddVertex(Coordinate worldPosition, bool createCustomFeature = false)
        {
            if (EditMode == EditMode.AddPolygon)
            {
                var firstPoint = worldPosition.Copy();
                // Add a second point right away. The second one will be the 'hover' vertex
                var secondPoint = worldPosition.Copy();
                _addInfo.Vertex = secondPoint;
                _addInfo.Vertices = new List<Coordinate>([firstPoint, secondPoint]);

                if (createCustomFeature)
                {
                    _addInfo.Feature = new CustomFeature(new Polygon(ToLinearRing(_addInfo.Vertices)));
                    _addInfo.Feature.Styles = new List<IStyle>()
                    {
                        new VectorStyle() { Fill = new Brush(Color.Red), Outline = new Pen(Color.Black), Opacity = 0.5f }
                    };
                }
                else
                    _addInfo.Feature = new GeometryFeature(new Polygon(ToLinearRing(_addInfo.Vertices)));

                Layer?.Add(_addInfo.Feature);
                Layer?.DataHasChanged();
                EditMode = EditMode.DrawingPolygon;
            }
            else if (EditMode == EditMode.DrawingPolygon)
            {
                if (_addInfo.Feature is null) return false;
                if (_addInfo.Vertices is null) return false;

                // Set the final position of the 'hover' vertex (that was already part of the geometry)
                _addInfo.Vertex.SetXY(worldPosition);
                _addInfo.Vertex = worldPosition.Copy(); // and create a new hover vertex
                _addInfo.Vertices.Add(_addInfo.Vertex);
                _addInfo.Feature.Geometry = new Polygon(ToLinearRing(_addInfo.Vertices));

                _addInfo.Feature?.Modified();
                Layer?.DataHasChanged();
            }
            return false;
        }

        private static LinearRing ToLinearRing(IList<Coordinate> vertices)
        {
            var linearRing = vertices.ToList();
            linearRing.Add(linearRing[0]); // Add first coordinate at end to close the ring.
            return new LinearRing(linearRing.ToArray());
        }

        
        public void ResetManipulations()
        {
            _dragInfo.Reset();
        }

        public bool IsManipulating()
        {
            return _dragInfo.Feature != null;
        }
    }
}
