/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    5.06.2018 Pavel Khrapkin
 *
 * Cris Keyack Session 09 https://www.youtube.com/watch?v=J71UTTUGQtU
 *  Cuts and Fittings
 */
using System.Collections;
using System.Windows;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        public object TSMUI { get; private set; }

        public void CK09_SetWorkplane()
        {
            // Reset the workplane back to global
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());

            Picker Picker = new Picker();
            Part PickedPart = null;
            try
            {
                PickedPart = Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART) as Part;
            }
            catch { PickedPart = null; }
            if (PickedPart != null)
            {
                // Change the workplane to the coordinate system of the plate
                var psk = new TransformationPlane(PickedPart.GetCoordinateSystem());
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(psk);

                // Show the plate in the model and the workplane change
                Model.CommitChanges();

                // Draw Positive Z axis.
                GraphicsDrawer Drawer = new GraphicsDrawer();
                var red = new Color(1, 0, 0);
                Drawer.DrawLineSegment(new T3D.Point(0, 0, 0), new T3D.Point(0, 0, 500), red);
            }
        }

        public void CK09_ApplyFitting()
        {
            // Current Workplane. Reminder how the user had the model before you did stuff.
            TransformationPlane CurrentPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

            Picker Picker = new Picker();
            Beam PickedBeam = null;
            try
            {
                PickedBeam = (Beam) Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
            }
            catch { PickedBeam = null; }
            if (PickedBeam != null)
            {
                // Change the workplane to the coordinate system of the Beam
                var psk = new TransformationPlane(PickedBeam.GetCoordinateSystem());
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(psk);

                // Applyfitting
                Fitting BeamFitting = new Fitting();
                BeamFitting.Father = PickedBeam;
                Plane FittingPlane = new Plane();
                FittingPlane.Origin = new T3D.Point(500, 0, 0);
                FittingPlane.AxisX = new T3D.Vector(0, 0, 500);
                FittingPlane.AxisY = new T3D.Vector(0, - 500, 0);
                BeamFitting.Plane = FittingPlane;
                BeamFitting.Insert();

                // Apply Line Cut
                CutPlane BeamLineCut = new CutPlane();
                BeamLineCut.Father = PickedBeam;
                Plane BeamCutPlane = new Plane();
                BeamCutPlane.Origin = new T3D.Point(200, 0, 0);
                BeamCutPlane.AxisX = new T3D.Vector(0, 0, 500);
                // Changing the positive vs. negative value here determines which direction
                // the line cut will take away material where as fitting looks at which end
                // of beam it is closest to figure out how to cut.
                BeamCutPlane.AxisX = new T3D.Vector(0, -500, 0);
                BeamLineCut.Plane = BeamCutPlane;
                BeamLineCut.Insert();

                // SetWorkplane back to what user had before
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);

                // Show the plate in the model and the workplane change
                Model.CommitChanges();

                // Draw Positive Z axis.
                GraphicsDrawer Drawer = new GraphicsDrawer();
                var red = new Color(1, 0, 0);
                Drawer.DrawLineSegment(new T3D.Point(0, 0, 0), new T3D.Point(0, 0, 500), red);
            }
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        }

        public void CK09_PartCut()
        {
            // Reset Workplane back to global
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        }

        public void CK09_PolygonCut()
        {
            // Reset Workplane back to global
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        }
    }
}
