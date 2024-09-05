using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace BibliotekaGierPlanszowych
{
    public partial class UserControlRandom : UserControl
    {
        private DBConnection db = new DBConnection();
        private List<int> liczbaGraczy = new List<int>();
        private List<string> queryResult = new List<string>();
        private int user;
        public UserControlRandom(int userID)
        {
            user = userID;
            InitializeComponent();
            WypelnienieDanymi();
        }

        //wypelnianie combobox danymi
        private void WypelnienieDanymi()
        {
            for (int i = 1; i < 11; i++)
            {
                liczbaGraczy.Add(i);
            }

            RndPlayers_combo.ItemsSource = liczbaGraczy;
            RndRate_combo.ItemsSource = liczbaGraczy;

            RndCategory_combo.ItemsSource = db.DatabaseDataGetting("category", "title_category", 0);
        }

        //wybieranie losowej gry na podstawie wybranych wartosci
        //losowanie na poziomie SQL
        private void Rnd_Button_Click(object sender, RoutedEventArgs e)
        {
            int minPlayers = (int)RndPlayers_combo.SelectedValue;
            int rate = (int)RndRate_combo.SelectedValue;
            String category = RndCategory_combo.SelectedValue.ToString();

            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("SELECT title FROM board_game WHERE id_user =  ");
            sbQuery.Append(user);
            sbQuery.Append(" and (loaned = 0 or loaned is null)");
            if(Checkbox_liczbaGraczy.IsChecked == true) sbQuery.Append(" and min_players <= " + minPlayers + " AND max_players >= " + minPlayers);
            if(Checkbox_Kategoria.IsChecked == true) sbQuery.Append(" AND id_category = (SELECT id_category FROM category WHERE title_category = '" + category + "')");
            if(Checkbox_Ocena.IsChecked == true) sbQuery.Append(" AND rate >= " + rate);
            sbQuery.Append(" ORDER BY RANDOM() LIMIT 1");


            String Query = sbQuery.ToString();

            try
            {
                queryResult = db.DatabasQueryExecute(Query);
                if (queryResult.Count > 0)
                {
                    Rnd_txtbox.Text = queryResult[0];
                }
                else
                {
                    MessageBox.Show("Nie odnaleziono gry spełniającej podane kryteria", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("Nie odnaleziono gry spełniającej podane kryteria", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                Rnd_txtbox.Text = "";
            }
        }

        //czyszczenie danych w panelu
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RndPlayers_combo.SelectedIndex = 0;
            RndRate_combo.SelectedIndex = 0;
            RndCategory_combo.SelectedIndex = 0;
            Rnd_txtbox.Text = "";
        }

        private void Checkbox_liczbaGraczy_Checked(object sender, RoutedEventArgs e)
        {
            if (RndPlayers_combo != null)
            {
                RndPlayers_combo.IsEnabled = true;
                Label_liczbaGraczy.Foreground = Brushes.Black;
            }
        }

        private void Checkbox_liczbaGraczy_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RndPlayers_combo != null)
            {
                RndPlayers_combo.IsEnabled = false;
                Label_liczbaGraczy.Foreground = Brushes.LightGray;
            }
        }

        private void Checkbox_kategoria_Checked(object sender, RoutedEventArgs e)
        {
            if (RndCategory_combo != null)
            {
                RndCategory_combo.IsEnabled = true;
                Label_Kategoria.Foreground = Brushes.Black;
            }
        }

        private void Checkbox_kategoria_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RndCategory_combo != null)
            {
                RndCategory_combo.IsEnabled = false;
                Label_Kategoria.Foreground = Brushes.LightGray;
            }
        }

        private void Checkbox_ocena_Checked(object sender, RoutedEventArgs e)
        {
            if (RndRate_combo != null)
            {
                RndRate_combo.IsEnabled = true;
                Label_Ocena.Foreground = Brushes.Black;
            }
        }

        private void Checkbox_ocena_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RndRate_combo != null)
            {
                RndRate_combo.IsEnabled = false;
                Label_Ocena.Foreground = Brushes.LightGray;
            }
        }
    }
}
