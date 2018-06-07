/* ----------------------------------------------------------------------------
 * TeklaLib - part of TeklaAPI module - separated Library simple common methods
 * 
 * 6.06.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 11.05.2018 - TeklaLib module created
 * 15.05.2018 - ReperShow method add
 * 16.05.2018 - PointShow add 
 *  1.06.2018 - PointXYZ description
 *  5.06.2018 - PickBeam add
 *  6.06.2018 - IAil(int) add
 * --- Methods: ---
 * Txt(point, text [, color])   - draw string text in point with color name
 * PoinXYZ(point)   - draw point coordinates as "(x, y, z)" string of integers
 * ShowI(x)         - draw x value as int in Tekla Window
 * Rep(point)       - draw Reper as Global [x,y,z] in point
 * ReperShow(CoordSys) - draw [x,y,z] arrow in Origin Point of Coordinate Syst
 * PointShow(p, text)  - draw point p and text - name of the point
 * PickBeam(text)   - Pick a beam from the Tekla model with text prompt
 * IAil(int n)      - display localyzed messabe number n in Tekla
 */
using System.Globalization;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {

        public void Txt(Point point, string text, string color = "Red")
        {
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            if (color == "Black") _color = new Color(0, 0, 0);
            GraphicsDrawer.DrawText(point, text, _color);
        }

        //Shows the point coordinates without decimals
        public void PointXYZ(Point p) => Txt(p, ShowXYZ(p));
        private string ShowXYZ(Point p)
            => "(" + ShowI(p.X) + ", " + ShowI(p.Y) + ", " + ShowI(p.Z) + ")";
        private string ShowI(double x)
            => x.ToString("0", CultureInfo.InvariantCulture);

        //Draws coordinate Reper with length 1000 in p in Global Coord System
        public void Rep(Point p)
        {
            Point pX = new Point(p.X + 1000, p.Y, p.Z);
            Point pY = new Point(p.X, p.Y + 1000, p.Z);
            Point pZ = new Point(p.X, p.Y, p.Z + 1000);
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            GraphicsDrawer.DrawLineSegment(p, pX, _color); Txt(pX, "X");
            GraphicsDrawer.DrawLineSegment(p, pY, _color); Txt(pY, "Y");
            GraphicsDrawer.DrawLineSegment(p, pZ, _color); Txt(pZ, "Z");
        }

        public void ReperShow(CoordinateSystem beamCoordinateSystem)
        {
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            Point p = beamCoordinateSystem.Origin;
            Vector aX = beamCoordinateSystem.AxisX.GetNormal();
            Vector aY = beamCoordinateSystem.AxisY.GetNormal();
            Vector aZ = Vector.Cross(aX, aY);
            Point pX = PointAddVector(p, aX, 1000);
            Point pY = PointAddVector(p, aY, 1000);
            Point pZ = PointAddVector(p, aZ, 1000);
            GraphicsDrawer.DrawLineSegment(p, pX, _color); Txt(pX, "X");
            GraphicsDrawer.DrawLineSegment(p, pY, _color); Txt(pY, "Y");
            GraphicsDrawer.DrawLineSegment(p, pZ, _color); Txt(pZ, "Z");
        }

        public Point PointAddVector(Point p, Vector v, double Norm = 1)
            => new Point(p.X + v.X * Norm, p.Y + v.Y * Norm, p.Z + v.Z * Norm);

        public void PointShow(Point p, string text = "")
        {
            const int l = 50;
            Point p1 = new Point(p.X + l, p.Y + l);
            Point p2 = new Point(p.X + l, p.Y - l);
            Point p3 = new Point(p.X - l, p.Y - l);
            Point p4 = new Point(p.X - l, p.Y + l);
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            GraphicsDrawer.DrawLineSegment(p1, p2, _color);
            GraphicsDrawer.DrawLineSegment(p2, p3, _color);
            GraphicsDrawer.DrawLineSegment(p3, p4, _color);
            GraphicsDrawer.DrawLineSegment(p4, p1, _color);
            GraphicsDrawer.DrawLineSegment(p1, p3, _color);
            GraphicsDrawer.DrawLineSegment(p2, p4, _color);
            Txt(p, text);
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

        public string LocalTxt(params string[] txt)
        {
            string str = string.Empty;
            foreach (string s in txt) str += LocalTxt(s);
            return str;
        }

        public string LocalTxt(params int[] n)
        {
            string str = string.Empty;
            foreach(int i in n)
            {
                str += local.GetText("by_number_msg_no_" + i);
            }
            return str;
        }
    }
}