/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    3.06.2018 Pavel Khrapkin
 *
 *  Cris Keyack Session 08 https://www.youtube.com/watch?v=XZswHuZykzU
 *  Write Selected Beam Data to Text File
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
        public void CK08_CreatePlate()
        {
            ArrayList PickedPoints = null;
            Picker Picker = new Picker();
            try
            {
                PickedPoints = Picker.PickPoints(Picker.PickPointEnum.PICK_POLYGON);
            }
            catch { PickedPoints = null; }
            if (PickedPoints != null)
            {
                ContourPlate Plate = new ContourPlate();
                Plate.AssemblyNumber.Prefix = "P";
                Plate.AssemblyNumber.StartNumber = 1;
                Plate.PartNumber.Prefix = "p";
                Plate.PartNumber.StartNumber = 1;
                Plate.Name = "PLATE";
                Plate.Profile.ProfileString = "PL25";
                Plate.Material.MaterialString = "C245";
                Plate.Finish = "";
                Plate.Class = "1";
                Plate.Position.Depth = Position.DepthEnum.FRONT;
                foreach(T3D.Point ThisPoint in PickedPoints)
                {
                    var chamfer = new Chamfer(12.7, 12.7, Chamfer.ChamferTypeEnum.CHAMFER_LINE);
                    var conturPoint = new ContourPoint(ThisPoint, chamfer);
                    Plate.AddContourPoint(conturPoint);
                }
                if(!Plate.Insert())
                {
                    Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Plate wasn't created.");
                }
                else
                {
                    // Change the workplane to the coordinate system of the plate.
                    var transformationPlane = new TransformationPlane(Plate.GetCoordinateSystem());
                    Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlane);

                    // Show the plate in the model and the workplane change.
                    Model.CommitChanges();

                    // This gets the plate's coordinates and information in the current workplane.
                    Plate.Select();
                    ReperShow(Plate.GetCoordinateSystem());
                    // Draw the coordinate of the plate in the model in the local coordinate system.
                    GraphicsDrawer Drawer = new GraphicsDrawer();
                    foreach (ContourPoint ContourPoint in Plate.Contour.ContourPoints)
                    {
                        double x = ContourPoint.X, y = ContourPoint.Y, z = ContourPoint.Z;
                        T3D.Point CornerPoint = new T3D.Point(x, y, z);
                        PointXYZ(ContourPoint);
                        //double ImperialValue = 25.4;
                        //double XValue = Math.Round(CornerPoint.X / ImperialValue, 4);
                        //double YValue = Math.Round(CornerPoint.Y / ImperialValue, 4);
                        //double ZValue = Math.Round(CornerPoint.Z / ImperialValue, 4);
                        //Drawer.DrawText(CornerPoint, "(" + XValue + "," + YValue + "," + ZValue + ")", new Color(1,0,0));
                        Drawer.DrawLineSegment(new LineSegment(new T3D.Point(0, 0, 0), new T3D.Point(0, 0, 500)), new Color(1, 0, 0));
                    }
                    mw.Msg("На экране Tekla построена пластина по заданным точкам"
                        + " и показаны координаты этих точек и репер ПСК.  [OK]");
                    MessageBox.Show("Построена пластина.");
                    mw.Msg();
                }
            }
        }

        public void CK08_SetWorkPlane()
        {
            // Reset Workplane back to global
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            mw.Msg("После выполнения этого метода, система координат глобальная [OK]");
            MessageBox.Show("Вернулись к глобальной системе координат");
            mw.Msg();
        }
    }
}
