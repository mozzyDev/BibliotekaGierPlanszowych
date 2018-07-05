using System;
using System.Collections.Generic;
using System.Data;
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

namespace BibliotekaGierPlanszowych
{
    
    public partial class UserControlList : UserControl
    {
        private DataColumn gameTitle = new DataColumn("gameTitle", typeof(string));
        private string pobranyTytul = "";

        public UserControlList()
        {
            InitializeComponent();
            GridRefresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddGame addGame = new AddGame();
            addGame.ShowDialog();
            GridRefresh();

        }

        private void GridRefresh()
        {
            string Query = "SELECT DISTINCT board_game.title AS 'Tytuł', category.title_category AS 'Kategoria', board_game.min_players AS 'Min Graczy'," +
            "board_game.max_players AS 'Max Graczy' FROM board_game, category WHERE category.id_category = board_game.id_category";
            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DataGridRefresh(Query, "board_game", List_DataGrid);
        }

        //usuwanie wartosci z DataGrid
        //tytuł jest przekazywany za pomocą Loaned_dataGrid_SelectionChanged()
        //następnie usuwany z bazy za pomocą kodu SQL
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string Query = "DELETE FROM board_game WHERE title = '" + pobranyTytul +"'";

            DBConnectionForExistingDB db = new DBConnectionForExistingDB();
            db.DatabasQueryExecute(Query);
            
            GridRefresh();
        }


        //pobieranie danych z GridView
        private void List_DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dg = (DataGrid)sender;
            DataRowView selectedItem = dg.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
                pobranyTytul = selectedItem[0].ToString();
                ListDelete_btn.IsEnabled = true;
            }
        }
    }
}
