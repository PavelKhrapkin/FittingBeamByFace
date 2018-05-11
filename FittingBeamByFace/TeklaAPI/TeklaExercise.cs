/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    11.05.2018 Pavel Khrapkin
 * 
 *  Tutorial Сеня Бусин https://www.youtube.com/watch?v=S-d0TBqMqVM
 *  TeklaOpenAPI Tutorial. Creating macro fitting a beam by face
 *  
 *  Chris Keyack Session 06 https://www.youtube.com/watch?v=TuSVLPB5NyI
 *  Pick Points and set WorkPlane
 * 
 */
using System.Collections;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        public void Pick2Points()
        {
            T3D.Point FirstPoint = null;
            T3D.Point SecondPoint = null;
            TSMUI.Picker Picker = new TSMUI.Picker();
            try
            {
                ArrayList PickPoints = Picker
                    .PickPoints(TSMUI.Picker.PickPointEnum.PICK_TWO_POINTS);
                FirstPoint = PickPoints[0] as T3D.Point;
                SecondPoint = PickPoints[1] as T3D.Point;
            }
            catch { FirstPoint = SecondPoint = null; }

            if (FirstPoint != null && SecondPoint != null)
            {
                T3D.Vector XVector = new T3D.Vector(SecondPoint.X - FirstPoint.X,
                    SecondPoint.Y - FirstPoint.Y, SecondPoint.Z - FirstPoint.Z);
                T3D.Vector YVector = XVector.Cross(new T3D.Vector(0, 0, -1));
                Model.GetWorkPlaneHandler()
                    .SetCurrentTransformationPlane(new TransformationPlane(FirstPoint
                    , XVector, YVector));
                ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
                Model.CommitChanges();
            }
        }

        public void PickPart()
        {
            ModelObject PickedObject = null;
            Picker Picker = new Picker();
            try
            {
                PickedObject = Picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_OBJECT);
            }
            catch { PickedObject = null; }

            if (PickedObject != null)
            {
                T3D.CoordinateSystem ObjectSystem = PickedObject.GetCoordinateSystem();
                Model.GetWorkPlaneHandler()
                    .SetCurrentTransformationPlane(new TransformationPlane(ObjectSystem));

                double StartX = 500;
                for (int i = 0; i < 7; i++)
                {
                    T3D.Point p1 = new T3D.Point(StartX, 0);
                    T3D.Point p2 = new T3D.Point(StartX, 0, 1000);
                    CreateBeam("test Beam", "I30B1_20_93", p1, p2);
                    StartX += 500;
                }
                ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
                Model.CommitChanges();
            }
        }

        public void Global()
        {
            Model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(new TransformationPlane());
            ViewHandler.SetRepresentation("standard"); //PKh> should be add for Tekla-2018
            Model.CommitChanges();
        }
    }
}
