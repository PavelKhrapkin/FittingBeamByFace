/* -----------------------------------------------------
* JoinLib module Unit Tests  25.06.2018 Pavel Khrapkin
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FittingBeamByFace.SteelJoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FittingBeamByFace.SteelJoin.Tests
{
    [TestClass()]
    public class UT_JoinLibTests
    {
        U JL = new U();

        [TestMethod()]
        public void UT_M20_7798_Length()
        {
            // test 0: проверяем результат M20_7798_Length(7, 3)
            //..соединяемые детали толщиной 7 и "запас" 3 мм -> 30 мм
            double lng = JL.M20_7798_Length(7, 3);
            Assert.AreEqual(30, lng);

            lng = JL.M20_7798_Length(33);  // например, соединение 15 +15 + 3
            Assert.AreEqual(55, lng);

            lng = JL.M20_7798_Length(220, 40);
            Assert.AreEqual(280, lng);

            // test 1: параметры M20_Length вне пределов 
            //!! сейчас этот тест заткнут, поскольку для его запуска 
            //!! нужно использовать технику подстановок вывода 
            //!! сообщенй об ошибках TSerror Mock -- ОТЛОЖИЛ
            //           lng = _TS.M20_7798_Length(10000);
        }
    }

    class U : JoinLib
    { 
    }
}