/* -----------------------------------------------------
* Tekla module Unit Tests  1.06.2018 Pavel Khrapkin
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSDL = Tekla.Structures.Dialog.Localization;

namespace TeklaAPI.Tests
{
    [TestClass()]
    public class UT_Tekla
    {
        #region --- variables, Initialyze, Cleanup
        U _TS = new U();
        Model Model;
        TransformationPlane ModelPlane, TmpPlane;
        Beam ThisBeam, MainBeam, AttBeam;

        [TestInitialize]
        public void Initialyze()
        {
            _TS.Init();
            Model = _TS.GetModel();
            if (!Model.GetConnectionStatus())
                throw new Exception("Tekla Model not connected!");
            ModelPlane = _TS.GetModelPlane();
            TmpPlane = _TS.GetTmpPlane();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // restore coordinate system to ModelPlain and delete Beam
            var returnedPlane = _TS._SetWorkPlane();
            //            Assert.AreEqual(returnedPlane, ModelPlane);
            if (ThisBeam != null) ThisBeam.Delete();
            if (MainBeam != null) MainBeam.Delete();
            if (AttBeam != null) AttBeam.Delete();
            Model.CommitChanges();
        }
        #endregion --- variables, Initialyze, Cleanup
        [TestMethod()]
        public void UT_Tekla_Init()
        {
            _TS.Init();
            Assert.IsNotNull(Model);
            Assert.IsNotNull(ModelPlane);
        }

        #region --- Txt, LocalTxt, PointShow, ReperShow
        [TestMethod()]
        public void UT_Txt()
        {
            // Test выводит надписи красным и черным, а потом эти надписи
            //.. стираются во время cleanup.Чтобы выводить тексты на в точке
            //..(7500, 1500, 0), а там, где указано, надо сменить ниже
            //..закомментированные определения Point tut
 //           TSMUI.Picker picker = new TSMUI.Picker();
            T3D.Point tut = new T3D.Point(7500.0, 1500.0);

            _TS.Txt(tut, "Примерчик..");
            tut.X += 500;
            tut.Y += 500;
            _TS.Txt(tut, "черный??", "Black");
        }

        [TestMethod()]
        public void UT_LocalTxt()
        {
            // test 0: check Language
            var local = _TS._local();
            Assert.IsNotNull(local);
            Assert.IsNotNull(local.Language);
            switch (local.Language)
            {
                case "rus": case "enu": case "esp": case "nld": case "deu":  break;
                default:
                    Assert.Fail($"Неизвестный язык ={local.Language}");
                    break;
            }

            // test 1: LocalTxt(40), LocalTxt("..40")
            string s = _TS.LocalTxt("by_number_msg_no_40");
            string v = _TS.LocalTxt(40);
            Assert.AreEqual(s, v);
            switch (local.Language)
            {
                case "rus": Assert.AreEqual("Не является профилем балки", s); break;
                case "enu": Assert.AreEqual("Not a beam profile", s); break;
                case "esp": Assert.AreEqual("No es un perfil de viga", s); break;
                case "nld": Assert.AreEqual("No es un perfil de viga", s); break;
                case "deu": Assert.AreEqual("Selektiertes Profil ist kein Träger-Typ", s); break;
            }

            // test 2: несколько параметров LocalTxt
            s = _TS.LocalTxt(283, 333);
            switch (local.Language)
            {
                case "rus": Assert.AreEqual("Соединения с балкой:Укажите деталь", s); break;
                default:
                    Assert.Fail($"Неизвестный язык ={local.Language}");
                    break;
            }

            // test 3: mix txt and int
            if (local.Language == "rus")
                Assert.AreEqual("Соединения с балкой: Укажите деталь", _TS.LocalTxt(283, " ", 333));
        }

        [TestMethod()]
        public void UT_ReperShow()
        {
            Point p1 = new Point(1000, 1000, 0);
            Point p2 = new Point(2000, 3000, 0);
            ThisBeam = _TS.CreateBeam("test Beam", "I50B1_20_93", p1, p2);
            var cs = ThisBeam.GetCoordinateSystem();

            Assert.AreEqual(1000.0, cs.Origin.X);
            Assert.AreEqual(1000.0, cs.Origin.Y);
            Assert.AreEqual(-246.0, cs.Origin.Z);

            _TS.ReperShow(cs);
        }

        [TestMethod()]
        public void UT_PointShow()
        {
            // test 0: в точке [1000, 3000] появляется метка и пустой текст
            Point p = new Point(1000, 3000);
            _TS.PointShow(p);

            // test 1: в точке [1500, 2500] появляется метка точки и буква "p"
            _TS.PointShow(new Point(1500, 2500), "p");

            // test 2: в точке [1800, 800] выводим координаты на экран Tekla
            _TS.PointXYZ(new Point(1800, 800));
        }
        #endregion --- Txt, LocalTxt, PointShow, ReperShow

        [TestMethod()]
        public void UT_PlaneShow()
        {
            Plane plane = new Plane();
            plane.Origin = new Point(1000, 1000, 0);
            plane.AxisX = new Vector(100, -40, 50);
            plane.AxisY = new Vector(40, 100, 50);

            CoordinateSystem cs = new CoordinateSystem(plane.Origin, plane.AxisX, plane.AxisY);
    //        _TS.PlaneShow(CoordinateSystem())

            _TS.PlaneShow(cs);
        }

        [TestMethod()]
        public void UT_Tekla_SetWorkPlane()
        {
            // Set coordinate system with new Beam
            T3D.Point p1 = new T3D.Point(0, 1500, 0);
            T3D.Point p2 = new T3D.Point(1500, 1500, 0);
            ThisBeam = _TS.CreateBeam("test Beam", "I20B1_20_93", p1, p2);
            Assert.AreEqual("C245", ThisBeam.Material.MaterialString);

            _TS._SetWorkPlane(ThisBeam);

            //          Assert.IsNotNull(plane);
        }

        [TestMethod()]
        public void UT_Tekla_CreateBeam()
        {
            // test 0: создаем балку вдоль оси Х, а потом ее стираем
            Point p1 = new Point(0, 500, 0);
            Point p2 = new Point(500, 500, 0);
            ThisBeam = _TS.CreateBeam("test Beam", "I50B1_20_93", p1, p2);

            Assert.AreEqual("C245", ThisBeam.Material.MaterialString);
            //           ThisBeam.Delete();
            Model.CommitChanges();
        }

        [TestMethod()]
        public void UT_W36()
        {
            Point p1 = new Point(0, 2000, 0);
            Point p2 = new Point(3000, 0, 0);
            MainBeam = _TS.CreateBeam("Main Beam", "I30B1_20_93", p1, p2);
            _TS._SetWorkPlane(MainBeam);

            Point p3 = new Point(400, 0, 0);
            Point p4 = new Point(400, 0, 2000);
            AttBeam = _TS.CreateBeam("Att Beam", "I20B1_20_93", p3, p4
                , PositionRotation:(int)Position.RotationEnum.BACK
                , PositionPlane:(int)Position.PlaneEnum.MIDDLE
                , PositionDepth:(int)Position.DepthEnum.MIDDLE
                , Class:"5");

            // толщина стенки AttBeam - WEB_THICKNESS
            double BeamThickness = -1;
            AttBeam.GetReportProperty("WEB_THICKNESS", ref BeamThickness);
            if (BeamThickness < 5 || BeamThickness > 23 )
                throw new Exception($"Wrong AttBeam thickness={BeamThickness}");

            // do it later            _TS.Node36(MainBeam, AttBeam);

            // тав
            int l_tav = 160;
            int l_size = 40;
            double w_tav = 13.5;
            double xTav = 400 + (BeamThickness + w_tav)/2;
            Point p5 = new Point(xTav, - l_tav/2);
            Point p6 = new Point(xTav, l_tav / 2);
            ThisBeam = _TS.CreateBeam("вут", "T40BT1_14_2_685_86", p5, p6, Class:"6",
                PositionRotation:(int)Position.RotationEnum.BELOW,
                PositionPlane:(int)Position.PlaneEnum.MIDDLE,
                PositionDepth:(int)Position.DepthEnum.FRONT);
        }

        [TestMethod()]
        public void UT_CutBeamByLine()
        {
            // Пример 1: из TeklaAPI CutPlate Example
            //.. создает балку в [0,0 - 1000,0] и обрезает ее по CutPlate
            Point Point = new Point(0, 0, 0);
            Point Point2 = new Point(1000, 0, 0);

            ThisBeam = new Beam();
            ThisBeam.StartPoint = Point;
            ThisBeam.EndPoint = Point2;
            ThisBeam.Profile.ProfileString = "HEA400";
            ThisBeam.Finish = "PAINT";
            ThisBeam.Insert();
            Model.CommitChanges();

            CutPlane CutPlane = new CutPlane();
            CutPlane.Plane = new Plane();
            CutPlane.Plane.Origin = new Point(400, 0, 0);
            CutPlane.Plane.AxisX = new Vector(0, 500, 0);
            CutPlane.Plane.AxisY = new Vector(0, 0, -1000);
            CutPlane.Father = ThisBeam;
            CutPlane.Insert();
            Model.CommitChanges();
            // В результате на экране Tekla создается балка вдоль оси Х
            //..и стирается ее часть, ближняя к точке (0,0) на Х=400.
            //..Чтобы стирать другую часть балки, поменяйте   

            // test 0: создаем балку вдоль оси Х, а потом ее стираем
            Point p1 = new Point(0, 2500, 0);
            Point p2 = new Point(2500, 0, 0);
            MainBeam = _TS.CreateBeam("MainBeam", "I50B1_20_93", p1, p2);

            // создаем режущую плоскость
            CutPlane BeamLineCut = new CutPlane();
                // Father - Объект модели над которым выполняется действие 
            BeamLineCut.Father = this.MainBeam;
            Plane BeamCutPlane = new Plane();
            BeamCutPlane.Origin = new Point(1000, 0, 0);
            BeamCutPlane.AxisX = new Vector(0, 0, 500);
            // Changing the positive vs. negative value here determines which direction
            // the line cut will take away material where as fitting looks at which end
            // of beam it is closest to figure out how to cut.
            BeamCutPlane.AxisY = new Vector(0, -500, 0);
            BeamLineCut.Plane = BeamCutPlane;
            BeamLineCut.Insert();

            Model.CommitChanges(); 
        }

        [TestMethod()]
        public void UT_CutBeamByPart()
        {
            // test 0: обрезаем балку ThisBeam балкой MainBeam
            //.. создаем балку [0, 1000 - 2000,1000] I20 и обрезает ее
            //.. балкой MainBeam [500,0 - 500, 1500] I50
            Point p1 = new Point(0, 3000);
            Point p2 = new Point(2000, 3000);
            ThisBeam = _TS.CreateBeam("ThisBeam", "I20B1_20_93", p1, p2
                , PositionDepth:(int) Position.DepthEnum.MIDDLE);

            Point cp1 = new Point(1000, 0);
            Point cp2 = new Point(1000, 4000);
            MainBeam = _TS.CreateBeam("MainBeam", "I50B1_20_93", cp1, cp2
                , PositionDepth: (int)Position.DepthEnum.MIDDLE
                , Class: BooleanPart.BooleanOperativeClassName);

            ThisBeam = _TS.CutBeamByPart(ThisBeam, MainBeam, false);

            // стираем ThisBeam и MainBeam
  //          ThisBeam.Delete();
 //           MainBeam.Delete();
 //           Model.CommitChanges();

            // test 1: обрезаем балку ThisBeam пластиной Plate
            p1 = new Point(3000, 2000, 1000);
            p2 = new Point(5000, 1000, 0);
            ThisBeam = _TS.CreateBeam("ThisBeam", "I20B1_20_93", p1, p2);
            ContourPoint c1 = new ContourPoint(new Point(4000, 1000, 0), null);
            ContourPoint c2 = new ContourPoint(new Point(4000, 3000, 0), null);
            ContourPoint c3 = new ContourPoint(new Point(4000, 3000, 2000), null);
            ContourPoint c4 = new ContourPoint(new Point(4000, 1000, 2000), null);
            var Plate = new ContourPlate();
            Plate.AddContourPoint(c1);
            Plate.AddContourPoint(c2);
            Plate.AddContourPoint(c3);
            Plate.AddContourPoint(c4);
            Plate.Profile.ProfileString = "PL200";
            Plate.Material.MaterialString = "C245";
            Plate.Class = "2";
            Plate.Name = "PLATE";
            Plate.Insert();
            Model.CommitChanges();

            ThisBeam = _TS.CutBeamByPart(ThisBeam, Plate, false);

   //         ThisBeam.Delete();
            Plate.Delete();
            Model.CommitChanges();
        }
    }

    class U : TeklaAPI
    {
        internal Model GetModel() => Model;
        internal TransformationPlane GetModelPlane() => ModelPlane;
        internal TransformationPlane GetTmpPlane() => TmpPlane;
        internal TransformationPlane _SetWorkPlane(Beam beam = null)
        {
            SetWorkPlane(beam);
            return GetTmpPlane();
        }
        internal TSDL _local() => local;
    }
}