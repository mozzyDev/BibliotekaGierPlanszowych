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
    /// Interaction logic for EditPlayedGames.xaml
    /// </summary>
    public partial class EditPlayedGames : Window
    {
        DBConnection db = new DBConnection();
        private string playedId;
        public EditPlayedGames(string id)
        {
            InitializeComponent();
            playedId =id;
            RefreshLabels();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }
        private void RefreshLabels()
        {
            List<string> QueryList = new List<string>();
            QueryList.Add(db.DatabaseDataGetOne("SELECT date FROM played_games WHERE id_played_games= " + playedId));
            QueryList.Add(db.DatabaseDataGetOne("SELECT players FROM played_games WHERE id_played_games= " + playedId));
            QueryList.Add(db.DatabaseDataGetOne("SELECT winner FROM played_games WHERE id_played_games= " + playedId));
            QueryList.Add(db.DatabaseDataGetOne("SELECT note FROM played_games WHERE id_played_games= " + playedId));

            try
            {
                label_Id.Text = playedId;
                label_Date.Text = QueryList[0];
                label_Players.Text = QueryList[1];
                label_Winner.Text = QueryList[2];
                label_Notes.Text = QueryList[3];
            }
            catch
            {
                throw;
            }

        }
    }
}
