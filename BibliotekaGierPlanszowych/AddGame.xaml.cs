using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BibliotekaGierPlanszowych
{

    public partial class AddGame : Window
    {
        private DBConnection db = new DBConnection();
        List<int> liczbaGraczy = new List<int>();
        int tryb = 0; //0 - dodawanie nowej gry, 1- edycja gry
        int getGameId = 0;
        int user;

        public AddGame(int userId)
        {
            InitializeComponent();
            CategoryComboboxRefresh();
            UstalenieWartosciMinMax();
            user = userId;
        }

        public AddGame(string imageUrl, int gameId, int userId) //w trybie edycji pobieram zdjecie
        {
            tryb = 1;
            getGameId = gameId;
            InitializeComponent();
            CategoryComboboxRefresh();
            UstalenieWartosciMinMax();

            if (!String.IsNullOrEmpty(imageUrl))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmap.EndInit();

                GameImage.Source = bitmap;
                user = userId;
            }
        }


        //przesuwanie menu
        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditCategory_btn_Click(object sender, RoutedEventArgs e)
        {
            Categories categories = new Categories();
            categories.ShowDialog();
            CategoryComboboxRefresh();
        }

        //odświeżanie combobox z kategoriami
        public void CategoryComboboxRefresh()
        {
            try
            {
                Category_combobox.ItemsSource = db.DatabaseDataGetting("category", "title_category", 0);
            }
            catch (ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
            catch (NullReferenceException exn)
            {
                Console.WriteLine(exn.Message);
                MessageBox.Show(exn.Message);
            }
        }

        public string GetImageUrl(int gameId)
        {
            string imageUrl = "";
            try
            {
                imageUrl = db.DatabaseDataGetOne("select image_url from board_game where id_board_game = " + gameId.ToString());
            }
            catch (ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
            catch (NullReferenceException exn)
            {
                Console.WriteLine(exn.Message);
                MessageBox.Show(exn.Message);
            }
            return imageUrl;
        }

        //odświeżanie combobox z liczbą graczy do wyboru
        private void MinLiczba_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //dynamiczna zmiana max liczby graczy
            if (MinLiczba_combo.SelectedValue != null)
            {
                liczbaGraczy.Clear();
                int i = (int)MinLiczba_combo.SelectedValue;
                for (int j = i; j < 11; j++)
                {

                    liczbaGraczy.Add(j);
                }
                MaxLiczba_combo.ItemsSource = liczbaGraczy;

            }

            //sprawdzanie czy min nie jest większe niż max
            try
            {
                if (MaxLiczba_combo.IsEnabled && MaxLiczba_combo.SelectedValue != null)
                {
                    if ((int)MaxLiczba_combo.SelectedValue < (int)MinLiczba_combo.SelectedValue)
                    {
                        MessageBox.Show("Maksymalna liczba graczy nie może być mniejsza niż minimalna", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                        MaxLiczba_combo.SelectedValue = MinLiczba_combo.SelectedValue;
                    }
                }
            }
            catch (FormatException exf)
            {
                Console.WriteLine(exf.Message);
                MessageBox.Show(exf.Message);
            }

            MaxLiczba_combo.IsEnabled = true;

        }

        //dodanie wartości dla liczby graczy
        private void UstalenieWartosciMinMax()
        {
            for (int i = 1; i < 10; i++)
            {
                liczbaGraczy.Add(i);
            }

            MinLiczba_combo.ItemsSource = liczbaGraczy;
            MaxLiczba_combo.ItemsSource = liczbaGraczy;
        }

        //dodawanie nowej gry
        private void GameAdd_btn_Click(object sender, RoutedEventArgs e)
        {
            if (tryb == 0)
            {
                List<String> listaTytulow = new List<string>();
                String zapytanieTytuly = "SELECT title FROM board_game";
                DateTime today = DateTime.Today;

                if (Title_txtbox.Text.Length > 1 && MaxLiczba_combo.SelectedValue != null && !String.IsNullOrEmpty(Age_txtbox.Text) &&
                    !String.IsNullOrEmpty(Year_txtbox.Text) && !String.IsNullOrEmpty(Time_txtbox.Text))
                {
                    //sprawdzanie czy w bazie nie ma już gry o tej nazwie
                    //pobranie listy wszyskich tytułów
                    listaTytulow = db.DatabasQueryExecute(zapytanieTytuly);
                    bool niepoprawnyTytul = false;

                    //sprawdzenie czy nowy tytul znajduje sie juz w bazie
                    foreach (String item in listaTytulow)
                    {
                        if (item.Equals(Title_txtbox.Text))
                        {
                            niepoprawnyTytul = true;
                        }
                    }
                    //jesli nowy tytul znajduje sie w bazie - usuwamy poprzedni rekord
                    if (niepoprawnyTytul)
                    {
                        db.DatabasQueryExecute("DELETE FROM board_game WHERE title = '" + this.Title_txtbox.Text + "'");
                    }
                    //dodaje nowy rekord

                    String Query = "INSERT OR REPLACE INTO board_game (title, min_players, max_players, rate, id_category, add_date, yearpublished, playingtime, age, id_user) VALUES ('"
                        + this.Title_txtbox.Text + "', " + MinLiczba_combo.SelectedValue.ToString() + ", " + MaxLiczba_combo.SelectedValue.ToString() + ", "
                        + Rate_slider.Value.ToString() + ", " +
                        "(SELECT DISTINCT id_category FROM category WHERE title_category = '" + Category_combobox.SelectedValue.ToString() + "'), '" + today.ToShortDateString() +
                        "', " + Convert.ToInt32(Year_txtbox.Text) + ", " + Convert.ToInt32(Time_txtbox.Text) + "," + Convert.ToInt32(Age_txtbox.Text) + ", " + user + ")";

                    db.DatabaseDataChange(Query);

                    this.Close();

                }
                else
                {
                    MessageBox.Show("Nie wypełniono wszystkich pól!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                if (Title_txtbox.Text.Length > 1 && MaxLiczba_combo.SelectedValue != null && !String.IsNullOrEmpty(Age_txtbox.Text) &&
                   !String.IsNullOrEmpty(Year_txtbox.Text) && !String.IsNullOrEmpty(Time_txtbox.Text))
                {
                    String Query = "UPDATE board_game SET "
                        + " title = '" + this.Title_txtbox.Text + "'"
                        + ", min_players = " + MinLiczba_combo.SelectedValue.ToString()
                        + ", max_players = " + MaxLiczba_combo.SelectedValue.ToString()
                        + ", rate = " + Rate_slider.Value.ToString()
                        + ", id_category = (SELECT DISTINCT id_category FROM category WHERE title_category = '" + Category_combobox.SelectedValue.ToString() + "')"
                        + ", yearpublished = " + Convert.ToInt32(Year_txtbox.Text)
                        + ", playingtime = " + Convert.ToInt32(Time_txtbox.Text)
                        + ", age = " + Convert.ToInt32(Age_txtbox.Text)
                        + " where id_board_game = " + getGameId.ToString();

                    try
                    {
                        db.DatabaseDataChange(Query);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nie wypełniono wszystkich pól!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }

        }
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Sprawdzenie, czy znak jest cyfrą
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            // Regex do sprawdzania, czy tekst zawiera tylko cyfry
            Regex regex = new Regex("[^0-9]+"); // pozwala tylko na cyfry
            return !regex.IsMatch(text);
        }

        private void LastPlayed_btn_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;

            string queryLastPlayed = @"
                UPDATE board_game
                    set lastPlayed = '"
                + today.ToShortDateString()
                + "' where id_board_game = " + getGameId.ToString();

            MessageBox.Show("Zaktualizowano datę ostatniej gry");
            try
            {
                db.DatabaseDataChange(queryLastPlayed);
            }
            catch (Exception)
            {
                throw;
            }
            this.Close();
        }
    }
}
