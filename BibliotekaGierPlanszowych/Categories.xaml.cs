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
        }

        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            String AddQuery = "INSERT INTO category (title_category) VALUES ('" + this.titleCategory_txtbox.Text + "')";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.CategoryAdd(AddQuery);
            MessageBox.Show("Zapisano kategorię w bazie danych", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information );
        }
    }
}
