/* ------------------------------------------------------------------
 * TeklaAPI - module is working with TeklaStructure over OpenAPI
 * 
 * 26.06.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 24.04.2018 - Common TeklaAPI module created
 *  7.05.2018 - Senia Busin Excercise
 *  8.05.2018 - DrawTextExample Excercise
 * 11.05.2018 - Separated few methods to TeklaLib
 *  7.06.2018 - Localization
 *  8.06.2018 - CeateBeam interface expanded
 * 26.06,2018 - separated code for Joins and JoinLib
 * --- Methods: ---
 * Init()   - setup Model connection, save ModelPlane
 * SetWorkPlane([theBeam])  - create workplane at theBeam, 
 *                            by default,restore saved
 * Node36(MainBeam, AttBeam) - implement Node36
 */
using System;
using System.Collections;
using System.IO;
using System.Windows;
using Tekla.Structures.Datatype;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;
using TSDL = Tekla.Structures.Dialog.Localization;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        private FittingBeamByFace.MainWindow mw = null;
        protected TSDL local;        
        protected TransformationPlane ModelPlane, TmpPlane;       
        Beam MainBeam = null, AttBeam = null;
        private Model model;
        public Model Model { get => model; set => model = value; }

        public void Init(dynamic mainWindow = null)
        {
            mw = mainWindow;
            Model = new Model();
            if (!Model.GetConnectionStatus())
                throw new Exception("Tekla not connected");

            // Load LocalizationFile
            string file = @"C:\Program Files\Tekla Structures\2018\messages\by_number.ail";
            if (!File.Exists(file))  throw new Exception("No AIL file");   
            Tekla.Structures.Dialog.Dialogs.SetSettings(string.Empty);
            string lang = (string)Settings.GetValue("language");
            local = new TSDL(file, lang);

            // Save Tekla Model Workplane to restore after change to any Part Plane
            SetWorkPlane();
            ModelPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            Model.CommitChanges();
        }

        public void SetWorkPlane(Beam theBeam = null)
        {
            if (theBeam != null)
            {
                CoordinateSystem BeamCoordinateSystem = theBeam.GetCoordinateSystem();
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(BeamCoordinateSystem));
                Model.CommitChanges();
                ViewHandler.SetRepresentation("standard");
                return;
            }

            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            Model.CommitChanges();
            ViewHandler.SetRepresentation("standard");
        }

        public void TSerror(string msg) => MessageBox.Show(msg);

        public void RepShow(T3D.Point p, T3D.Vector vX, T3D.Vector vY)
        {
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);

            T3D.Vector nX = vX.GetNormal();
            T3D.Vector nY = vY.GetNormal();

            T3D.Point pX = new T3D.Point(p.X + 1000, p.Y, p.Z);
            T3D.Point pY = new T3D.Point(p.X, p.Y + 1000, p.Z);
            T3D.Point pZ = new T3D.Point(p.X, p.Y, p.Z + 1000);

            GraphicsDrawer.DrawLineSegment(p, pX, _color); Txt(pX, "X");
            GraphicsDrawer.DrawLineSegment(p, pY, _color); Txt(pY, "Y");
            GraphicsDrawer.DrawLineSegment(p, pZ, _color); Txt(pZ, "Z");
        }

        public T3D.Vector Rotate(T3D.Vector Vector, double Radians)
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

        internal void ExtendControlLine()
        {
            Picker picker = new Picker();
            ArrayList alist = picker.PickLine("Pick Line");
            T3D.Point p1 = alist[0] as T3D.Point;
            T3D.Point p2 = alist[1] as T3D.Point;

            ModelObject mo = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick Control Line");
            ControlLine cLine = mo as ControlLine;

            Line line1 = new Line(p1, p2);
            Line line2 = new Line(cLine.Line);

            Intersection.LineToLine(line1, line2);
            LineSegment intersectionSegment = Intersection.LineToLine(line1, line2);

            double dist = T3D.Distance.PointToPoint(intersectionSegment.Point1, intersectionSegment.Point2);
            if (dist > 0.0001) return;
        }

        // Exercise 10.05.2018 - pick line and coordinate display
        internal void CoordinateOfLine()
        {
            Picker picker = new Picker();
            ArrayList alist = picker.PickLine("Pick Line");
            T3D.Point p1 = alist[0] as T3D.Point;
            T3D.Point p2 = alist[1] as T3D.Point;
            Txt(p1, ShowXYZ(p1));
            Txt(p2, ShowXYZ(p2));
        }

        internal void SB_ExReper()
        {
            Picker picker = new Picker();
            T3D.Point p = picker.PickPoint("Pick Point for Reper");
            Rep(p);
            mw.Msg("В точке, выбранной по левой кнопке мышки, появляются"
               + " красные линии репера глобальной системы координат. [OK]");
            MessageBox.Show("Выведен репер");
            mw.Msg();

            //T3D.Point p1 = picker.PickPoint("Pick Point for Rotated П/4 Reper");
            //T3D.Vector v1 = new T3D.Vector(1, 1, 1);
            //T3D.Vector v2 = new T3D.Vector(Rotate(v1, 3.1415926 / 4));
            //Rep(p1);
        }
    }
}