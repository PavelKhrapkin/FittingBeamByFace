/* ------------------------------------------------------------------
 * TeklaAPI - module is working with TeklaStructure over OpenAPI
 * 
 * 24.04.2018 Pavel Khrapkin NIP Informatica, St.-Petersburg
 * 
 * --- Methods: ---
 * Init()   - setup Model connection, save ModelPlane
 * CreateBeam(name, prfString, p1, p2)  - create ThisBeam from p1 to p2
 * SetWorkPlane([theBeam])  - create workplane at theBeam, 
 *                            by default,restore saved
 * Node36(MainBeam, AttBeam) - implement Node36
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;

namespace TeklaAPI
{
    public class TeklaAPI
    {
        protected Model Model;
        protected TransformationPlane ModelPlane, TmpPlane;
        Beam MainBeam = null, AttBeam = null;

        public void Init()
        {
            Model = new Model();
            if (!Model.GetConnectionStatus())
                throw new Exception("Tekla not connected");

            // Save Tekla Model Workplane to restore after change to any Part Plane
            SetWorkPlane();
            ModelPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            Model.CommitChanges();
        }

        public Beam CreateBeam(string name, string prfStr, T3D.Point p1, T3D.Point p2)
        {
            if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
            {
                MessageBox.Show("StartPoint and EndPoint are the same -- Beam not Created.");
                return null;
            }
            Beam ThisBeam = new Beam();
            ThisBeam.StartPoint = p1;
            ThisBeam.EndPoint = p2;
            ThisBeam.Profile.ProfileString = prfStr;
            ThisBeam.Material.MaterialString = "C245";
            ThisBeam.Class = "7";
            ThisBeam.Insert();
            Model.CommitChanges();
            return ThisBeam;
        }

        protected void SetWorkPlane(Beam theBeam = null)
        {
            if (theBeam != null)
            {
                T3D.CoordinateSystem BeamCoordinateSystem = theBeam.GetCoordinateSystem();
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(BeamCoordinateSystem));
                Model.CommitChanges();
                ViewHandler.SetRepresentation("standard");
                return;
            }

            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            Model.CommitChanges();
            ViewHandler.SetRepresentation("standard");
        }

        public void Node36(Beam MainBeam, Beam AttBeam)
        {

        }
    }
}