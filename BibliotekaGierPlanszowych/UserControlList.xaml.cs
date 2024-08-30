using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace BibliotekaGierPlanszowych
{
    public partial class UserControlList : UserControl
    {
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        string PobranyTytul { get; set; }
        private DBConnection db = new DBConnection();
        
        public UserControlList()
        {
            InitializeComponent();
            GridRefresh();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddGame addGame = new AddGame();
            addGame.ShowDialog();
            GridRefresh();
        }

        private void ButtonAddBGG_Click(object sender, RoutedEventArgs e)
        {
            AddGameBGG addGameBGG = new AddGameBGG();
            addGameBGG.ShowDialog();
            GridRefresh();
        }

        //odświeżenie danych w GridData
        private void GridRefresh()
        {
            string query = @"
            SELECT SUBSTR(b.title, 1, 28)  AS 'Tytuł',
                SUBSTR(c.title_category, 1, 18) AS 'Kategoria',
                b.min_players AS 'Min_Graczy',
                b.max_players AS 'Max_Graczy',
                b.playingtime||' min.' AS 'CzasGry',
                b.add_date as DataDodania,
                b.lastPlayed as OstatnioGrana
            FROM board_game b
                JOIN category c ON c.id_category = b.id_category
            ORDER BY b.title;
            ";
            try
            {
                db.DataGridRefresh(query, "board_game", List_DataGrid);

            }
            catch(ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
        }

        //usuwanie wartosci z DataGrid
        //tytuł jest przekazywany za pomocą Loaned_dataGrid_SelectionChanged()
        //następnie usuwany z bazy za pomocą kodu SQL
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć grę?", "Usuwanie", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                //pobranie id gry
                string id = db.DatabaseDataGetOne("SELECT id_board_game FROM board_game WHERE title = '" + PobranyTytul + "'");

                //usunięcie z listy gier
                string Query = "DELETE FROM board_game WHERE title = '" + PobranyTytul + "'";
                db.DatabasQueryExecute(Query);
                
                //usunięcie z pożyczonych
                Query = "DELETE FROM pozyczone WHERE id_board_game = " +id ;
                db.DatabasQueryExecute(Query);

                GridRefresh();
            }
            
        }


        //pobieranie danych z GridView
        private void List_DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dg = (DataGrid)sender;
            DataRowView selectedItem = dg.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
                PobranyTytul = selectedItem[0].ToString();
                ListDelete_btn.IsEnabled = true;
                ListEdit_btn.IsEnabled = true;
            }
        }

        //edycja - otwarcie panelu AddGame i wypełnienie go danymi zaznaczonej gry
        private void ListEdit_btn_Click(object sender, RoutedEventArgs e)
        {
            
            DBConnection db = new DBConnection();
            
            List<string> QueryList = new List<string>();
            QueryList.Add(PobranyTytul);
            QueryList.Add(db.DatabaseDataGetOne("SELECT category.title_category FROM board_game, category WHERE category.id_category = board_game.id_category AND board_game.title = '"
                + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT min_players FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT max_players FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT rate FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT image_url FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT id_board_game FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT playingtime FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT yearpublished FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT age FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT SUBSTR(add_date, 1, 10) FROM board_game WHERE title = '" + PobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT SUBSTR(lastPlayed, 1, 10) FROM board_game WHERE title = '" + PobranyTytul + "'"));

            AddGame addGame = new AddGame(QueryList[5].ToString(), Convert.ToInt32(QueryList[6])); //tryb edycji z url zdjecia i ref gry

            try
            {
                addGame.Title_txtbox.Text = QueryList[0];
                addGame.Category_combobox.SelectedValue = QueryList[1];
                addGame.MinLiczba_combo.SelectedValue = Convert.ToInt32(QueryList[2]);
                addGame.MaxLiczba_combo.SelectedValue = Convert.ToInt32(QueryList[3]);
                addGame.Rate_slider.Value = Convert.ToInt32(QueryList[4]);

                addGame.Time_txtbox.Text = QueryList[7];
                addGame.Year_txtbox.Text = QueryList[8];
                addGame.Age_txtbox.Text = QueryList[9];
                addGame.AddingDate.Content = QueryList[10];
                addGame.PlayedDate.Content = QueryList[11];

            }
            catch(ArgumentException ex)
            {
                MessageBox.Show("Błędne dane", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            addGame.ShowDialog();
            GridRefresh();
        }

        private void ButtonGetCollection_Click(object sender, RoutedEventArgs e)
        {
            string userName = "mozzy_mozzy";
            // Wywołaj metodę wyszukiwania
            APIBGG bgg = new APIBGG();

            List<Item> bggGameCollectionList = bgg.GetBoardGamesForUser(userName);
            if (bggGameCollectionList.Count > 0)
            {
                MessageBox.Show(bggGameCollectionList.Count.ToString());
            }
            else
            {
                MessageBox.Show("Nic nie znaleziono", "Informacja");
            }
        }
    }
}
