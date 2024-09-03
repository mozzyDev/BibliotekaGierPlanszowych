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
    /// Interaction logic for GetCollection.xaml
    /// </summary>
    public partial class GetCollection : Window
    {
        private int user;
        public GetCollection(int userID)
        {
            user = userID;
            InitializeComponent();
        }

        private void Get_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(User_txtbox.Text))
            {
                string userName = User_txtbox.Text;
                APIBGG bgg = new APIBGG();
                String QueryCollectionBgg = "";
                DBConnection db = new DBConnection();
                DateTime today = DateTime.Today;
                string titleName = "";
                string rating = "0";

                List<Item> bggGameCollectionList = bgg.GetBoardGamesForUser(userName);
                if (bggGameCollectionList.Count > 0)
                {
                    MessageBox.Show("Liczba znalezionych gier: " + bggGameCollectionList.Count.ToString());
                    //Dodanie gier do listy
                    foreach (var item in bggGameCollectionList)
                    {
                        
                        if (item.stats.Rating.Value.Equals("N/A"))
                        {
                            rating = "0";
                        }
                        else
                        {
                            rating = item.stats.Rating.Value.ToString();
                        }
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(item.Name);
                        titleName = System.Text.Encoding.UTF8.GetString(bytes);
                        int rate;
                        if (Convert.ToInt32(rating) > 1)
                            rate = Convert.ToInt32(rating);
                        else
                            rate = 1;


                        QueryCollectionBgg = "INSERT or ignore into board_game (title, min_players, max_players, rate, add_date, id_category, "
                           + "yearpublished, playingtime, image_url, id_user) VALUES ('"
                           + titleName + "', " + item.stats.MinPlayers + ", " + item.stats.MaxPlayers + ", " + rate + ", '" + today.ToShortDateString() + "', 7 , "  //7 to brak kategorii
                            + item.YearPublished + ", " + item.stats.PlayingTime + ", '" + item.Image + "', "+user+")";

                        db.DatabaseDataChange(QueryCollectionBgg);

                        QueryCollectionBgg = "";
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nic nie znaleziono", "Informacja");
                }
            }
            else
            {
                MessageBox.Show("Należy wpisać nazwę użytkownika BGG", "Informacja");
            }
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
