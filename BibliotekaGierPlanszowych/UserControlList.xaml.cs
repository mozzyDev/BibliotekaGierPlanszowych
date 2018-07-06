﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace BibliotekaGierPlanszowych
{
    
    public partial class UserControlList : UserControl
    {
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        private string pobranyTytul = "";

        public UserControlList()
        {
            InitializeComponent();
            GridRefresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddGame addGame = new AddGame();
            addGame.ShowDialog();
            GridRefresh();

        }

        //odświeżenie danych w GridData
        private void GridRefresh()
        {
            string Query = "SELECT DISTINCT board_game.title AS 'Tytuł', category.title_category AS 'Kategoria', board_game.min_players AS 'Min Graczy'," +
            "board_game.max_players AS 'Max Graczy' FROM board_game, category WHERE category.id_category = board_game.id_category";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DataGridRefresh(Query, "board_game", List_DataGrid);
        }

        //usuwanie wartosci z DataGrid
        //tytuł jest przekazywany za pomocą Loaned_dataGrid_SelectionChanged()
        //następnie usuwany z bazy za pomocą kodu SQL
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć grę?", "Usuwanie", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                DBConnectionForExistingDB db = new DBConnectionForExistingDB();
             
                //pobranie id gry
                string id = db.DatabaseDataGetOne("SELECT id_board_game FROM board_game WHERE title = '" + pobranyTytul + "'");

                //usunięcie z listy gier
                string Query = "DELETE FROM board_game WHERE title = '" + pobranyTytul + "'";
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
                pobranyTytul = selectedItem[0].ToString();
                ListDelete_btn.IsEnabled = true;
                ListEdit_btn.IsEnabled = true;
            }
        }

        //edycja - otwarcie panelu AddGame i wypełnienie go danymi zaznaczonej gry
        private void ListEdit_btn_Click(object sender, RoutedEventArgs e)
        {
            
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            AddGame addGame = new AddGame();
            List<string> QueryList = new List<string>();
            QueryList.Add(pobranyTytul);
            QueryList.Add(db.DatabaseDataGetOne("SELECT category.title_category FROM board_game, category WHERE category.id_category = board_game.id_category AND board_game.title = '"
                + pobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT min_players FROM board_game WHERE title = '" + pobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT max_players FROM board_game WHERE title = '" + pobranyTytul + "'"));
            QueryList.Add(db.DatabaseDataGetOne("SELECT rate FROM board_game WHERE title = '" + pobranyTytul + "'"));

            try
            {
                addGame.Title_txtbox.Text = QueryList[0];
                addGame.Category_combobox.SelectedValue = QueryList[1];
                addGame.MinLiczba_combo.SelectedValue = Convert.ToInt32(QueryList[2]);
                addGame.MaxLiczba_combo.SelectedValue = Convert.ToInt32(QueryList[3]);
                addGame.Rate_slider.Value = Convert.ToInt32(QueryList[4]);
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

        
    }
}
