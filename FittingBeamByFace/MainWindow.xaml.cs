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

        private void Button_Pick2Points_Click(object sender, RoutedEventArgs e)
        {
            TS.Pick2Points();
        }

        private void Button_PickPart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Global_Click(object sender, RoutedEventArgs e)
        {

        }

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

    }
}
