/* ----------------------------------------------------------------------------
 * TeklaLib - part of TeklaAPI module - separated Library simple common methods
 * 
 * 11.05.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- History: ---
 * 11.05.2018 - TeklaLib module created
 * --- Methods: ---
 * Txt(point, text [, color])   - draw string text in point with color name
 * ShowXYT(point)   - draw point coordinates as "(x, y, z)" string with integer
 * ShowI(x)         - draw x value as int in Tekla Window
 */
using System.Globalization;
using Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {

        public void Txt(T3D.Point point, string text, string color = "Red")
        {
            GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
            Color _color = new Color(1, 0, 0);
            if (color == "Black") _color = new Color(0, 0, 0);
            GraphicsDrawer.DrawText(point, text, _color);
        }

        //Shows the point coordinates without decimals
        private string ShowXYZ(T3D.Point p)
            => "(" + ShowI(p.X) + ", " + ShowI(p.Y) + ", " + ShowI(p.Z) + ")";
        private string ShowI(double x)
            => x.ToString("0", CultureInfo.InvariantCulture);
    }
}