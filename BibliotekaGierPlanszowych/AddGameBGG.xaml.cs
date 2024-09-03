using System;
using System.Collections.Generic;
using System.Data;
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
        private DataColumn Bgg_gameTitle = new DataColumn("BggTitle", typeof(string));
        private int user;
        public AddGameBGG(int userID)
        {
            user = userID;
            InitializeComponent();
            AddBgg_btn.IsEnabled = false;
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

            List<BoardGame> bggGameList = bgg.SearchBoardGame(gameName);
            if (bggGameList.Count > 0)
            {
                List_BggGameList.ItemsSource = null;
                List_BggGameList.ItemsSource = bggGameList;
                DataGridColumn column = List_BggGameList.Columns[0];
                column.Width = new DataGridLength(240);
                DataGridColumn column2 = List_BggGameList.Columns[1];
                column2.Width = new DataGridLength(120);
                DataGridColumn column3 = List_BggGameList.Columns[2];
                column3.Visibility = Visibility.Collapsed;
                AddBgg_btn.IsEnabled = true;
            }
            else
                MessageBox.Show("Nic nie znaleziono", "Informacja");
        }

        //pobieranie danych z GridView

        private void ButtonAddBGG_Click(object sender, RoutedEventArgs e)
        {
            int gameId = 0; ;
            if (List_BggGameList.SelectedItem != null)
            {
                BoardGame gameToGet = (BoardGame)List_BggGameList.SelectedItem;
                gameId = gameToGet.Id;
            }
            if (gameId != 0)
            {
                APIBGG bgg = new APIBGG();

                List<BoardGameBgg> bggGameList = bgg.AddBoardGame(gameId);

                if (bggGameList.Count > 0)
                {
                    DBConnection db = new DBConnection();
                    DateTime today = DateTime.Today;
                    
                    string gameName = "";
                    foreach (var item in bggGameList)
                    {

                        foreach (var item2 in item.Name)
                        {
                            if (item2.IsPrimary)
                            {
                                byte[] bytes = System.Text.Encoding.Default.GetBytes(item2.Value);
                                gameName = System.Text.Encoding.UTF8.GetString(bytes);
                            }
                        }

                        String QueryGameBgg = "INSERT OR REPLACE INTO board_game (title, min_players, max_players, rate, add_date, id_category, "
                            + "yearpublished, playingtime, minplaytime, maxplaytime, age, image_url, id_user, rate) VALUES ('"
                            + gameName + "', " + item.MinPlayers + ", " + item.MaxPlayers + ", 1 , '" + today.ToShortDateString() + "', 7 , "  //7 to brak kategorii
                            + item.YearPublished + ", " + item.PlayingTime + ", " + item.MinPlayTime + ", " + item.MaxPlayTime + ", " + item.Age + ", '" + item.image_url + "', "+user+", 1)";
                        db.DatabaseDataChange(QueryGameBgg);

                    }
                }
            }
            else
            {
                MessageBox.Show("Nie zaznaczono żadnego rekordu");
            }
        }
    }

}
