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

        public void W36(Beam MainBeam, Beam AttBeam)
        {
            // толщина стенки AttBeam - WEB_THICKNESS
            double BeamThickness = -1;
            AttBeam.GetReportProperty("WEB_THICKNESS", ref BeamThickness);
            if (BeamThickness < 5 || BeamThickness > 23)
                throw new Exception($"Wrong AttBeam thickness={BeamThickness}");

  //          if (TS.Model == null) throw new Exception("jj");

            // вут
            int l_vut = 160;
            int l_size = 40;
            double w_tav = 13.5;
            double xTav = 400 + (BeamThickness + w_tav) / 2;
            Point p5 = new Point(xTav, -l_vut / 2);
            Point p6 = new Point(xTav, l_vut / 2);
            Beam vut = TS.CreateBeam("вут", "T40BT1_14_2_685_86", p5, p6, Class: "6",
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
       //     BoltArray.CutLength = 30;
            // Add to specings of bolts in the X direction
            double Xoff = vutWeb ? 80 : vut.StartPoint.X;
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
            // Edge disctance from first point picked to first bolt in x direction
  //          BoltArray.StartPointOffset.Dx = 40;
            //        BoltArray.StartPointOffset.Dy = 38.1;
            //Front lines up nicely with x/y position in current workplane.
            BoltArray.Position.Rotation = Position.RotationEnum.FRONT;
            BoltArray.PartToBoltTo = vut;
            BoltArray.PartToBeBolted = beam;
            BoltArray.FirstPosition = new Point(dx, 0, 0);
            BoltArray.SecondPosition = new Point(1000, 0, 0);

            TS.PointShow(BoltArray.FirstPosition, "First");
            TS.PointShow(BoltArray.SecondPosition, "2nd");

            if (BoltArray.Insert())
            {
                // Draw X Axis of bolt group.
                TS.Line(BoltArray.FirstPosition, BoltArray.SecondPosition);
                // Set WorkPlane back to what user had before
                TS.Model.CommitChanges();
                //         return BoltArray;
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
