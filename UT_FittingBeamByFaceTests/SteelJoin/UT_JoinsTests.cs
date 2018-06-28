/* --------------------------------------------------------------------
 * UT_JoinsTests - Unit Tests узлов Joins    27.06.2018 Pavel Khrapkin
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace FittingBeamByFace.SteelJoin.Tests
{
    [TestClass()]
    public class UT_JoinsTests
    {
        const string W36 = "W36_";   // this test Name prefix

        _TeklaAPI TS = new _TeklaAPI();
        JoinLib JL = new JoinLib();
        Joins _J = new Joins();

        Model Model;
        TransformationPlane ModelPlane, TmpPlane;

        [TestInitialize]
        public void Initialyze()
        {
            TS.Init();
            Model = TS.GetModel();
            if (!Model.GetConnectionStatus())
                throw new Exception("Tekla Model not connected!");
            ModelPlane = TS.GetModelPlane();
            TmpPlane = TS.GetTmpPlane();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Read all Model Elements and delete Parts with prefix W36
            var selector = Model.GetModelObjectSelector();
            Type[] Types = new Type[1];
            Types.SetValue(typeof(Part), 0);
            ModelObjectEnumerator objParts = selector.GetAllObjectsWithType(Types);
            while(objParts.MoveNext())
            {
                Part myObj = objParts.Current as Part;
                if (myObj == null) continue;
                if (myObj.Name.Substring(0, W36.Length) == W36) myObj.Delete();
            }
            TS._SetWorkPlane(); // restore coordinate system to ModelPlain
        }

        [TestMethod()]
        public void UT_W36()
        {
            // test 0: создаем MainBeam и AttBeam, потом соединяем их W36
            Point p1 = new Point(0, 2000, 0);
            Point p2 = new Point(3000, 0, 0);
            Beam MainBeam = TS.CreateBeam(W36+"Main Beam", "I30B1_20_93", p1, p2);
            TS._SetWorkPlane(MainBeam);

            Point p3 = new Point(700, 0, 0);
            Point p4 = new Point(700, 0, 2000);
            Beam AttBeam = TS.CreateBeam(W36+"Att Beam", "I20B1_20_93", p3, p4
                , PositionRotation: (int)Position.RotationEnum.BACK
                , PositionPlane: (int)Position.PlaneEnum.MIDDLE
                , PositionDepth: (int)Position.DepthEnum.MIDDLE
                , Class: "5");
            _J.W36(MainBeam, AttBeam);

         
        }
    }

    class _TeklaAPI : TeklaAPI.TeklaAPI
    {
        internal Model GetModel() => Model;
        internal TransformationPlane GetModelPlane() => ModelPlane;
        internal TransformationPlane GetTmpPlane() => TmpPlane;
        internal TransformationPlane _SetWorkPlane(Beam beam = null)
        {
            SetWorkPlane(beam);
            return GetTmpPlane();
        }
//        internal TSDL _local() => local;

        internal string _TSerror() => message;
        //           Mock<TeklaAPI> mock = new Mock<TeklaAPI>();
        private string message;
        public new void TSerror(string msg) { message = msg; }
    }
}