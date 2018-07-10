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

        public UserControlLoaned()
        {
            InitializeComponent();
            GameComboBoxRefresh();
            GridRefresh();

        }

        //odswieżanie danych w combobox
        private void GameComboBoxRefresh()
        {
            String Query = "SELECT DISTINCT title FROM board_game EXCEPT SELECT title FROM board_Game WHERE id_board_game IN(SELECT id_board_game FROM pozyczone)";
            GameComboBox.ItemsSource = db.DatabasQueryExecute(Query);
        }

        //dodawanie danych do bazy
        private void AddLoaned_btn_Click(object sender, RoutedEventArgs e)
        {   
            String Query = "INSERT OR REPLACE INTO pozyczone (person, id_board_game) VALUES ('"
                + this.Loaned_txtbox.Text + "', (SELECT DISTINCT id_board_game FROM board_game WHERE title = '" + GameComboBox.SelectedValue.ToString() + "'))";
            
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
            string Query = "DELETE FROM pozyczone WHERE pozyczone.id_loaned IN ("
                + "SELECT pozyczone.id_loaned FROM pozyczone, board_game WHERE board_game.title = '" 
                + PobranyTytul +"' AND pozyczone.id_board_game = board_game.id_board_game)";

            db.DatabasQueryExecute(Query);
            GameComboBoxRefresh();
            GridRefresh();
        }


    }
}
