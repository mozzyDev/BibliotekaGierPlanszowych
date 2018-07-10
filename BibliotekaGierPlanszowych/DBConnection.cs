using System;
using System.Collections.Generic;
using Finisar.SQLite;
using System.Data;
using System.Windows.Controls;


namespace BibliotekaGierPlanszowych
{
    //Klasa odpowiedzialna za tworzenie i wykonywanie operacji na bazie danych
   public class DBConnection : IDisposable
    {
        
        private static String DatabaseConnectionValue = "Data Source = database.db; Version = 3;Compress = True";
        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
        protected String Query { get; set; }
        protected SQLiteCommand sqlite_cmd;
        protected SQLiteDataReader sqlite_datareader;

       
        //pobieranie danych z bazy, dla wielu danych
        //zwraca listę wyników string
        public List<String> DatabaseDataGetting(String table, String column, int columnNr)
        {
            List<String> list = new List<String>();
            try
            {
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
            }
            catch (SQLiteException exs)
            {
                Console.WriteLine(exs.Message);
            }
            finally
            {
                sqlite_conn.Close();
            }
            list.Sort();
            return list;
        }

        //pobieranie dla combobox - panel: Pożyczone
        public List<String> DatabasQueryExecute(String Query)
        {
            List<String> list = new List<String>();
            try
            {
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
            }
            catch (SQLiteException exs)
            {
                Console.WriteLine(exs.Message);
            }
            finally
            {
                sqlite_conn.Close();
            }
            list.Sort();
            return list;
        }
       

        //wykonywanie zapytań na danych w bazie danych
        public void DatabaseDataChange(String Query)
        {
            try
            {
                sqlite_conn.Open();
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = Query;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (SQLiteException exs)
            {
                Console.WriteLine(exs.Message);
            }
            finally
            {
                sqlite_conn.Close();
            }
        }


        //wykonywanie zapytań na danych w bazie danych dla jednego zwracanego elementu
        public String DatabaseDataGetOne(String Query)
        {
            String item = "";
            try
            {
                sqlite_conn.Open();
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = Query;
                sqlite_cmd.ExecuteNonQuery();
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    item = sqlite_datareader.GetString(0);
                }
            }
            catch (SQLiteException exs)
            {
                Console.WriteLine(exs.Message);
            }
            finally
            {
                sqlite_conn.Close();
            }
            return item;
        }

        //uzupełnianie danych w DataGrid
        public void DataGridRefresh(String Query, String TableName, DataGrid NameOfDataGrid)
        {
            try
            {
                sqlite_conn.Open();
                SQLiteCommand DataGridCommand = new SQLiteCommand(Query, sqlite_conn);
                DataGridCommand.ExecuteNonQuery();
                SQLiteDataAdapter adp = new SQLiteDataAdapter(DataGridCommand);

                DataTable dt = new DataTable(TableName);
                adp.Fill(dt);
                NameOfDataGrid.ItemsSource = dt.DefaultView;
                adp.Update(dt);
            }
            catch (SQLiteException exs)
            {
                Console.WriteLine(exs.Message);
            }
            finally
            {
                sqlite_conn.Close();
            }
        }

        public void Dispose()
        {
            Dispose();
        }
    }
}
