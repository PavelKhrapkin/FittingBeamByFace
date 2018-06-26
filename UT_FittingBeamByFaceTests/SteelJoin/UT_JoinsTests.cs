/* --------------------------------------------------------------------
 * UT_JoinsTests - Unit Tests узлов Joins    25.06.2018 Pavel Khrapkin
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FittingBeamByFace.SteelJoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TeklaAPI;

namespace FittingBeamByFace.SteelJoin.Tests
{
    [TestClass()]
    public class UT_JoinsTests
    {
        U TS = new U();

        [TestMethod()]
        public void UT_W36()
        {
            // test 0: создаем MainBeam и AttBeam, потом соединяем их W36
            Point p1 = new Point(0, 2000, 0);
            Point p2 = new Point(3000, 0, 0);
            Beam MainBeam = TS.CreateBeam("Main Beam", "I30B1_20_93", p1, p2);
            _TS._SetWorkPlane(MainBeam);

            Point p3 = new Point(400, 0, 0);
            Point p4 = new Point(400, 0, 2000);
            Beam AttBeam = _TS.CreateBeam("Att Beam", "I20B1_20_93", p3, p4
                , PositionRotation: (int)Position.RotationEnum.BACK
                , PositionPlane: (int)Position.PlaneEnum.MIDDLE
                , PositionDepth: (int)Position.DepthEnum.MIDDLE
                , Class: "5");

            // толщина стенки AttBeam - WEB_THICKNESS
            double BeamThickness = -1;
            AttBeam.GetReportProperty("WEB_THICKNESS", ref BeamThickness);
            if (BeamThickness < 5 || BeamThickness > 23)
                throw new Exception($"Wrong AttBeam thickness={BeamThickness}");

            // do it later            _TS.Node36(MainBeam, AttBeam);

            // вут
            int l_vut = 160;
            int l_size = 40;
            double w_tav = 13.5;
            double xTav = 400 + (BeamThickness + w_tav) / 2;
            Point p5 = new Point(xTav, -l_vut / 2);
            Point p6 = new Point(xTav, l_vut / 2);
            Beam vut = _TS.CreateBeam("вут", "T40BT1_14_2_685_86", p5, p6, Class: "6",
                PositionRotation: (int)Position.RotationEnum.BELOW,
                PositionPlane: (int)Position.PlaneEnum.MIDDLE,
                PositionDepth: (int)Position.DepthEnum.FRONT);

            VutBolt(vut, true, MainBeam);
            VutBolt(vut, false, AttBeam);
        }

        public void VutBolt(Beam vut, bool vutWeb, Beam beam)
        {
            if (vut == null || beam == null) throw new Exception("Wrong beams");

            ///          CoordinateSystem BeamCoordinateSystem = vut.GetCoordinateSystem();
            CoordinateSystem BeamCoordinateSystem = beam.GetCoordinateSystem();
            var tp = new TransformationPlane(BeamCoordinateSystem);
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(tp);
            Model.CommitChanges();
            ViewHandler.SetRepresentation("standard");

            // BoltGroupCode
            BoltArray BoltArray = new BoltArray();
            BoltArray.BoltSize = 20;
            BoltArray.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            BoltArray.BoltStandard = "7798";
            BoltArray.CutLength = 30;
            // Add to specings of bolts in the X direction
            double Xoff = vutWeb ? 80 : vut.StartPoint.X;
            BoltArray.AddBoltDistX(80);
            BoltArray.AddBoltDistY(0); // I40-80; I45-90; I50,I55-100; I60-110
            // Edge disctance from first point picked to first bolt in x direction
            BoltArray.StartPointOffset.Dx = 40;
            //        BoltArray.StartPointOffset.Dy = 38.1;
            //Front lines up nicely with x/y position in current workplane.
            BoltArray.Position.Rotation = Position.RotationEnum.FRONT;
            BoltArray.PartToBoltTo = vut;
            BoltArray.PartToBeBolted = beam;
            BoltArray.FirstPosition = new Point(0, 0, 0);
            BoltArray.SecondPosition = new Point(1000, 0, 0);

            _TS.PointShow(BoltArray.FirstPosition, "First");
            _TS.PointShow(BoltArray.SecondPosition, "2nd");

            if (BoltArray.Insert())
            {
                // Draw X Axis of bolt group.
                _TS.Line(BoltArray.FirstPosition, BoltArray.SecondPosition);
                // Set WorkPlane back to what user had before
                Model.CommitChanges();
                //         return BoltArray;
            }
            else
            {
                Tekla.Structures.Model.Operations.Operation
                        .DisplayPrompt("Bolt not done");
            }
        }
    }
}