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
    /// Interaction logic for PlayedGames.xaml
    /// </summary>
    public partial class PlayedGames : Window
    {
        private int? user;
        private int? board_game;
        private string board_game_title;
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        string idRozgrywki { get; set; }

        DBConnection db = new DBConnection();
        public PlayedGames(int userID, int board_game_id, string board_game_id_title)
        {
            user = userID;
            board_game = board_game_id;
            board_game_title = board_game_id_title;
            if (!String.IsNullOrEmpty(board_game_id_title) && board_game != null && user != null)
            {
                InitializeComponent();
                this.Txt_Title.Content = board_game_title;
                GridRefresh();
            }
            else
            {
                MessageBox.Show("Błąd otwarcia okna PlayedGames");           
            }
            
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GridRefresh()
        {
            string query = @"
            SELECT     
                p.id_played_games as 'Id',
                p.date as 'Data',
                p.winner as 'Zwycięzca'                
            FROM played_games p               
            WHERE p.id_user = " + user + @"
              and p.id_board_game = " + board_game +@"
            ORDER BY p.id_played_games desc;
            ";
            try
            {
                db.DataGridRefresh(query, "played_games", List_PlayedGrid);

            }
            catch (ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
        }
        private void List_PlayedGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dg = (DataGrid)sender;
            DataRowView selectedItem = dg.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
                idRozgrywki = selectedItem[0].ToString();
                btn_edit.IsEnabled = true;
                btn_delete.IsEnabled = true;
            }
        }
        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Txt_Players.Text) && Txt_Date.SelectedDate != null && !String.IsNullOrEmpty(Txt_Winner.Text))
            {
                string playedQuery = "";
                StringBuilder sb = new StringBuilder();               

                sb.Append("INSERT INTO played_games(id_user, id_board_game, date, players, winner, note) VALUES (");
                sb.Append(user);
                sb.Append(", ");
                sb.Append(board_game);
                sb.Append(", '");
                sb.Append(Txt_Date.SelectedDate.Value.ToShortDateString());
                sb.Append("', '");
                sb.Append(Txt_Players.Text);
                sb.Append("', '");
                sb.Append(Txt_Winner.Text);
                sb.Append("', '");
                sb.Append(Txt_Notes.Text);
                sb.Append("');");

                playedQuery = sb.ToString();

                string queryLastPlayed = @"
                  UPDATE board_game
                   set lastPlayed = '"
                  + Txt_Date.SelectedDate.Value.ToShortDateString()
                  + "' where id_board_game = " + board_game.ToString() + " and id_user = " + user;

                try
                {
                    db.DatabaseDataChange(playedQuery);
                    db.DatabaseDataChange(queryLastPlayed);

                }
                catch (Exception)
                {

                    throw;
                }

                UpdatePlayedCnt(board_game, 1);

                MessageBox.Show("Dodano nową rozgrywkę dla gry: " + board_game_title);
                GridRefresh();
                Txt_Date.SelectedDate = null;
                Txt_Players.Text = "";
                Txt_Winner.Text = "";
                Txt_Notes.Text = "";
            }
            else
            {
                MessageBox.Show("Należy uzupełnić dane");
            }
        }

        private void Edit_btn_Click(object sender, RoutedEventArgs e)
        {
            EditPlayedGames editPlayedGames = new EditPlayedGames(idRozgrywki);

            editPlayedGames.ShowDialog();
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "Usuwanie", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {

                string Query = "DELETE FROM played_games  WHERE id_played_games = " + idRozgrywki;

                db.DatabasQueryExecute(Query);

                UpdatePlayedCnt(board_game, 0);

                GridRefresh();
            }
        }

        private void UpdatePlayedCnt(int? boardGameId, int mode) //mode - 1 dodawanie, 0 - usuwanie
        {
            //aktualizacja ilosci rozegranych gier
            string playedCnt = "0";
            int playedCntInt = 0;
            string updatePlayedQuery = "";
            string getPlayedQuery = "select played_cnt from board_game where id_board_game = " + board_game;
            try
            {
                playedCnt = db.DatabaseDataGetOne(getPlayedQuery);
                if (String.IsNullOrEmpty(playedCnt)) playedCnt = "0";
            }
            catch (Exception)
            {
                throw;
            }
            if (!String.IsNullOrEmpty(playedCnt))
            {
                playedCntInt = Convert.ToInt32(playedCnt);
                if (mode == 1) playedCntInt++;
                if (mode == 0) playedCntInt--;

                if (playedCntInt < 0) playedCntInt = 0;

                playedCnt = playedCntInt.ToString();
                updatePlayedQuery = $"update board_game set played_cnt = {playedCnt} where id_board_game = {board_game}";
            }

            try
            {
                db.DatabaseDataChange(updatePlayedQuery);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
