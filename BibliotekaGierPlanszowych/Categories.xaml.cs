using System;
using System.Windows;
using System.Windows.Controls;

namespace BibliotekaGierPlanszowych
{
   
    public partial class Categories : Window
    {
        private DBConnection db = new DBConnection();
        public Categories()
        {
            InitializeComponent();
            CategoryListRefresh();
        }

        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //usuwanie kategorii
        private void ButtonDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            String Query = "DELETE FROM category WHERE title_category = '" + this.CategoryListBox.SelectedValue.ToString() + "'";
            db.DatabaseDataChange(Query);
            MessageBox.Show("Usunięto kategorię z bazy danych", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            CategoryListRefresh();
        }
        
        //dodawanie kategorii
        private void Button_CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            String Query = "INSERT OR REPLACE INTO category (title_category) VALUES ('" + this.titleCategory_txtbox.Text + "')";
            db.DatabaseDataChange(Query);
            titleCategory_txtbox.Text = null;
            MessageBox.Show("Zapisano kategorię w bazie danych", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information );
            CategoryListRefresh();
        }

        //odświeżenie listBoxa
        public void CategoryListRefresh()
        {
            try
            {
                CategoryListBox.ItemsSource = db.DatabaseDataGetting("category", "title_category", 0);
            }
            catch(ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
        }

        //uruchamiania przycisku, gdy wpisana jest nazwa kategorii
        private void TitleCategory_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;

            this.CategoryAdd_btn.IsEnabled = box.Text.Length > 1;
        }

        private void CategoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CategoryDelete_btn.IsEnabled = CategoryListBox.SelectedValue != null;
        }
    }
}
