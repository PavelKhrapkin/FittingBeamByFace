/* ------------------------------------------------------------------
 *  Головной модуль MainWindow.cs      21.06.2018 Pavel Khrapkin 
 *  
 *  Использует модули TeklaAPI разделенные на 3 части partial
 *  1) TeklaLib     - библиотечные методы
 *  2) TeklaExercise - упражнения Сени Бусина, Криса Кейак и мои
 *                     (разбито на отдельные файлы TeklaExercise_CK06..)
 *  3) TeklaAPI     - отдадка, в состоянии между 1) и 2)
 *  
 *  Unit Test - UT_TeklaAPI
 */
using System.Windows;
using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace FittingBeamByFace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TeklaAPI.TeklaAPI TS = new TeklaAPI.TeklaAPI();
        public delegate void Work();
        private TextBox context;
        public const string IDEFAULT = "I20B1_20_93";
        public string prfStr = IDEFAULT;

        public MainWindow()
        {
            InitializeComponent();
            TS.Init(this);
            ProfileText.Text = prfStr;
        }

        /// <summary>
        /// http://the--semicolon.blogspot.ru/p/change-wpf-window-label-content-from.html
        /// </summary>
        /// <param name="v"></param>
        public void Msg(string v = "")
        {
            Dispatcher.Invoke(new Action(() => 
            {
                if (context == null) return;
                context.Text = v;
                if (v == "")
                {
                    context.Background = System.Windows.Media.Brushes.White;
                    context.Height = 0;
                }
                else
                {
                    context.Background = System.Windows.Media.Brushes.Yellow;
                    int n = context.LineCount;
                    context.Height = 18*n;
                }               
            }));
            Thread.Sleep(100);
        }

        private void InvokeTS(Action sub, dynamic explanation)
        {
            context = explanation;
            Work xx = new Work(sub);
            xx.BeginInvoke(null, null);
        }

        #region --- Chris Keyack Session 06 ---
        private void Button_CK06_Peak2Points_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK06_Pick2Points, CK06_1);


        private void Button_CK06_PickPart_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK06_ByPickPart, CK06_2);
  
        private void Button_CK06_Global_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK06_Global, CK06_3);

        #endregion --- Chris Keyack Session 06 ---

        #region --- Chris Keyack Session 07 ---
        private void Button_CK07_Peak2Points_Click(object sender, RoutedEventArgs e) 
            => InvokeTS(TS.CK07_Beam, CK07_1);

        private void InputProfile(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return) prfStr = ProfileText.Text;
            if (prfStr == "") prfStr = IDEFAULT; ;
        }

        private void Button_CK07_Column_Click(object sender, RoutedEventArgs e) 
            => InvokeTS(TS.CK07_Column, CK07_2);

        private void Button_CK07_Polibeam_Click(object sender, RoutedEventArgs e)
             => InvokeTS(TS.CK07_Polybeam, CK07_3);
        #endregion --- Chris Keyack Session 07 ---

        #region --- Chris Keyack Session 08 ---
        private void Button_CK08_CreatePlate_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK08_CreatePlate, CK08_1);

        private void Button_CK08_SetWorkPlane_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK08_SetWorkPlane, CK08_2);
        #endregion --- Chris Keyack Session 08 ---

        #region --- Chris Keyack Session 09 ---
        private void Button_CK09_SetWorkplane_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK09_SetWorkplane, CK09_1);

        private void Button_CK09_ApplyFitting_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK09_ApplyFitting, CK09_2);

        private void Button_CK09_PartCut_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK09_PartCut, CK09_3);

        private void Button_CK09_PolygonCut_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK09_PolygonCut, CK09_4);
        #endregion --- Chris Keyack Session 09 ---

        #region --- Chris Keyack Session 10 ---
        private void Button_CK10_Bolt_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK10_Bolt, CK10_1);

        private void Button_CK10_SetWorkPlane_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK10_SetWorkPlane, CK10_1);

        private void Button_CK10_Weld_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK10_Weld, CK10_2);

        private void Button_CK10_AddTo_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.CK10_AddTo, CK10_3);

        private void Button_CK10_GetAssembly_Click(object sender, RoutedEventArgs e)
          => InvokeTS(TS.CK10_GetAssembly, CK10_4);
        #endregion --- Chris Keyack Session 10 ---

        #region --- Сеня Бусин ---
        private void Button_Go_Click(object sender, RoutedEventArgs e)
        {
            TS.FittingBeamByFace();
        }

        private void Button_Coordinate_Click(object sender, RoutedEventArgs e)
        {
            TS.CoordinateOfLine();
//            TS.ExtendControlLine();
        }

        private void Button_Rep_Click(object sender, RoutedEventArgs e)
            => InvokeTS(TS.SB_ExReper, SB_01);
        #endregion --- Сеня Бусин ---

        #region --- W36 ---
        private void Button_GoW36_Click(object sender, RoutedEventArgs e)
        {
            TS.DevelopW36();
        }

        private void Button_W36_CheckCrossBeam(object sender, RoutedEventArgs e)
        {
            TS.CheckCrossBeam();
        }
        #endregion --- W36 ---
    }
}
