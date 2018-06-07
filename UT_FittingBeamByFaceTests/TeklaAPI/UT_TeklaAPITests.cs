using TeklaAPI;
/* -----------------------------------------------------
* Tekla module Unit Tests  1.06.2018 Pavel Khrapkin
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;
using Tekla.Structures;
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
        public void UT_Tekla_Node36()
        {
            T3D.Point p1 = new T3D.Point(0, 2000, 0);
            T3D.Point p2 = new T3D.Point(3000, 0, 0);
            MainBeam = _TS.CreateBeam("Main Beam", "I30B1_20_93", p1, p2);
            _TS._SetWorkPlane(MainBeam);

            T3D.Point p3 = new T3D.Point(400, 0, 0);
            T3D.Point p4 = new T3D.Point(400, 0, 2000);
            AttBeam = _TS.CreateBeam("Att Beam", "I20B1_20_93", p3, p4);
            AttBeam.Position.Depth = Position.DepthEnum.FRONT;
            AttBeam.Position.Rotation = Position.RotationEnum.BACK;

            // do it later            _TS.Node36(MainBeam, AttBeam);

            T3D.Point p5 = new T3D.Point(400, 0, 0);
            T3D.Point p6 = new T3D.Point(400, 0, 50);
            ThisBeam = _TS.CreateBeam("вут", "T40BT1_14_2_685_86", p5, p6);
        }

        [TestMethod()]
        public void Example1()
        {
          
        }


        [TestMethod()]
        public void UT_CutBeamByLine()
        {
            // Пример 1: из TeklaAPI CutPlate Example
            //.. создает балку в [0,0 - 1000,0] и обрезает ее по CutPlate
            Point Point = new Point(0, 0, 0);
            Point Point2 = new Point(1000, 0, 0);

            Beam Beam = new Beam();
            Beam.StartPoint = Point;
            Beam.EndPoint = Point2;
            Beam.Profile.ProfileString = "HEA400";
            Beam.Finish = "PAINT";
            Beam.Insert();
            Model.CommitChanges();

            CutPlane CutPlane = new CutPlane();
            CutPlane.Plane = new Plane();
            CutPlane.Plane.Origin = new Point(400, 0, 0);
            CutPlane.Plane.AxisX = new Vector(0, 500, 0);
            CutPlane.Plane.AxisY = new Vector(0, 0, -1000);
            CutPlane.Father = Beam;
            CutPlane.Insert();
            Model.CommitChanges();

            // test 0: создаем балку вдоль оси Х, а потом ее стираем
            Point p1 = new Point(0, 2500, 0);
            Point p2 = new Point(2500, 0, 0);
            ThisBeam = _TS.CreateBeam("MainBeam", "I50B1_20_93", p1, p2);

            // создаем режущую плоскость
            CutPlane BeamLineCut = new CutPlane();
            BeamLineCut.Father = ThisBeam;
            Plane BeamCutPlane = new Plane();
            BeamCutPlane.Origin = new Point(1000, 0, 0);
            BeamCutPlane.AxisX = new Vector(0, 0, 500);
            // Changing the positive vs. negative value here determines which direction
            // the line cut will take away material where as fitting looks at which end
            // of beam it is closest to figure out how to cut.
            BeamCutPlane.AxisX = new Vector(0, -500, 0);
            BeamLineCut.Plane = BeamCutPlane;
            BeamLineCut.Insert();

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