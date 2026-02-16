using System;
using System.Collections.Generic;
using Finisar.SQLite;
using System.Data;
using System.Windows.Controls;
using System.Globalization;

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
            catch (Exception exs)
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
        // Pomocnicza metoda do konwersji kolumn dat na DateTime
        private void ConvertDateColumnsToDateTime(DataTable dt)
        {
            string[] dateColumnNames = { "DataDodania", "OstatnioGrana" };
            string[] dateFormats = { "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };

            foreach (string columnName in dateColumnNames)
            {
                if (dt.Columns.Contains(columnName))
                {
                    DataColumn oldColumn = dt.Columns[columnName];

                    // Tworzenie nowej kolumny typu DateTime
                    DataColumn newColumn = new DataColumn(columnName + "_temp", typeof(DateTime));
                    newColumn.AllowDBNull = true;
                    dt.Columns.Add(newColumn);

                    // Konwersja wartości z string na DateTime
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row[columnName] != DBNull.Value && row[columnName] != null)
                        {
                            string dateString = row[columnName].ToString();
                            if (!string.IsNullOrWhiteSpace(dateString))
                            {
                                DateTime parsedDate;
                                if (DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                                {
                                    row[newColumn] = parsedDate;
                                }
                                else if (DateTime.TryParse(dateString, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.None, out parsedDate))
                                {
                                    row[newColumn] = parsedDate;
                                }
                                // Jeśli nie uda się sparsować, pozostawiamy DBNull
                            }
                        }
                    }

                    // Usunięcie starej kolumny i zmiana nazwy nowej
                    int columnIndex = oldColumn.Ordinal;
                    dt.Columns.Remove(oldColumn);
                    newColumn.ColumnName = columnName;
                    newColumn.SetOrdinal(columnIndex);
                }
            }
        }
        //uzupełnianie danych w DataGrid
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

                // Konwersja kolumn dat z string na DateTime dla poprawnego sortowania
                ConvertDateColumnsToDateTime(dt);

                NameOfDataGrid.ItemsSource = dt.DefaultView;
                // Usunięto adp.Update(dt); - nie jest potrzebne przy odczycie danych
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
