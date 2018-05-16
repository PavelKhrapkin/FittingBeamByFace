/* ----------------------------------------------------------------------------
 * TeklaLib - part of TeklaAPI module - separated Library simple common methods
 * 
 * 15.05.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 11.05.2018 - TeklaLib module created
 * 15.05.2018 - ReperShow method add
 * --- Methods: ---
 * Txt(point, text [, color])   - draw string text in point with color name
 * ShowXYZ(point)   - draw point coordinates as "(x, y, z)" string with integer
 * ShowI(x)         - draw x value as int in Tekla Window
 * Rep(point)       - draw Reper as Global [x,y,z] in point
 * ReperShow(CoordSys) - draw [x,y,z] arrow in Origin Point of Coordinate Syst
 */
using System.Globalization;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;

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
    }
}