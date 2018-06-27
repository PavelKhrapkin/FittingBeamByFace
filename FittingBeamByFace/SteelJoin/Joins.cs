/* ---------------------------------------------------------------------------
 * Joins - Steel Assembly Joins   26.06.2018 Pavel Khrapkin
 * Main module of Plugins for steel stractural connections in SteelJoin folder
 * 
 * --- History 26.06.2018 - module created
 * 
 *
 */
using System;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace FittingBeamByFace.SteelJoin
{
    public class Joins
    {
        _TS TS = new _TS();
        JoinLib JL = new JoinLib();

        public void W36(Beam MainBeam, Beam AttBeam)
        {
            double BeamThickness = JL.BeamWebThickness(AttBeam);
            int l_vut = 160;
            int l_size = 40;
            double w_vut = 13.5;
            double x_vut = AttBeam.StartPoint.X + (BeamThickness + w_vut) / 2;
            Point p5 = new Point(x_vut, -l_vut / 2);
            Point p6 = new Point(x_vut, l_vut / 2);
            Beam vut = TS.CreateBeam("W36_вут", "T40BT1_14_2_685_86", p5, p6, Class: "6",
                PositionRotation: (int)Position.RotationEnum.BELOW,
                PositionPlane: (int)Position.PlaneEnum.MIDDLE,
                PositionDepth: (int)Position.DepthEnum.FRONT);

            VutBolt(vut, false, MainBeam);
            VutBolt(vut, true, AttBeam);
        }

        public void VutBolt(Beam vut, bool vutWeb, Beam beam)
        {
            if (vut == null || beam == null) throw new Exception("Wrong beams");

            TS.SetWorkPlane(beam);

            double l = 80;
            double dx = vut.StartPoint.X - l / 2;
            if (vutWeb) dx = 80;

            // BoltGroupCode
            BoltArray BoltArray = new BoltArray();
            BoltArray.BoltSize = 20;
            BoltArray.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            BoltArray.BoltStandard = "7798";
            BoltArray.Length = 30;
            if (!vutWeb)
            {
                BoltArray.AddBoltDistX(80);
                BoltArray.AddBoltDistY(0); // I40-80; I45-90; I50,I55-100; I60-110
            }
            else
            {
                BoltArray.AddBoltDistX(0);
                BoltArray.AddBoltDistY(80); // I40-80; I45-90; I50,I55-100; I60-110
            }
   
            BoltArray.Position.Rotation = Position.RotationEnum.FRONT;
            BoltArray.PartToBoltTo = vut;
            BoltArray.PartToBeBolted = beam;
            BoltArray.FirstPosition = new Point(dx, 0, 0);
            BoltArray.SecondPosition = new Point(1000000, 0, 0);

            //TS.PointShow(BoltArray.FirstPosition, "First");
            //TS.PointShow(BoltArray.SecondPosition, "2nd");

            if (BoltArray.Insert())
            {
                // Draw X Axis of bolt group.
                //TS.Line(BoltArray.FirstPosition, BoltArray.SecondPosition);
                // Set WorkPlane back to what user had before
                TS.Model.CommitChanges();
            }
            else
            {
                Tekla.Structures.Model.Operations.Operation
                        .DisplayPrompt("Bolt not done");
            }
        }
    }

    class _TS : TeklaAPI.TeklaAPI
    {

    }
}
