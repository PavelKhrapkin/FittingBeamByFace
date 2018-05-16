﻿/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    11.05.2018 Pavel Khrapkin
 * 
 *  Chris Keyack Session 06 https://www.youtube.com/watch?v=TuSVLPB5NyI
 *  Pick Points and set WorkPlane
 *  
 *  Cris Keyack Session 07 https://www.youtube.com/watch?v=kiDV1vwOOCg
 *  Write Selected Beam Data to Text File
 *  
 *  Tutorial Сеня Бусин https://www.youtube.com/watch?v=S-d0TBqMqVM
 *  TeklaOpenAPI Tutorial. Creating macro fitting a beam by face
 *  
 * 
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
using TSMUI = Tekla.Structures.Model.UI;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        #region --- Cris Keyack Session 06 ---
        public void Pick2Points()
        {
            T3D.Point FirstPoint = null;
            T3D.Point SecondPoint = null;
            Picker Picker = new Picker();
            try
            {
                ArrayList PickPoints = Picker
                    .PickPoints(TSMUI.Picker.PickPointEnum.PICK_TWO_POINTS);
                FirstPoint = PickPoints[0] as T3D.Point;
                SecondPoint = PickPoints[1] as T3D.Point;
            }
            catch { FirstPoint = SecondPoint = null; }

            if (FirstPoint != null && SecondPoint != null)
            {
                T3D.Vector XVector = new T3D.Vector(SecondPoint.X - FirstPoint.X,
                    SecondPoint.Y - FirstPoint.Y, SecondPoint.Z - FirstPoint.Z);
                T3D.Vector YVector = XVector.Cross(new T3D.Vector(0, 0, -1));
                Model.GetWorkPlaneHandler()
                    .SetCurrentTransformationPlane(new TransformationPlane(FirstPoint
                    , XVector, YVector));
                ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
                Model.CommitChanges();
            }
        }

        public void PickPart()
        {
            ModelObject PickedObject = null;
            Picker Picker = new Picker();
            try
            {
                PickedObject = Picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_OBJECT);
            }
            catch { PickedObject = null; }

            if (PickedObject != null)
            {
                T3D.CoordinateSystem ObjectSystem = PickedObject.GetCoordinateSystem();
                Model.GetWorkPlaneHandler()
                    .SetCurrentTransformationPlane(new TransformationPlane(ObjectSystem));

                double StartX = 500;
                for (int i = 0; i < 7; i++)
                {
                    T3D.Point p1 = new T3D.Point(StartX, 0);
                    T3D.Point p2 = new T3D.Point(StartX, 0, 1000);
                    CreateBeam("test Beam", "I30B1_20_93", p1, p2);
                    StartX += 500;
                }
                ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
                Model.CommitChanges();
            }
        }

        public void Global()
        {
            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane());
            ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            Model.CommitChanges();
        }
        #endregion --- Cris Keyack Session 06 ---
        #region --- Cris Keyack Session 07 ---
        public void Beam(string prfStr)
        {
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
                Beam ThisBeam = CreateBeam("MyBeam", prfStr, FirstPoint, SecondPoint);
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
                MessageBox.Show("Beam Inserted");

                ThisBeam.Profile.ProfileString = "—200*20";
                double Height = 0;
                ThisBeam.GetReportProperty("HEIGHT", ref Height);
                string ReportProfile = "—200*20";
                ThisBeam.GetReportProperty("PROFILE", ref ReportProfile);
                Model.CommitChanges();
                MessageBox.Show("Beam Modified - PL");

                ThisBeam.Delete();
                Model.CommitChanges();
                MessageBox.Show("Beam DELETED");
            }
            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane());
            ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            Model.CommitChanges();
        }

        public void Column()
        {
            //T3D.Point FirstPoint = null;
            //Picker Picker = new Picker();
            //try
            //{
            //    FirstPoint = Picker.PickPoint(Picker.PickPointEnum.PICK_ONE_POINT);
            //}
            //catch { PickedPoint = null; }
            //if (PickedPoint != null)
            //{

            //}
            //Model.GetWorkPlaneHandler()
            //    .SetCurrentTransformationPlane(new TransformationPlane());
            //ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            //Model.CommitChanges();
        }

        public void Polibeam()
        {
            ArrayList PickedPoint = null;
            Picker Picker = new Picker();
            try
            {
                PickedPoint = Picker.PickPoints(Picker.PickPointEnum.PICK_POLYGON);
            }
            catch { PickedPoint = null; }
            if(PickedPoint != null)
            {

            }
            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane());
            ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            Model.CommitChanges();
        }
        #endregion --- Cris Keyack Session 07 ---
        #region --- Tekla OpenAPI reference DrawTextExample 8.5.2018 ---
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
        #endregion ------ DrawTextExample Excercise 8.5.2018 ----
        #region --- Сеня Бусин Creating macro fitting a beam by face ---
        public GeometricPlane PickFace()
        {
            Picker picker = new Picker();
            PickInput input = picker.PickFace("Pick a FACE");
            IEnumerator enumerator = input.GetEnumerator();
            List<T3D.Point> points = new List<T3D.Point>();

            while (enumerator.MoveNext())
            {
                InputItem item = enumerator.Current as InputItem;
                if (item.GetInputType() == InputItem.InputTypeEnum.INPUT_POLYGON)
                {
                    ArrayList alist = item.GetData() as ArrayList;
         //           int counter = 1;
                    foreach (T3D.Point p in alist)
                    {
                        points.Add(p);
          //              Txt(p, counter.ToString());
          //              counter++;
                    }
                }
            }
            T3D.Point origin = points[1];
            T3D.Vector axisX = new T3D.Vector(points[0] - points[1]);
            T3D.Vector axisY = new T3D.Vector(points[2] - points[1]);
            GeometricPlane geomPlane = new GeometricPlane(origin, axisX, axisY);

            Model model = new Model();
            WorkPlaneHandler workPlane = model.GetWorkPlaneHandler();
            TransformationPlane currentPlane = workPlane.GetCurrentTransformationPlane();
            Matrix matrix = currentPlane.TransformationMatrixToLocal;
            T3D.Point p1 = matrix.Transform(geomPlane.Origin);
            T3D.Point p2 = matrix.Transform(geomPlane.Origin + geomPlane.Normal);
            geomPlane.Origin = p1;
            geomPlane.Normal = new T3D.Vector(p2 - p1);
            T3D.Point dummy = null;
            int counter = 1;
            foreach (T3D.Point pt in points)
            {
                dummy = matrix.Transform(pt);
                Txt(dummy, counter.ToString());
                counter++;
            }

            return geomPlane;
        }

        public Beam PickBeam()
        {
            Picker picker = new Picker();
            return picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick a Beam") as Beam;
        }

        public void FittingBeamByFace()
        {
            Beam beam = PickBeam();
            GeometricPlane geomPlane = PickFace();

            Fitting fitting = new Fitting();
            fitting.Father = beam;
            CoordinateSystem beamCS = beam.GetCoordinateSystem();
            ReperShow(beamCS);
            Line lineAlongBeamAxisX = new Line(beamCS.Origin, beamCS.AxisX);
            //do u need Z asis
            //T3D.Vector axisZ = beamCS.
            T3D.Point intersectionPoint = Intersection.LineToPlane(lineAlongBeamAxisX, geomPlane);
            T3D.Point randomPoint = new T3D.Point(intersectionPoint + new T3D.Point(100,100,100));
            randomPoint = Projection.PointToPlane(randomPoint, geomPlane);
            T3D.Vector x = new T3D.Vector(randomPoint - intersectionPoint);
            T3D.Vector y = geomPlane.Normal.Cross(x);
            Plane plane = new Plane();
            plane.Origin = intersectionPoint;
            plane.AxisX = x;
            plane.AxisY = y;
            x.Normalize(500);
            y.Normalize(500);
            fitting.Plane = plane;
            fitting.Insert();



        }

        #endregion --- Сеня Бусин Creating macro fitting a beam by face ---
    }
}
