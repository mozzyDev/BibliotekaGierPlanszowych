using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace BibliotekaGierPlanszowych
{
    public partial class UserControlLoaned : UserControl
    {
        //do pobrania nazwy gry z DataGrid
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        string PobranyTytul { get; set; }
        private DBConnection db = new DBConnection();
        private int user;

        public UserControlLoaned(int userId)
        {
            user = userId;
            InitializeComponent();
            GameComboBoxRefresh();
            GridRefresh();

        }

        //odswieżanie danych w combobox
        private void GameComboBoxRefresh()
        {
            string query = "select distinct b.title from board_game b where b.id_user = " +user + " AND (b.loaned = 0 OR b.loaned IS NULL)";
            
            GameComboBox.ItemsSource = db.DatabasQueryExecute(query);
        }

        //dodawanie danych do bazy
        private void AddLoaned_btn_Click(object sender, RoutedEventArgs e)
        {
            
            string queryLoanInsert = @"INSERT INTO loaned (person, id_board_game, id_user)
                             VALUES ('" + Loaned_txtbox.Text + @"', (SELECT DISTINCT id_board_game FROM board_game WHERE id_user = " + user + " and title = '" + GameComboBox.SelectedValue.ToString() + "'), "+user+")";

            string queryLoanUpdate = "update board_game  set loaned = 1 where id_user = " + user + " and title = '" + GameComboBox.SelectedValue.ToString() + "'";

            db.DatabaseDataChange(queryLoanInsert);
            db.DatabaseDataChange(queryLoanUpdate);
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
            string Query = "SELECT distinct l.person AS 'Osoba', b.title AS 'Tytuł' FROM loaned l join board_game b on (l.id_board_game = b.id_board_game) where l.id_user = " + user;
            try
            {
                db.DataGridRefresh(Query, "pozyczone", loaned_dataGrid);
            }
            catch(ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
}

        //pobieranie danych z GridView
        private void Loaned_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView selectedItem = dg.SelectedItem as DataRowView;
                if (selectedItem != null)
                {
                    PobranyTytul = selectedItem[1].ToString();
                    LoanedDelete_btn.IsEnabled = true;
                }
            }
            catch(FormatException exf)
            {
                Console.WriteLine(exf.Message);
                MessageBox.Show(exf.Message);
            }
        }

        //usuwanie wartosci z DataGrid
        //tytuł jest przekazywany za pomocą Loaned_dataGrid_SelectionChanged()
        //następnie usuwany z bazy za pomocą kodu SQL
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {            
            if (!String.IsNullOrEmpty(PobranyTytul))
            {
                string queryReturnDelete = @"delete from loaned where id_board_game = (select id_board_game from board_game b where b.id_user = " + user + @" and b.title = '" + PobranyTytul + "')";
                string queryReturnUpdate = @"update board_game set loaned = 0 where id_user = " + user + " and title = '" + PobranyTytul + "'";
               
                db.DatabasQueryExecute(queryReturnDelete);
                db.DatabasQueryExecute(queryReturnUpdate);
                GameComboBoxRefresh();
                GridRefresh();
            }
            else
            {
                MessageBox.Show("Wybierz grę do usunięcia");
            }
        }


    }
}
