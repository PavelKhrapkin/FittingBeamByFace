/* -----------------------------------------------------------------------
 * Упражнения с TeklaAPI    5.06.2018 Pavel Khrapkin
 * Подходы к созданию узла W36
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using T3D = Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;

namespace TeklaAPI
{
    public partial class TeklaAPI
    {
        public void DevelopW36()
        {

        }

        public void CheckCrossBeam()
        {
            var beam = PickBeam("Select Main Beam");
            if (beam != null)
            {
                var psk = beam.GetCoordinateSystem();
                ReperShow(beam.GetCoordinateSystem());
                PointXYZ(beam.StartPoint);
                PointXYZ(beam.EndPoint);
            }
        }

        public Beam PickBeam(string txt)
        {
            ModelObject PickedObject = null;
            Picker Picker = new Picker();
  //          Picker.PickObject(txt);
            try
            {
               PickedObject = Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
            }
            catch { PickedObject = null; }
            //if (MainBeam == null)
            //{
            //    MessageBox.Show("Основная балка не выбрана");
            //    return null;
            //}
            return (Beam) PickedObject;
        }
    }
}
