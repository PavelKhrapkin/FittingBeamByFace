/* ------------------------------------------------------------------
 * TeklaAPI - module is working with TeklaStructure over OpenAPI
 * 
 * 7.05.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 24.04.2018 - Common TeklaAPI module created
 *  7.05.2018 - Senia Busin Excercise
 *  8.05.2018 - DrawTextExample Excercise
 * --- Methods: ---
 * Init()   - setup Model connection, save ModelPlane
 * CreateBeam(name, prfString, p1, p2)  - create ThisBeam from p1 to p2
 * SetWorkPlane([theBeam])  - create workplane at theBeam, 
 *                            by default,restore saved
 * Node36(MainBeam, AttBeam) - implement Node36
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;

namespace TeklaAPI
{
    public class TeklaAPI
    {
        protected Model Model;
        protected TransformationPlane ModelPlane, TmpPlane;
        Beam MainBeam = null, AttBeam = null;

        #region ------ DrawTextExample Excercise 8.5.2018 ----
#if DrawerTextExample
        private readonly Model _Model = new Model();
        private static GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();

        internal void MyDrawTextExample()
        {
            Picker picker = new Picker();
            T3D.Point tut = picker.PickPoint("Укажи точку!");

            Txt(tut, "Примерчик..");
            tut.X += 500;
            tut.Y += 500;
            Txt(tut, "черный??", "Black");

//            GraphicsDrawer.DrawText(tut, "ПРЕВЕД!!", new Color(1, 0, 0));
        }

        private readonly static Color TextColor = new Color(1, 0, 1);

        internal void DrawTextExample()
        {
            Picker picker = new Picker();
            do
            {
                MainBeam = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick MainBeam") as Beam;
                AttBeam = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick AttBeam") as Beam;
            } while (MainBeam == null || AttBeam == null);
            
            ShowExtremesInOtherObjectCoordinates(MainBeam, AttBeam);
        }
        //Shows the beam's extremes in the coordinates of the reference model object
        private void ShowExtremesInOtherObjectCoordinates(ModelObject ReferenceObject, Beam Beam)
        {
            //Set the transformation plane to use the beam's coordinate system in order to get the beam's extremes in the local coordinate system
            TransformationPlane CurrentTP = _Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            _Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam.GetCoordinateSystem()));

            //Update the beam's extremes to the new transformation plane
            Beam.Select();
            T3D.Point LocalStartPoint = Beam.StartPoint;
            T3D.Point LocalEndPoint = Beam.EndPoint;

            //Get the beam's extremes in the reference object's coordinates
            Matrix TransformationMatrix = MatrixFactory.ByCoordinateSystems(Beam.GetCoordinateSystem(), ReferenceObject.GetCoordinateSystem());

            //Transform the extreme points to the new coordinate system
            T3D.Point BeamStartPoint = TransformationMatrix.Transform(LocalStartPoint);
            T3D.Point BeamEndPoint = TransformationMatrix.Transform(LocalEndPoint);

            _Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);

            //Transform the points where to show the texts to current work plane coordinate system
            Matrix TransformationToCurrent = MatrixFactory.FromCoordinateSystem(ReferenceObject.GetCoordinateSystem());
            T3D.Point BeamStartPointInCurrent = TransformationToCurrent.Transform(BeamStartPoint);
            T3D.Point BeamEndPointInCurrent = TransformationToCurrent.Transform(BeamEndPoint);

            //Display results
            DrawCoordinateSytem(ReferenceObject.GetCoordinateSystem());
            GraphicsDrawer.DrawText(BeamStartPointInCurrent, FormatPointCoordinates(BeamStartPoint), TextColor);
            GraphicsDrawer.DrawText(BeamEndPointInCurrent, FormatPointCoordinates(BeamEndPoint), TextColor);
        }

        //Draws the coordinate system in which the values are shown
        private static void DrawCoordinateSytem(CoordinateSystem CoordinateSystem)
        {
            DrawVector(CoordinateSystem.Origin, CoordinateSystem.AxisX, "X");
            DrawVector(CoordinateSystem.Origin, CoordinateSystem.AxisY, "Y");
        }

        //Draws the vector of the coordinate system
        private static void DrawVector(T3D.Point StartPoint, T3D.Vector Vector, string Text)
        {
            Color Color = new Color(0, 1, 1);
            const double Radians = 0.43;

            Vector = Vector.GetNormal();
            T3D.Vector Arrow01 = new T3D.Vector(Vector);

            Vector.Normalize(500);
            T3D.Point EndPoint = new T3D.Point(StartPoint);
            EndPoint.Translate(Vector.X, Vector.Y, Vector.Z);
            GraphicsDrawer.DrawLineSegment(StartPoint, EndPoint, Color);

            GraphicsDrawer.DrawText(EndPoint, Text, Color);

            Arrow01.Normalize(-100);
            T3D.Vector Arrow = ArrowVector(Arrow01, Radians);

            T3D.Point ArrowExtreme = new T3D.Point(EndPoint);
            ArrowExtreme.Translate(Arrow.X, Arrow.Y, Arrow.Z);
            GraphicsDrawer.DrawLineSegment(EndPoint, ArrowExtreme, Color);

            Arrow = ArrowVector(Arrow01, -Radians);

            ArrowExtreme = new T3D.Point(EndPoint);
            ArrowExtreme.Translate(Arrow.X, Arrow.Y, Arrow.Z);
            GraphicsDrawer.DrawLineSegment(EndPoint, ArrowExtreme, Color);
        }

        //Draws the arrows of the vectors
        private static T3D.Vector ArrowVector(T3D.Vector Vector, double Radians)
        {
            double X, Y, Z;

            if (Vector.X == 0 && Vector.Y == 0)
            {
                X = Vector.X;
                Y = (Vector.Y * Math.Cos(Radians)) - (Vector.Z * Math.Sin(Radians));
                Z = (Vector.Y * Math.Sin(Radians)) + (Vector.Z * Math.Cos(Radians));
            }
            else
            {
                X = (Vector.X * Math.Cos(Radians)) - (Vector.Y * Math.Sin(Radians));
                Y = (Vector.X * Math.Sin(Radians)) + (Vector.Y * Math.Cos(Radians));
                Z = Vector.Z;
            }

            return new T3D.Vector(X, Y, Z);
        }

        //Shows the point coordinates with only two decimals
        private static string FormatPointCoordinates(T3D.Point Point)
        {
            string Output = String.Empty;

            Output = "(" + Point.X.ToString("0.00", CultureInfo.InvariantCulture) + ", " +
                     Point.Y.ToString("0.00", CultureInfo.InvariantCulture) + ", " +
                     Point.Z.ToString("0.00", CultureInfo.InvariantCulture) + ")";

            return Output;
        }
#endif
#endregion ------ DrawTextExample Excercise 8.5.2018 ----

        public void Init()
        {
            Model = new Model();
            if (!Model.GetConnectionStatus())
                throw new Exception("Tekla not connected");

            // Save Tekla Model Workplane to restore after change to any Part Plane
            SetWorkPlane();
            ModelPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            Model.CommitChanges();
        }

        public Beam PickBeam()
        {
            return null;
        }

        public GeometricPlane PickFace()
        {
            Picker picker = new Picker();
            PickInput input = picker.PickFace("Pick a FACE");
            IEnumerator enumerator = input.GetEnumerator();
            List<T3D.Point> points = new List<T3D.Point>();

            while (enumerator.MoveNext())
            {
                InputItem item = enumerator.Current as InputItem;
                if(item.GetInputType() == InputItem.InputTypeEnum.INPUT_POLYGON)
                {
                    ArrayList alis = item.GetData() as ArrayList;
                    int counter = 1;
                    foreach(T3D.Point p in alis)
                    {
                        points.Add(p);
                        counter++;
                    }
                }
            }
            return null;
        }

        public void FittingBeamByFace()
        {
            GeometricPlane geomPlane = PickFace();
        }

        public Beam CreateBeam(string name, string prfStr, T3D.Point p1, T3D.Point p2)
        {
            if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
            {
                MessageBox.Show("StartPoint and EndPoint are the same -- Beam not Created.");
                return null;
            }
            Beam ThisBeam = new Beam();
            ThisBeam.StartPoint = p1;
            ThisBeam.EndPoint = p2;
            ThisBeam.Profile.ProfileString = prfStr;
            ThisBeam.Material.MaterialString = "C245";
            ThisBeam.Class = "7";
            ThisBeam.Insert();
            Model.CommitChanges();
            return ThisBeam;
        }

        protected void SetWorkPlane(Beam theBeam = null)
        {
            if (theBeam != null)
            {
                T3D.CoordinateSystem BeamCoordinateSystem = theBeam.GetCoordinateSystem();
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(BeamCoordinateSystem));
                Model.CommitChanges();
                ViewHandler.SetRepresentation("standard");
                return;
            }

            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            Model.CommitChanges();
            ViewHandler.SetRepresentation("standard");
        }

        public void Node36(Beam MainBeam, Beam AttBeam)
        {

        }

        public void Txt(T3D.Point point, string text, string color = "Red")
        {
//            private static
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            if (color == "Black") _color = new Color(0, 0, 0);
            GraphicsDrawer.DrawText(point, text, _color);
        }
    }
}