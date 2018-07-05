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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data;

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Logika interakcji dla klasy UserControlWishlist.xaml
    /// </summary>
    public partial class UserControlWishlist : UserControl
    {
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        private string pobranyTytul = "";

        public UserControlWishlist()
        {
            InitializeComponent();
            GridRefresh();
        }

        //uruchamia przycisk po wpisaniu tytułu
        private void Wish_txtbox_changed(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            this.WishAdd_btn.IsEnabled = box.Text.Length > 1;
        }

        private void WishAdd_btn_Click(object sender, RoutedEventArgs e)
        {
            //dodawanie danych do bazy
            String Query = "INSERT OR REPLACE INTO wishlist (title_wishlist, price_wishlist) VALUES ('"+ this.WishTitle_txtbox.Text +"', '"
                + this.WishPrice_txtbox.Text +"')";

            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabaseDataChange(Query);
            WishTitle_txtbox.Clear();
            WishPrice_txtbox.Clear();
            GridRefresh();
        }

        //walidacja - tylko cyfry
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //uzupełnianie danych w GridData
        private void GridRefresh()
        {
            string Query = "SELECT wishlist.title_wishlist AS 'Gra', wishlist.price_wishlist || 'zł' AS 'Cena' FROM wishlist";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DataGridRefresh(Query, "wishlist", Wishlist_DataGrid);
        }

        //usuwanie wartosci z DataGrid
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string Query = "DELETE FROM wishlist WHERE title_wishlist = '" + pobranyTytul +"'";

            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabasQueryExecute(Query);
            GridRefresh();
        }

        //pobieranie danych z GridView
        private void Wishlist_DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            DataRowView selectedItem = dg.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
                pobranyTytul = selectedItem[0].ToString();
                WishDelete_btn.IsEnabled = true;
            }
        }
    }
}
