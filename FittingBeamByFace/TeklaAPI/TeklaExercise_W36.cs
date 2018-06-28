/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    27.06.2018 Pavel Khrapkin
 * Подходы к созданию узла W36
 */
using FittingBeamByFace.SteelJoin;
using Tekla.Structures.Model;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        Joins SJ = new Joins();

        public void W36_Get2beam()
        {
            Beam MainBeam = PickBeam(283, 24);
            Beam AttBeam = PickBeam(283, 25);
            SJ.W36(MainBeam, AttBeam);
        }

        public void W36_CheckCrossBeam()
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
