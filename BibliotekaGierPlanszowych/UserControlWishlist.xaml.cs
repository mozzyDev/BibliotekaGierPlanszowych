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

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Logika interakcji dla klasy UserControlWishlist.xaml
    /// </summary>
    public partial class UserControlWishlist : UserControl
    {
        public UserControlWishlist()
        {
            InitializeComponent();
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
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
