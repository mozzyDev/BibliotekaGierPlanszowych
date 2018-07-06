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

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Logika interakcji dla klasy UserControlStats.xaml
    /// </summary>
    public partial class UserControlStats : UserControl
    {
        private List<string> Query = new List<string>();
        private List<string> Stat = new List<string>();
        
        private DBConnectionForExistingDB db = new DBConnectionForExistingDB();

        public UserControlStats()
        {
            InitializeComponent();
            StatRefresh();
        }

        //wypełnianie statystyk danymi z bazy danych
        private void StatRefresh()
        {
            Query.Add("SELECT DISTINCT COUNT(title) FROM board_game");
            Query.Add("SELECT DISTINCT COUNT(id_board_game) FROM pozyczone");
            Query.Add("SELECT DISTINCT COUNT(id_category) FROM category");
            Query.Add("SELECT DISTINCT COUNT(id_wishlist) FROM wishlist");
            Query.Add("SELECT DISTINCT ROUND(AVG(rate), 1) FROM board_game");
            Query.Add("SELECT DISTINCT title FROM board_game ORDER BY id_board_game DESC LIMIT 1");

            for (int i = 0; i < 6; i++)
            {
                Stat.Add(db.DatabaseDataGetOne(Query[i]));
            }

            LiczbaGier_label.Content = Stat[0];
            Pozyczonych_label.Content = Stat[1];
            Kategorii_label.Content = Stat[2];
            Zyczen_label.Content = Stat[3];
            Srednia_label.Content = Stat[4];
            Ostatnia_label.Content = Stat[5];


        }
    }
}
