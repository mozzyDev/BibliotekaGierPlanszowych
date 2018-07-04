using System;
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
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Logika interakcji dla klasy AddGame.xaml
    /// </summary>
    public partial class AddGame : Window
    {
        List<int> liczbaGraczy = new List<int>();

        public AddGame()
        {
            InitializeComponent();
            CategoryComboboxRefresh();
            UstalenieWartosciMinMax();
            
        }

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

        public void CategoryComboboxRefresh()
        {
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            Category_combobox.ItemsSource = db.DatabaseDataGetting("category", "title_category", 0);
            Category_combobox.SelectedItem = db.DatabaseDataGetting("category", "title_category", 0)[0];

        }

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
            if (MaxLiczba_combo.IsEnabled && MaxLiczba_combo.SelectedValue!= null)
            {
                if((int)MaxLiczba_combo.SelectedValue < (int)MinLiczba_combo.SelectedValue)
                {
                    MessageBox.Show("Maksymalna liczba graczy nie może być mniejsza niż minimalna", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                    MaxLiczba_combo.SelectedValue = MinLiczba_combo.SelectedValue;
                }
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

        private void GameAdd_btn_Click(object sender, RoutedEventArgs e)
        {
            
            DateTime today = DateTime.Today;

            if(Title_txtbox.Text.Length > 1 && MaxLiczba_combo.SelectedValue != null)
            {
                //MessageBox.Show(today.ToString());
                //dodawanie gry do bazy
                String Query = "INSERT OR REPLACE INTO board_game (title, min_players, max_players, rate, id_category, add_date) VALUES ('" 
                    + this.Title_txtbox.Text + "', " + MinLiczba_combo.SelectedValue.ToString() + ", " + MaxLiczba_combo.SelectedValue.ToString() + ", "
                    + Rate_slider.Value.ToString() + ", " +
                    "(SELECT DISTINCT id_category FROM category WHERE title_category = '" + Category_combobox.SelectedValue.ToString() + "'), '" + today.ToString() +"')";


                    DBConnectionForExistingDB db = new DBConnectionForExistingDB();
                    db.DatabaseDataChange(Query);
                    
                    MessageBox.Show("Zapisano grę w bazie danych", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Nie wypełniono wszystkich pól!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
