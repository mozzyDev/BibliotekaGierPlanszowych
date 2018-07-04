using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finisar.SQLite;

namespace BibliotekaGierPlanszowych
{
    class DBConnection
    {
        
        String NewDatabase { get; set; }
        public static String DatabaseConnectionValue = "Data Source = database.db; Version = 3; New = True; Compress = True";

        //plik txt z kodem SQL, tworzącym bazę danych
        public string SqlCommandCreateDatabase = File.ReadAllText(@"createTableCode.txt");

        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
        private SQLiteCommand sqlite_cmd;
        

        //Utworzenie bazy danych
        public void DatabaseCreate()
        {
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = SqlCommandCreateDatabase;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
        }

    }
}
