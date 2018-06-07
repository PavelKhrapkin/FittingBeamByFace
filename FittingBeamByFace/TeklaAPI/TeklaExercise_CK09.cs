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

        //PKh 7/6/18
        public Beam CutBeamByLine(Beam beam, CutPlane cutPlane)
        {
            // Apply Line Cut
            CutPlane BeamLineCut = new CutPlane();
            BeamLineCut.Father = beam;
            Plane BeamCutPlane = new Plane();
            BeamCutPlane.Origin = new T3D.Point(200, 0, 0);
            BeamCutPlane.AxisX = new T3D.Vector(0, 0, 500);
            // Changing the positive vs. negative value here determines which direction
            // the line cut will take away material where as fitting looks at which end
            // of beam it is closest to figure out how to cut.
            BeamCutPlane.AxisX = new T3D.Vector(0, -500, 0);
            BeamLineCut.Plane = BeamCutPlane;
            BeamLineCut.Insert();
            return beam;
        }

        public void CK09_PartCut()
        {
            // Current Workplane. Reminder how the user had the model before you did stuff.
            TransformationPlane CurrentPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

            Picker Picker = new Picker();
            Beam PickedPart = null;
            try
            {
                PickedPart = (Beam)Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
            }
            catch { PickedPart = null; }
            if (PickedPart != null)
            {
                // Change the workplane to the coordinate system of the Beam
                var psk = new TransformationPlane(PickedPart.GetCoordinateSystem());
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(psk);

                Beam BeamPartObject = new Beam();
                BeamPartObject.StartPoint = new T3D.Point(400, 0, -200);
                BeamPartObject.StartPoint = new T3D.Vector(400, 0, 200);
                BeamPartObject.Profile.ProfileString = "D200";
                BeamPartObject.Material.MaterialString = "ANTIMATERIAL";
                BeamPartObject.Class = BooleanPart.BooleanOperativeClassName;
                BeamPartObject.Name = "CUT";
                BeamPartObject.Position.Depth = Position.DepthEnum.MIDDLE;
                BeamPartObject.Position.Rotation = Position.RotationEnum.FRONT;
                BeamPartObject.Position.Plane = Position.PlaneEnum.MIDDLE;
                if (!BeamPartObject.Insert())
                {
                    Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Cut wasn't created.");
                    // SetWorkPlane back to what user had before
                    Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);
                }
                else
                {
                    BooleanPart PartCut = new BooleanPart();
                    PartCut.Father = PickedPart;
                    PartCut.OperativePart = BeamPartObject;
                    PartCut.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
                    if(!PartCut.Insert())
                    {
                        // SetWorkPlane back to what user had before
                        Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);
                    }
                    else
                    {
                        // We don't need the phisical part in the model anymore.
                        BeamPartObject.Delete();

                        // SetWorkPlane back to what user had before
                        Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);

                        // Show the fitting in the model but the user
                        //..will never see the workplane change
                        Model.CommitChanges();
                    }
                }
            }
        }

        public void CK09_PolygonCut()
        {
            // Current Workplane. Reminder how the user had the model before you did stuff.
            TransformationPlane CurrentPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

            Picker Picker = new Picker();
            Beam PickedPart = null;
            try
            {
                PickedPart = (Beam)Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
            }
            catch { PickedPart = null; }
            if (PickedPart != null)
            {
                // Change the workplane to the coordinate system of the Beam
                var psk = new TransformationPlane(PickedPart.GetCoordinateSystem());
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(psk);

                ContourPlate ContourPlateObject = new ContourPlate();
                ContourPlateObject.AssemblyNumber.Prefix = "XX";
                ContourPlateObject.AssemblyNumber.StartNumber = 1;
                ContourPlateObject.Name = "CUT";
                ContourPlateObject.PartNumber.Prefix = "xx";
                ContourPlateObject.Profile.ProfileString = "200";
                ContourPlateObject.Material.MaterialString = "ANTIMATERIAL";
                ContourPlateObject.Finish = "";
                // This is the Important Part!
                ContourPlateObject.Class = BooleanPart.BooleanOperativeClassName;
                ContourPlateObject.Position.Depth = Position.DepthEnum.MIDDLE;
                // when doing a polygon cut make sure you don't do roght alone edge
                //..or sometimes you maight get a solid error and your part will disappeared
                ContourPlateObject.AddContourPoint(new ContourPoint(new T3D.Point(-10, -10, 0), null));
                ContourPlateObject.AddContourPoint(new ContourPoint(new T3D.Point(100, -10, 0), null));
                ContourPlateObject.AddContourPoint(new ContourPoint(new T3D.Point(100, 100, 0), null));
                ContourPlateObject.AddContourPoint(new ContourPoint(new T3D.Point(-10, 100, 0), null));
                if (!ContourPlateObject.Insert())
                {
                    Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Plate wasn't created.");
                    // SetWorkPlane back to what user had before
                    Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);
                }
                else
                {
                    BooleanPart PolygonCut = new BooleanPart();
                    PolygonCut.Father = PickedPart;
                    PolygonCut.OperativePart = ContourPlateObject;
                    PolygonCut.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
                    if (!PolygonCut.Insert())
                    {
                        // SetWorkPlane back to what user had before
                        Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);
                    }
                    else
                    {
                        // We don't need the phisical part in the model anymore.
                        ContourPlateObject.Delete();

                        // SetWorkPlane back to what user had before
                        Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentPlane);

                        // Show the fitting in the model but the user
                        //..will never see the workplane change
                        Model.CommitChanges();
                    }
                }
            }
        }
    }
}
