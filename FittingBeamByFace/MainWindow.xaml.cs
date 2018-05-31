/* ------------------------------------------------------------------
 *  Головной модуль MainWindow.cs       25.05.2018 Pavel Khrapkin 
 *  
 *  Использует модули TeklaAPI разделенные на 3 части partial
 *  1) TeklaLib     - библиотечные методы
 *  2) TeklaExercise - упражнения Сени Бусина, Криса Кейак и мои
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

        public MainWindow()
        {
            InitializeComponent();
            TS.Init(this);
            ProfileText.Text = "I20B1_20_93";
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
        public string prfStr = string.Empty;

        private void Button_CK07_Peak2Points_Click(object sender, RoutedEventArgs e)
        {
            prfStr = ProfileText.Text;
            if (prfStr == "") prfStr = "I20B1_20_93";
            InvokeTS(TS.CK07_Beam, CK07_1);
        }

        private void InputProfile(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return) prfStr = ProfileText.Text;
        }

        private void Button_CK07_Column_Click(object sender, RoutedEventArgs e)
        {
            InvokeTS(TS.CK07_Column, CK07_2);
            //context = CK07_2;
            //Work xx = new Work(TS.CK07_Column);
            //xx.BeginInvoke(null, null);        
        }

        private void Button_CK07_Polibeam_Click(object sender, RoutedEventArgs e)
        {
            TS.CK07_Polibeam();
        }
        #endregion --- Chris Keyack Session 07 ---

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

        private void Button_DrawReper_Click(object sender, RoutedEventArgs e)
        {
            TS.ExReper();
        }
        #endregion --- Сеня Бусин ---

        #region --- W36 ---
        private void Button_GoW36_Click(object sender, RoutedEventArgs e)
        {
            TS.DevelopW36();
        }
        #endregion --- W36 ---
    }
}
