using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Data;

namespace BibliotekaGierPlanszowych
{
    public partial class UserControlWishlist : UserControl, IDisposable
    {
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        private String PobranyTytul { get; set; }
        private DBConnection db = new DBConnection();

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
        //dodawanie danych do bazy
        private void WishAdd_btn_Click(object sender, RoutedEventArgs e)
        {
            String Query = "INSERT OR REPLACE INTO wishlist (title_wishlist, price_wishlist) VALUES ('"+ this.WishTitle_txtbox.Text +"', '"
                + this.WishPrice_txtbox.Text +"')";

            db.DatabaseDataChange(Query);
            WishTitle_txtbox.Clear();
            WishPrice_txtbox.Clear();
            GridRefresh();
        }

        //walidacja - tylko cyfry
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
            catch(ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
        }

        //uzupełnianie danych w GridData
        private void GridRefresh()
        {
            string Query = "SELECT wishlist.title_wishlist AS 'Gra', wishlist.price_wishlist || 'zł' AS 'Cena' FROM wishlist";
            db.DataGridRefresh(Query, "wishlist", Wishlist_DataGrid);
        }

        //usuwanie wartosci z DataGrid
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string Query = "DELETE FROM wishlist WHERE title_wishlist = '" + PobranyTytul +"'";

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
                PobranyTytul = selectedItem[0].ToString();
                WishDelete_btn.IsEnabled = true;
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
