using Mapsui.Manipulations;
using Mapsui.Nts.Editing;
using Mapsui.Styles;
using Mapsui.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Extensions;
using Mapsui.Nts.Extensions;

namespace AvaloniaMapsUiEditingTest.Editing
{
    public class EditManipulation
    {
        public static bool OnTapped(WidgetEventArgs e, EditManager editManager, bool createCustomFeature = false)
        {
            var editLayer = editManager.Layer;
            if (editLayer == null)
                return false;

            if (editManager.EditMode is EditMode.DrawingPolygon or EditMode.DrawingLine)
            {
                if (e.ShiftPressed || e.GestureType == GestureType.DoubleTap || e.GestureType == GestureType.LongPress)
                {
                    if (e.GestureType != GestureType.DoubleTap) // Add last vertex but not on a double tap because it is preceded by a single tap.
                        editManager.AddVertex(e.Map.Navigator.Viewport.ScreenToWorld(e.ScreenPosition).ToCoordinate());
                    return editManager.EndEdit();
                }
                else
                    editManager.AddVertex(e.Map.Navigator.Viewport.ScreenToWorld(e.ScreenPosition).ToCoordinate());
            }
            else if (editManager.EditMode is EditMode.AddPoint or EditMode.AddLine or EditMode.AddPolygon)
                if (e.GestureType == GestureType.SingleTap)
                    editManager.AddVertex(e.Map.Navigator.Viewport.ScreenToWorld(e.ScreenPosition).ToCoordinate(), createCustomFeature);

            return false;
        }

        public static bool OnPointerReleased(EditManager editManager)
        {
            if (editManager.IsManipulating())
            {
                editManager.ResetManipulations();
                return true;
            }
            return false;
        }

        public static bool OnPointerMoved(WidgetEventArgs e, EditManager editManager)
        {
            var editLayer = editManager.Layer;
            if (editLayer == null)
                return false;

            var result = false;

            if (e.GestureType == GestureType.Hover)
            {
                editManager.HoveringVertex(e.GetMapInfo([]));
                result = false;
            }
            editLayer.DataHasChanged();
            return result;
        }
    }
}
