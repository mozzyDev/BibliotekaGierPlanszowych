using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finisar.SQLite;
using System.Collections;
using System.Collections.ObjectModel;

namespace BibliotekaGierPlanszowych
{
    class DBConnectionForExistingDB : DBConnection
    {
        private new static String DatabaseConnectionValue = "Data Source = database.db; Version = 3;Compress = True";
        private String Query { get; set; }
        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
        private SQLiteCommand sqlite_cmd;
        private SQLiteDataReader sqlite_datareader;

        //dodawanie danych do bazy danych
        public void CategoryAdd(String Query)
        {
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Query;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
            
        }

        //odświeżanie danych w combobox
        public List<String> ComboboxRefresh(String table, int columnNr)
        {
            List<String> list = new List<String>();
            sqlite_conn.Open();
            Query = "SELECT * FROM " + table;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Query;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string item = sqlite_datareader.GetString(columnNr);
                list.Add(item);
            }
            sqlite_conn.Close();
            list.Sort();
            return list;
        }
        

    }
}
