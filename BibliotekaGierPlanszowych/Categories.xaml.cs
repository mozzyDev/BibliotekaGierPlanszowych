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

namespace BibliotekaGierPlanszowych
{
   
    public partial class Categories : Window
    {
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
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabaseDataChange(Query);
            MessageBox.Show("Usunięto kategorię z bazy danych", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            CategoryListRefresh();
        }
        
        //dodawanie kategorii
        private void Button_CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            String Query = "INSERT OR REPLACE INTO category (title_category) VALUES ('" + this.titleCategory_txtbox.Text + "')";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabaseDataChange(Query);
            titleCategory_txtbox.Text = null;
            MessageBox.Show("Zapisano kategorię w bazie danych", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information );
            CategoryListRefresh();
        }

        //odświeżenie listBoxa
        public void CategoryListRefresh()
        {
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            CategoryListBox.ItemsSource = db.DatabaseDataGetting("category", "title_category", 0);
        }

        //uruchamiania przycisku, gdy wpisana jest nazwa kategorii
        private void titleCategory_txtbox_TextChanged(object sender, TextChangedEventArgs e)
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
