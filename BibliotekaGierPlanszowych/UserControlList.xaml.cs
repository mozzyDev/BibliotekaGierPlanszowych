using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;

namespace BibliotekaGierPlanszowych
{
    public partial class UserControlList : UserControl
    {
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        string idGry { get; set; }
        private int user;
        private DataView dataView; // Do przechowywania DataView dla filtrowania
        private DBConnection db = new DBConnection();

        public UserControlList(int userID)
        {
            user = userID;
            InitializeComponent();

            // Dodanie obsługi zdarzenia AutoGeneratingColumn dla formatowania dat
            List_DataGrid.AutoGeneratingColumn += List_DataGrid_AutoGeneratingColumn;

            GridRefresh();
        }
        // Obsługa formatowania kolumn dat
        private void List_DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "DataDodania" || e.Column.Header.ToString() == "OstatnioGrana")
            {
                DataGridTextColumn textColumn = e.Column as DataGridTextColumn;
                if (textColumn != null)
                {
                    Binding binding = textColumn.Binding as Binding;
                    if (binding != null)
                    {
                        binding.StringFormat = "dd.MM.yyyy";
                    }
                }
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddGame addGame = new AddGame(user);
            addGame.ShowDialog();
            GridRefresh();
        }

        private void ButtonAddBGG_Click(object sender, RoutedEventArgs e)
        {
            AddGameBGG addGameBGG = new AddGameBGG(user);
            addGameBGG.ShowDialog();
            GridRefresh();
        }

        //odświeżenie danych w GridData
        private void GridRefresh()
        {
            string query = @"
            SELECT 
                b.id_board_game as 'ID',
                substr(b.title, 1, 26) AS 'Tytuł',
                SUBSTR(c.title_category, 1, 15) AS 'Kategoria',
                b.min_players AS 'Min_Graczy',
                b.max_players AS 'Max_Graczy',
                b.playingtime||' min.' AS 'CzasGry',
                b.add_date as DataDodania,
                b.lastPlayed as OstatnioGrana,
                b.played_cnt as Rozgrywki
            FROM board_game b
                JOIN category c ON c.id_category = b.id_category
            WHERE b.id_user = " + user + @"
            ORDER BY b.add_date desc;
            ";
            try
            {
                db.DataGridRefresh(query, "board_game", List_DataGrid);

                // Pobranie DataView z DataGrid dla filtrowania
                if (List_DataGrid.ItemsSource != null)
                {
                    dataView = (DataView)List_DataGrid.ItemsSource;
                }
            }
            catch (ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
        }
        // Metoda filtrowania po tytule
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataView != null)
            {
                string searchText = SearchTextBox.Text.Trim();

                if (string.IsNullOrEmpty(searchText))
                {
                    // Jeśli pole jest puste, pokaż wszystkie rekordy
                    dataView.RowFilter = "";
                }
                else
                {
                    // Filtruj po kolumnie "Tytuł" (case-insensitive)
                    dataView.RowFilter = $"Tytuł LIKE '%{searchText.Replace("'", "''")}%'";
                }
            }
        }
        //usuwanie wartosci z DataGrid
        //tytuł jest przekazywany za pomocą Loaned_dataGrid_SelectionChanged()
        //następnie usuwany z bazy za pomocą kodu SQL
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć grę?", "Usuwanie", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
               
                //usunięcie z listy gier
                string Query = "DELETE FROM board_game  WHERE id_board_game = " + idGry;
                
                db.DatabasQueryExecute(Query);
                
                //usunięcie z pożyczonych
                Query = "DELETE FROM pozyczone WHERE id_board_game = " + idGry;
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
                idGry = selectedItem[0].ToString();
                ListDelete_btn.IsEnabled = true;
                ListEdit_btn.IsEnabled = true;
            }
        }

        //edycja - otwarcie panelu AddGame i wypełnienie go danymi zaznaczonej gry
        private void ListEdit_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenGameDetails();
        }

        private void ButtonGetCollection_Click(object sender, RoutedEventArgs e)
        {
            GetCollection getCollection = new GetCollection(user);
            getCollection.ShowDialog();
            GridRefresh();
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz WSZYSTKIE gry?", "Usuwanie", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {

                //usunięcie z listy gier
                string Query = "DELETE FROM board_game  WHERE id_user = " +user;

                db.DatabasQueryExecute(Query);

                //usunięcie z pożyczonych
                Query = "DELETE FROM pozyczone WHERE id_user = " + user;
                db.DatabasQueryExecute(Query);

                GridRefresh();
            }
        }
        // Obsługa podwójnego kliknięcia na grę w DataGrid
        private void List_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Sprawdź czy kliknięto na wiersz (nie na nagłówek lub puste miejsce)
            if (List_DataGrid.SelectedItem != null)
            {
                // Wywołaj metodę otwierającą okno "Pokaż grę"
                OpenGameDetails();
            }
        }

        // Wyodrębniona metoda do otwierania szczegółów gry
        private void OpenGameDetails()
        {
            // Sprawdź czy gra jest zaznaczona
            if (string.IsNullOrEmpty(idGry))
            {
                return;
            }

            DBConnection db = new DBConnection();

            List<string> QueryList = new List<string>();
            QueryList.Add(db.DatabaseDataGetOne("SELECT title FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT category.title_category FROM board_game, category WHERE category.id_category = board_game.id_category AND board_game.id_board_game = '"
                + idGry + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT min_players FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT max_players FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT rate FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT image_url FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT id_board_game FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT playingtime FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT yearpublished FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT age FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT SUBSTR(add_date, 1, 10) FROM board_game WHERE id_board_game = " + idGry));
            QueryList.Add(db.DatabaseDataGetOne("SELECT SUBSTR(lastPlayed, 1, 10) FROM board_game WHERE id_board_game = " + idGry));

            AddGame addGame = new AddGame(QueryList[5].ToString(), Convert.ToInt32(QueryList[6]), user); //tryb edycji z url zdjecia i ref gry

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
            catch (ArgumentException ex)
            {
                MessageBox.Show("Błędne dane", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            addGame.ShowDialog();
            GridRefresh();
        }
    }
}
