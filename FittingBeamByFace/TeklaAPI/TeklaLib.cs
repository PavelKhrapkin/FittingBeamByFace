﻿/* ----------------------------------------------------------------------------
 * TeklaLib - part of TeklaAPI module - separated Library simple common methods
 * 
 * 22.06.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 11.05.2018 - TeklaLib module created
 * 15.05.2018 - ReperShow method add
 * 16.05.2018 - PointShow add 
 *  1.06.2018 - PointXYZ description
 *  5.06.2018 - PickBeam add
 *  6.06.2018 - IAil(int) add
 * 18.06.2018 - PlainShow add, VectorAd add 
 * 22.06.2018 - Line and PolyLine add, optimization with Line, color
 * --- Methods: ---
 * Line(p1, p2, [color])    - draw line from Point p1 to Point p2 
 * PolyLine([color], Points[] p) - draw polyline with color
 * Txt(point, text [, color])   - draw string text in point with color name
 * PoinXYZ(point)   - draw point coordinates as "(x, y, z)" string of integers
 * ShowI(x)         - draw x value as int in Tekla Window
 * Rep(point)       - draw Reper as Global [x,y,z] in point
 * ReperShow(CoordSys) - draw [x,y,z] arrow in Origin Point of Coordinate Syst
 * PointAdVector(p, v, [lng])   - get Point [p + v] with length lng
 * VectorAd(v1, v2) - add Vectors v1 and v2
 * PointShow(p, text)  - draw point p and text - name of the point
 * PlainShow(Plain)    - draw rectangular shows the Plane
 * PickBeam(text)   - Pick a beam from the Tekla model with text prompt
 * LocalTxt(dynamic[] x) - make localyzed messages by number or text string
 */
using System.Globalization;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        const string RED = "RED";
        private Color _color = new Color(1, 0, 0);

        public void Line(Point p1, Point p2, string color = "red")
            => GraphicsDrawer.DrawLineSegment(p1, p2, setColor(color));

        public void PolyLine(string color = "red", params Point[] p)
        {
            setColor(color);
            PolyLine(p);
        }
        public void PolyLine(params Point[]p)
        {
            if (p.Length < 2) return;
            var pnt = p[0];
            for( int i = 1; i < p.Length; i++)
            {
                Line(pnt, p[i]);
                pnt = p[i];
            }
        }

        public void Txt(Point point, string text, string color = RED) 
            => GraphicsDrawer.DrawText(point, text, setColor(color));

        private Color setColor(string color = RED)
        {
            string col = color.ToUpper();
            _color = new Color(1, 0, 0);
            if (col == "BLACK") _color = new Color(0, 0, 0);
            return _color;
        }

        //Shows the point coordinates without decimals
        public void PointXYZ(Point p) => Txt(p, ShowXYZ(p));
        private string ShowXYZ(Point p)
            => "(" + ShowI(p.X) + ", " + ShowI(p.Y) + ", " + ShowI(p.Z) + ")";
        private string ShowI(double x)
            => x.ToString("0", CultureInfo.InvariantCulture);

        //Draws coordinate Reper with length 1000 in p in Global Coord System
        public void Rep(Point p, string color=RED)
        {
            setColor(color);
            Point pX = new Point(p.X + 1000, p.Y, p.Z);
            Point pY = new Point(p.X, p.Y + 1000, p.Z);
            Point pZ = new Point(p.X, p.Y, p.Z + 1000);
            Line(p, pX); Txt(pX, "X");
            Line(p, pY); Txt(pY, "Y");
            Line(p, pZ); Txt(pZ, "Z");
        }

        public void ReperShow(CoordinateSystem beamCoordinateSystem, int lng = 1000)
        {
            var trPlane = new TransformationPlane();
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(trPlane);
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            Point p = beamCoordinateSystem.Origin;
            Vector aX = beamCoordinateSystem.AxisX.GetNormal();
            Vector aY = beamCoordinateSystem.AxisY.GetNormal();
            Vector aZ = Vector.Cross(aX, aY);
            Point pX = PointAddVector(p, aX, lng);
            Point pY = PointAddVector(p, aY, lng);
            Point pZ = PointAddVector(p, aZ, lng);
            GraphicsDrawer.DrawLineSegment(p, pX, _color); Txt(pX, "X");
            GraphicsDrawer.DrawLineSegment(p, pY, _color); Txt(pY, "Y");
            GraphicsDrawer.DrawLineSegment(p, pZ, _color); Txt(pZ, "Z");
        }

        public Point PointAddVector(Point p, Vector v, double Norm = 1)
            => new Point(p.X + v.X * Norm, p.Y + v.Y * Norm, p.Z + v.Z * Norm);
        public Vector VectorAd(Vector v1, Vector v2)
            => new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

        public void PointShow(Point p, string text = "")
        {
            const int l = 50;
            Point p1 = new Point(p.X + l, p.Y + l);
            Point p2 = new Point(p.X + l, p.Y - l);
            Point p3 = new Point(p.X - l, p.Y - l);
            Point p4 = new Point(p.X - l, p.Y + l);
            PolyLine(p1, p2, p3, p4, p1, p3, p4, p2);
            Txt(p, text);
        }

        public void PlaneShow(CoordinateSystem cs, int lng=1000)
        {

            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane(cs));

            Model.CommitChanges();
            ViewHandler.SetRepresentation("standard");
            var p = cs.Origin;
            Point p1 = new Point(p.X + lng, p.Y - lng);
            Point p2 = new Point(p.X + lng, p.Y + lng);
            Point p3 = new Point(p.X - lng, p.Y + lng);
            Point p4 = new Point(p.X - lng, p.Y - lng);

            //var trPlane = new TransformationPlane();
            //Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(trPlane);
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
   
            GraphicsDrawer.DrawLineSegment(p1, p2, _color); Txt(p1, "p1");
            GraphicsDrawer.DrawLineSegment(p2, p3, _color); Txt(p2, "p2");
            GraphicsDrawer.DrawLineSegment(p3, p4, _color); Txt(p3, "p3");
            GraphicsDrawer.DrawLineSegment(p4, p1, _color); Txt(p4, "p4");

            Model.GetWorkPlaneHandler()
               .SetCurrentTransformationPlane(new TransformationPlane());

            ReperShow(cs);
        }

        public Beam PickBeam(params int[] n) => PickBeam(LocalTxt(n));
        public Beam PickBeam(string prompt = "Pick a beam")
        {
            Picker picker = new Picker();
            Beam selectedBeam = null;
            while (selectedBeam == null)
            {
                var part = Picker.PickObjectEnum.PICK_ONE_PART;
                selectedBeam = picker.PickObject(part, LocalTxt(prompt)) as Beam;
            }
            return selectedBeam;
        }

        public string LocalTxt(params dynamic[] x)
        {
            string str = string.Empty;
            foreach (var s in x)
            {
                if(s.GetType() == typeof(string)) str += local.GetText(s);
                if(s.GetType() == typeof(int))
                    str += local.GetText("by_number_msg_no_" + s.ToString());
            }
            return str;
        }

        public Beam CutBeamByPart(Beam beam, Part part, bool startSideCut=true)
        {
            // вырезаем ThasBeam по MainBeam
            var partClass = part.Class;
            BooleanPart Beam = new BooleanPart();
            Beam.Father = beam;
            part.Class = BooleanPart.BooleanOperativeClassName;
            Beam.SetOperativePart(part);
            Beam.Insert();
            part.Class = partClass;
            part.Modify();

            // создаем режущую плоскость, чтобы отбросить 
            //..(левую) часть ThisBeam по плоскости MainBeam
            CutPlane BeamLineCut = new CutPlane();
            BeamLineCut.Father = beam;
            Plane BeamCutPlane = new Plane();
            var cs = part.GetCoordinateSystem();
            if (startSideCut) cs.AxisX *= -1;
            BeamCutPlane.AxisX = cs.AxisX;
            BeamCutPlane.AxisY = cs.AxisY;
            BeamCutPlane.Origin = cs.Origin;
            BeamLineCut.Plane = BeamCutPlane;
            BeamLineCut.Insert();

            Model.CommitChanges();

            return beam;
        }
    }
}