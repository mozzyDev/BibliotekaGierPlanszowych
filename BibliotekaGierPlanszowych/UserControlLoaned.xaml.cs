using Finisar.SQLite;
using System;
using System.Collections.Generic;
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
        public UserControlLoaned()
        {
            InitializeComponent();
            GameComboBoxRefresh();
            GridRefresh();

        }

        //odswieżanie danych w combobox
        private void GameComboBoxRefresh()
        {
            String Query = "SELECT title FROM board_game EXCEPT SELECT title FROM board_Game WHERE id_category IN(SELECT id_board_game FROM pozyczone)";
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
            string Query = "SELECT * FROM pozyczone";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DataGridRefresh(Query, "pozyczone", loaned_dataGrid);
        }
        

        
    }
}
