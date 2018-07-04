using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finisar.SQLite;

namespace BibliotekaGierPlanszowych
{
    class DBConnectionForExistingDB : DBConnection
    {
        private static String DatabaseConnectionValue = "Data Source = database.db; Version = 3;Compress = True";
        private String AddQuery { get; set; }
        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
        

        //dodawanie danych do bazy danych
        public void CategoryAdd(String AddQuery)
        {
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = AddQuery;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
            
        }

    }
}
