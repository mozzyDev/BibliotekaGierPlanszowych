using Finisar.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Logika interakcji dla klasy UserControlLoaned.xaml
    /// </summary>
    public partial class UserControlLoaned : UserControl
    {
        //do pobrania nazwy gry z DataGrid
        DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        string pobranyTytul = "";

        public UserControlLoaned()
        {
            InitializeComponent();
            GameComboBoxRefresh();
            GridRefresh();

        }

        //odswieżanie danych w combobox
        private void GameComboBoxRefresh()
        {
            String Query = "SELECT title FROM board_game EXCEPT SELECT title FROM board_Game WHERE id_board_game IN(SELECT id_board_game FROM pozyczone)";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            GameComboBox.ItemsSource = db.DatabasQueryExecute(Query);
        }

        private void AddLoaned_btn_Click(object sender, RoutedEventArgs e)
        {   
            //dodawanie danych do bazy
            String Query = "INSERT OR REPLACE INTO pozyczone (person, id_board_game) VALUES ('"
                + this.Loaned_txtbox.Text + "', (SELECT DISTINCT id_board_game FROM board_game WHERE title = '" + GameComboBox.SelectedValue.ToString() + "'))";
            
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabaseDataChange(Query);
            Loaned_txtbox.Clear();
            GameComboBoxRefresh();
            GridRefresh();

        }

        //uruchamia przycisk po wpisaniu w textbox
        private void Loaned_txtbox_changed(object sender, RoutedEventArgs e)
        {
                TextBox box = sender as TextBox;
                this.AddLoaned_btn.IsEnabled = box.Text.Length > 1;
        }

        //uzupełnianie danych w GridData
        private void GridRefresh()
        {
            string Query = "SELECT pozyczone.person AS 'Osoba', board_game.title AS 'Tytuł' FROM pozyczone, board_game WHERE pozyczone.id_board_game = board_game.id_board_game";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DataGridRefresh(Query, "pozyczone", loaned_dataGrid);
        }

        //pobieranie danych z GridView
        private void Loaned_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            DataGrid dg = (DataGrid)sender;
            DataRowView selectedItem = dg.SelectedItem as DataRowView;
            if(selectedItem != null)
            {
                pobranyTytul = selectedItem[1].ToString();
                LoanedDelete_btn.IsEnabled = true;
            }
            
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string Query = "DELETE FROM pozyczone WHERE pozyczone.id_loaned IN ("
                + "SELECT pozyczone.id_loaned FROM pozyczone, board_game WHERE board_game.title = '" 
                + pobranyTytul +"' AND pozyczone.id_board_game = board_game.id_board_game)";


            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabasQueryExecute(Query);
            GameComboBoxRefresh();
            GridRefresh();
        }




    }
}
