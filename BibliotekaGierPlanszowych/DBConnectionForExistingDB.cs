using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finisar.SQLite;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;

namespace BibliotekaGierPlanszowych
{
    class DBConnectionForExistingDB : DBConnection, IDisposable
    {
        private new static String DatabaseConnectionValue = "Data Source = database.db; Version = 3;Compress = True";
        private String Query { get; set; }
        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
        private SQLiteCommand sqlite_cmd;
        private SQLiteDataReader sqlite_datareader;

       

        //pobieranie danych z bazy, dla wielu danych
        //zwraca listę wyników string
        public List<String> DatabaseDataGetting(String table, String column, int columnNr)
        {
            List<String> list = new List<String>();
            sqlite_conn.Open();
            Query = "SELECT DISTINCT " + column + " FROM " + table;
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

        //pobieranie dla combobox - pożyczone
        public List<String> DatabasQueryExecute(String Query)
        {
            List<String> list = new List<String>();
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Query;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string item = sqlite_datareader.GetString(0);
                list.Add(item);
            }
            sqlite_conn.Close();
            list.Sort();
            return list;
        }
       

        //wykonywanie zapytań na danych w bazie danych
        public void DatabaseDataChange(String Query)
        {
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Query;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();

        }


        //wykonywanie zapytań na danych w bazie danych dla jednego zwracanego elementu
        public String DatabaseDataGetOne(String Query)
        {
            String item = "";
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = Query;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                item = sqlite_datareader.GetString(0);
            }
            
            sqlite_conn.Close();
            return item;

        }

        //uzupełnianie danych w DataGrid
        public void DataGridRefresh(String Query, String TableName, DataGrid NameOfDataGrid)
        {

            sqlite_conn.Open();
            SQLiteCommand DataGridCommand = new SQLiteCommand(Query, sqlite_conn);
            DataGridCommand.ExecuteNonQuery();
            SQLiteDataAdapter adp = new SQLiteDataAdapter(DataGridCommand);

            DataTable dt = new DataTable(TableName);
            adp.Fill(dt);
            NameOfDataGrid.ItemsSource = dt.DefaultView;
            adp.Update(dt);
            sqlite_conn.Close();
        }



    }
}
