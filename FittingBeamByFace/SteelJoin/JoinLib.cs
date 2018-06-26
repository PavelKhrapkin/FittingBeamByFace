/* ---------------------------------------------------------------------------
 * JoinLib - Steel Assembly Join Library    26.06.2018 Pavel Khrapkin
 * Library of common Joins Plugin methods
 * 
 * --- History 26.06.2018 - module created
 * 
 * --- Methods: ---
 * M20_7798_Length(connectingLength, AddLength) - return Bolt length with
 *                  accounted total thickness of connected details and optional
 *                  additional length for extra nuts, Wasters etc
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSLIB = TeklaAPI.TeklaAPI;

namespace FittingBeamByFace.SteelJoin
{
    public class JoinLib
    {
        TeklaAPI.TeklaAPI TS = new TeklaAPI.TeklaAPI();
        public double M20_7798_Length(int connectedLng, double AddBoltLength = 0)
        {
            int[] bolt_lng_GOST = { 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 90
                    , 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200
                    , 200, 220, 240, 260, 280, 300};
            // Default Additional Length for M20 Bolt = Bolt Head + Wasters + Nuts 
            const int M20addLng = 18;
            int i = 0;
            double atLeast = M20addLng + AddBoltLength + connectedLng;
            while (bolt_lng_GOST[i] < atLeast)
            {
                i++;
                if (i >= bolt_lng_GOST.Length)
                {
                    TS.TSerror(TS.LocalTxt(616));
                    return -1;
                }
            }
            return bolt_lng_GOST[i];
        }
    }
}
