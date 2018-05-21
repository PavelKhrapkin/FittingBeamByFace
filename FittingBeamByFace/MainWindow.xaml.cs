/* ------------------------------------------------------------------
 *  Головной модуль MainWindow.cs       11.05.2018 Pavel Khrapkin 
 *  
 *  Использует модули TeklaAPI разделенные на 3 части partial
 *  1) TeklaLib     - библиотечные методы
 *  2) TeklaExercise - упражнения Сени Бусина, Криса Кейак и мои
 *  3) TeklaAPI     - отдадка, в состоянии между 1) и 2)
 *  
 *  Unit Test - UT_TeklaAPI
 */
using System.Windows;
using System.Windows.Input;

namespace FittingBeamByFace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TeklaAPI.TeklaAPI TS = new TeklaAPI.TeklaAPI();

        public MainWindow()
        {
            InitializeComponent();
            TS.Init();
        }

        #region --- Chris Keyack Session 06 ---
        private void Button_CK06_Peak2Points_Click(object sender, RoutedEventArgs e)
        {
            TS.CK06_Pick2Points();
        }

        private void Button_CK06_PickPart_Click(object sender, RoutedEventArgs e)
        {
            TS.CK06_ByPickPart();
        }

        private void Button_CK06_Global_Click(object sender, RoutedEventArgs e)
        {
            TS.CK06_Global();
        }
        #endregion --- Chris Keyack Session 06 ---

        #region --- Chris Keyack Session 07 ---
        private string prfStr = string.Empty;

        private void Button_Beam_Click(object sender, RoutedEventArgs e)
        {
            if (prfStr == "") prfStr = "I20B1_20_93";
            TS.Beam(prfStr);
        }

        private void InputProfile(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return) prfStr = ProfileText.Text;
        }

        private void Button_Column_Click(object sender, RoutedEventArgs e)
        {
            TS.Column();
        }

        private void Button_Polibeam_Click(object sender, RoutedEventArgs e)
        {
            TS.Polibeam();
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
