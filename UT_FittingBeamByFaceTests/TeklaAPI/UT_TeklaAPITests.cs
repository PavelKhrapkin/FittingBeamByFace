/* -----------------------------------------------------
 * Tekla module Unit Tests  25.04.2018 Pavel Khrapkin
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

namespace TeklaAPI.Tests
{
    [TestClass()]
    public class UT_Tekla
    {
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

        [TestMethod()]
        public void UT_Tekla_Init()
        {
            _TS.Init();
            Assert.IsNotNull(Model);
            Assert.IsNotNull(ModelPlane);
        }

        [TestMethod()]
        public void UT_Tekla_CreateBeam()
        {
            // test 0: создаем балку вдоль оси Х, а потом ее стираем
            T3D.Point p1 = new T3D.Point(0, 500, 0);
            T3D.Point p2 = new T3D.Point(500, 500, 0);
            ThisBeam = _TS.CreateBeam("test Beam", "I50B1_20_93", p1, p2);

            Assert.AreEqual("C245", ThisBeam.Material.MaterialString);
            //           ThisBeam.Delete();
            Model.CommitChanges();
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
    }
}