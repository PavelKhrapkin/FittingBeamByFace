/* ------------------------------------------------------------------
 * TeklaAPI - module is working with TeklaStructure over OpenAPI
 * 
 * 7.05.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 24.04.2018 - Common TeklaAPI module created
 *  7.05.2018 - Senia Busin Excercise
 *  8.05.2018 - DrawTextExample Excercise
 * 11.05.2018 - Separated few methods to TeklaLib
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
using Tekla.Structures.Geometry3d;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        protected Model Model;
        protected TransformationPlane ModelPlane, TmpPlane;
        Beam MainBeam = null, AttBeam = null;

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

        public Beam CreateBeam(string name, string prfStr, T3D.Point p1, T3D.Point p2)
        {
            if(p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z) goto ErrXeqY;
            if (prfStr == "" || prfStr == null) goto ErrPrfStr;
            Beam ThisBeam = new Beam();
            ThisBeam.StartPoint = p1;
            ThisBeam.EndPoint = p2;
            ThisBeam.Profile.ProfileString = prfStr;
            ThisBeam.Material.MaterialString = "C245";
            ThisBeam.Class = "7";
            ThisBeam.Insert();
            Model.CommitChanges();
            return ThisBeam;

            string msg;
            ErrPrfStr: msg = "Profile string is empty"; goto Err;
            ErrXeqY: msg = "StartPoint and EndPoint are the same -- Beam not Created.";
            Err: MessageBox.Show(msg);
            return null;
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

            double dist = Distance.PointToPoint(intersectionSegment.Point1, intersectionSegment.Point2);
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

        internal void ExReper()
        {
            Picker picker = new Picker();
            T3D.Point p = picker.PickPoint("Pick Point for Reper");
            Rep(p);

            T3D.Point p1 = picker.PickPoint("Pick Point for Rotated П/4 Reper");
            T3D.Vector v1 = new T3D.Vector(1, 1, 1);
            T3D.Vector v2 = new T3D.Vector(Rotate(v1, 3.1415926 / 4));
            Rep(p1);

        }
    }
}