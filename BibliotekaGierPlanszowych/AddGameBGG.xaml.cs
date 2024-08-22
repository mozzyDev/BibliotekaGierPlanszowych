using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Interaction logic for AddGameBGG.xaml
    /// </summary>
    public partial class AddGameBGG : Window
    {
        public AddGameBGG()
        {
            InitializeComponent();
        }
        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            string gameName = Title_txtbox.Text;

            // Wywołaj metodę wyszukiwania
            APIBGG bgg = new APIBGG();

            List<string> gameReturn = bgg.SearchBoardGame(gameName);
            if(gameReturn.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in gameReturn)
                {
                    sb.Append(item+"/n");
                }
                MessageBox.Show("Znaleziono: " + sb.ToString(), "Informacja");
            }
                
            else
                MessageBox.Show("Nic nie znaleziono", "Informacja");
        }
    }
}
