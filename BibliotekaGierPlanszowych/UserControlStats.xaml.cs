using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BibliotekaGierPlanszowych
{
    public partial class UserControlStats : UserControl
    {
        private List<string> Query = new List<string>();
        private List<string> Stat = new List<string>();
        string top1_concat, top2_concat, top3_concat = "";
        private int user;
        
        private DBConnection db = new DBConnection();

        public UserControlStats(int userID)
        {
            user = userID;
            InitializeComponent();
            StatRefresh();
        }

        //wypełnianie statystyk danymi z bazy danych
        private void StatRefresh()
        {
            Query.Add("SELECT DISTINCT COUNT(title) FROM board_game where id_user = "+user);
            Query.Add("SELECT DISTINCT COUNT(id_board_game) FROM loaned where id_user = " + user);
            Query.Add("SELECT DISTINCT COUNT(id_category) FROM category");
            Query.Add("SELECT DISTINCT COUNT(id_wishlist) FROM wishlist where id_user = " + user);
            Query.Add("SELECT DISTINCT ROUND(AVG(rate), 1) FROM board_game where id_user = " + user);
            Query.Add("SELECT DISTINCT title FROM board_game where id_user = " + user + " ORDER BY id_board_game DESC LIMIT 1");
            Query.Add("select winner, count(winner) as occurrences from played_games group by winner order by occurrences desc limit 1 ; ");
            Query.Add("select winner, count(winner) as occurrences from played_games group by winner order by occurrences desc limit 1 offset 1; ");
            Query.Add("select winner, count(winner) as occurrences from played_games group by winner order by occurrences desc limit 1 offset 2; ");
            Query.Add("select count(winner) as occurrences from played_games group by winner order by occurrences desc limit 1 ; ");
            Query.Add("select count(winner) as occurrences from played_games group by winner order by occurrences desc limit 1 offset 1; ");
            Query.Add("select count(winner) as occurrences from played_games group by winner order by occurrences desc limit 1 offset 2; ");
            try
            {
                for (int i = 0; i < 12; i++)
                {
                    
                    Stat.Add(db.DatabaseDataGetOne(Query[i]));
                }

                LiczbaGier_label.Content = Stat[0];
                Pozyczonych_label.Content = Stat[1];
                Kategorii_label.Content = Stat[2];
                Zyczen_label.Content = Stat[3];

                if (!String.IsNullOrEmpty(Stat[6]) && !String.IsNullOrEmpty(Stat[9])) top1_concat = Stat[6] + ": " +Stat[9];
                if (!String.IsNullOrEmpty(Stat[7]) && !String.IsNullOrEmpty(Stat[10])) top2_concat = Stat[7] + ": " + Stat[10];
                if (!String.IsNullOrEmpty(Stat[8]) && !String.IsNullOrEmpty(Stat[11])) top3_concat = Stat[8] + ": " + Stat[11];
                top1.Content = top1_concat;
                top2.Content = top2_concat;
                top3.Content = top3_concat;


                Ostatnia_label.Content = Stat[5];
            }
            catch(IndexOutOfRangeException exi)
            {
                Console.WriteLine(exi.Message);
                MessageBox.Show(exi.Message);
            }
            catch (ArgumentException exa)
            {
                Console.WriteLine(exa.Message);
                MessageBox.Show(exa.Message);
            }
        }
    }
}
