/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    3.06.2018 Pavel Khrapkin
 * 
 *  Cris Keyack Session 07 https://www.youtube.com/watch?v=kiDV1vwOOCg
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
        public void CK07_Beam()
        {
            string prf = mw.prfStr;
            // Create Beam after Pick 2 Points - StartPoint and EndPoint of Beam
            T3D.Point FirstPoint = null, SecondPoint = null;
            Picker Picker = new Picker();
            try
            {
                ArrayList PickedPoints = Picker.PickPoints(Picker.PickPointEnum.PICK_TWO_POINTS);
                FirstPoint = PickedPoints[0] as T3D.Point;
                SecondPoint = PickedPoints[1] as T3D.Point;
            }
            catch { FirstPoint = SecondPoint  = null; }
            if (FirstPoint != null && SecondPoint != null)
            {
                Beam ThisBeam = CreateBeam("MyBeam", prf, FirstPoint, SecondPoint);
                ThisBeam.Finish = "GALVANIZED";
                ThisBeam.Class = "4";
                ThisBeam.AssemblyNumber.Prefix = "B";
                ThisBeam.AssemblyNumber.StartNumber = 1;
                ThisBeam.Position.Depth = Position.DepthEnum.BEHIND;
                ThisBeam.Position.Plane = Position.PlaneEnum.MIDDLE;
                ThisBeam.Position.Rotation = Position.RotationEnum.TOP;
                ThisBeam.Modify();
                ThisBeam.SetUserProperty("USER_FIELD_1", "PEOPLE");
                string UserField = "";
                ThisBeam.GetUserProperty("USER_FIELD_1", ref UserField);
                Solid BeamSolid = ThisBeam.GetSolid();
                T3D.CoordinateSystem BeamCoordinateSystem = ThisBeam.GetCoordinateSystem();
                Assembly BeamAssembly = ThisBeam.GetAssembly();
                ModelObjectEnumerator BeamsBolt = ThisBeam.GetBolts();
                ReperShow(BeamCoordinateSystem);
                Model.CommitChanges();
                mw.Msg("Балка и репер выводится в точке 1 с осью Х по направлению к фиолетовой точке"
                    + " (ручке конца балки) - точке 2. Профиль как в поле TextBox выше.      [ОК]");
                MessageBox.Show("Создал балку по 2 точкам и вывел репер");
                mw.Msg();

                string ReportProfile = "—200*20";
                ThisBeam.Class = "2";
                ThisBeam.Profile.ProfileString = ReportProfile;
                ThisBeam.Modify();
                double Height = 0;
                ThisBeam.GetReportProperty("HEIGHT", ref Height);
                
                ThisBeam.GetReportProperty("PROFILE", ref ReportProfile);
                Model.CommitChanges();
                mw.Msg("Профиль балки изменяется на красную пластину -200x20  [OK]");
                MessageBox.Show("Beam Modified - PL");

                ThisBeam.Delete();
                Model.CommitChanges();
                mw.Msg("После всех предыдущих манипуляций, балка стерта.  [ОК]");
                MessageBox.Show("Beam DELETED");
                mw.Msg();
            }
            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane());
            ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            Model.CommitChanges();
        }

        public void CK07_Column()
        {
            // Create Column after Pick a Point of this column
            T3D.Point FirstPoint = null;
            Picker Picker = new Picker();
            try
            {
                ArrayList PickedPoints = Picker.PickPoints(Picker.PickPointEnum.PICK_ONE_POINT);
                FirstPoint = PickedPoints[0] as T3D.Point;
            }
            catch { FirstPoint = null; }
            if (FirstPoint != null)
            {
                T3D.Point SecondPoint = new T3D.Point(FirstPoint.X, FirstPoint.Y, FirstPoint.Z + 8000);
                Beam ThisBeam = CreateBeam("MyColumn", mw.prfStr, FirstPoint, SecondPoint);
                ThisBeam.Finish = "D";
                ThisBeam.Class = "7";
                ThisBeam.AssemblyNumber.Prefix = "C";
                ThisBeam.AssemblyNumber.StartNumber = 1;
                ThisBeam.Position.Depth = Position.DepthEnum.MIDDLE;
                ThisBeam.Position.Plane = Position.PlaneEnum.MIDDLE;
                ThisBeam.Position.Rotation = Position.RotationEnum.TOP;
                ThisBeam.Modify();
                //ThisBeam.SetUserProperty("USER_FIELD_1", "PEOPLE");
                //string UserField = "";
                //ThisBeam.GetUserProperty("USER_FIELD_1", ref UserField);
                //Solid BeamSolid = ThisBeam.GetSolid();
                T3D.CoordinateSystem BeamCoordinateSystem = ThisBeam.GetCoordinateSystem();
                Assembly BeamAssembly = ThisBeam.GetAssembly();
                ModelObjectEnumerator BeamsBolt = ThisBeam.GetBolts();
                ReperShow(BeamCoordinateSystem);
                Model.CommitChanges();
      
                Matrix FromColumnSystem = MatrixFactory.FromCoordinateSystem(ThisBeam.GetCoordinateSystem());
                T3D.Point FrontFacePoint = FromColumnSystem.Transform(new T3D.Point(50, 50, 500));
                GraphicsDrawer Drawer = new GraphicsDrawer();
                // Label for Front Face
                var label = new Color(0, 0, 0);
                Drawer.DrawLineSegment(new LineSegment(FirstPoint, FrontFacePoint), label);
                Drawer.DrawText(FrontFacePoint, "THIS IS FRONT FACE", label);

                mw.Msg("В выбранной точке создается колонна с осью Х ПСК, направленой вверх."
                   + " Профиль колонны как в поле TextBox выше. Черная надпись показывает"
                   + "  переднюю грань.      [ОК]");
                MessageBox.Show("Создал колонну и вывел репер, указал переднюю грань");
                mw.Msg();
            }
        }

        public void CK07_Polybeam()
        {
            ArrayList PickedPoints = null;
            Picker Picker = new Picker();
            try
            {
                PickedPoints = Picker.PickPoints(Picker.PickPointEnum.PICK_POLYGON);
            }
            catch { PickedPoints = null; }
            if(PickedPoints != null)
            {
                PolyBeam ThisPolyBeam = new PolyBeam();
                ThisPolyBeam.Profile.ProfileString = mw.prfStr;
                ThisPolyBeam.Name = "CURVED PLATE";
                ThisPolyBeam.Finish = "GALVANIZED";
                ThisPolyBeam.Class = "6";
                ThisPolyBeam.AssemblyNumber.Prefix = "P";
                ThisPolyBeam.AssemblyNumber.StartNumber = 1;
                ThisPolyBeam.PartNumber.Prefix = "m";
                ThisPolyBeam.Material.MaterialString = "C245";
                ThisPolyBeam.Position.Depth = Position.DepthEnum.FRONT;
                ThisPolyBeam.Position.Plane = Position.PlaneEnum.MIDDLE;
                ThisPolyBeam.Position.Rotation = Position.RotationEnum.TOP;
                if(PickedPoints.Count == 3)
                {
                    ThisPolyBeam.AddContourPoint(new ContourPoint(PickedPoints[0] as T3D.Point, null));
                    ThisPolyBeam.AddContourPoint(new ContourPoint(PickedPoints[1] as T3D.Point, new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT)));
                    ThisPolyBeam.AddContourPoint(new ContourPoint(PickedPoints[2] as T3D.Point, null));
                    mw.Msg("Выбрано 3 точки; по ним строится дуговая балка.   [OK]");
                }
                else
                {
                    foreach(T3D.Point ThisPoint in PickedPoints)
                    {
                        ThisPolyBeam.AddContourPoint(new ContourPoint(ThisPoint, null));
                    }
                    mw.Msg($"Выбрано {PickedPoints.Count} точек. Фаски составной балки отсутствуют.  [OK]");
                }
                ThisPolyBeam.Insert();
            }
            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane());
            ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            Model.CommitChanges();

//            mw.Msg("  [OK]");
            MessageBox.Show("Построена составная балка.");
            mw.Msg();
        }
    }
}
