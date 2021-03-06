﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace BibliotekaGierPlanszowych
{
    public partial class UserControlRandom : UserControl
    {
        private DBConnection db = new DBConnection();
        private List<int> liczbaGraczy = new List<int>();

        public UserControlRandom()
        {
            InitializeComponent();
            WypelnienieDanymi();
        }

        //wypelnianie combobox danymi
        private void WypelnienieDanymi()
        {
            for(int i = 1; i < 10; i++)
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
            

            String Query = "SELECT title FROM board_game " +
                "WHERE min_players <= " + minPlayers + "AND max_players >= " + minPlayers +" AND rate >= " + rate + " AND id_category = (SELECT id_category FROM category " +
                "WHERE title_category = '" + category +"') ORDER BY RANDOM() LIMIT 1";

            try
            {
                category = Rnd_txtbox.Text = db.DatabasQueryExecute(Query)[0];
                Rnd_txtbox.Text = category;
            }
            catch(IndexOutOfRangeException ex)
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
    }
}
