/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    5.06.2018 Pavel Khrapkin
 * Подходы к созданию узла W36
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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
        public void DevelopW36()
        {

        }

        public void CheckCrossBeam()
        {
            Beam MainBeam = PickBeam(283, 24);
            BeamShow(MainBeam);
            Beam AddBeam = PickBeam(283, 25);
            BeamShow(AddBeam);
        }

        public void BeamShow(Beam beam)
        {
            if (beam == null) return;
            var psk = beam.GetCoordinateSystem();
            ReperShow(beam.GetCoordinateSystem());
            PointXYZ(beam.StartPoint);
            PointXYZ(beam.EndPoint);
        }
    }
}
