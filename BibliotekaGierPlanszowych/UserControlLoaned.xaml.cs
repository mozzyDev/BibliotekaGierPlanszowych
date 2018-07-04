﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        }

        //odswieżanie danych w combobox
        private void GameComboBoxRefresh()
        {
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            GameComboBox.ItemsSource = db.DatabaseDataGetting("board_game", "title", 0);
            GameComboBox.SelectedItem = db.DatabaseDataGetting("board_game", "title", 0)[0];
        }

        private void AddLoaned_btn_Click(object sender, RoutedEventArgs e)
        {   
            //dodawanie danych do bazy
            String Query = "INSERT OR REPLACE INTO pozyczone (person, id_board_game) VALUES ('"
                + this.Loaned_txtbox.Text + "', (SELECT DISTINCT id_board_game FROM board_game WHERE title = '" + GameComboBox.SelectedValue.ToString() + "'))";
            
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabaseDataChange(Query);
            Loaned_txtbox.Clear();

        }

        //uruchamia przycisk po wpisaniu w textbox
        private void Loaned_txtbox_changed(object sender, RoutedEventArgs e)
        {
                TextBox box = sender as TextBox;
                this.AddLoaned_btn.IsEnabled = box.Text.Length > 1;
        }

    }
}
