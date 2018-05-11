/* ------------------------------------------------------------------
 *  Tutorial Сеня Бусин https://www.youtube.com/watch?v=S-d0TBqMqVM
 *  TeklaOpenAPI Tutorial. Creating macro fitting a beam by face
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
    }
}
