using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finisar.SQLite;


namespace BibliotekaGierPlanszowych
{
    class DBConnection : IDisposable
    {

        
        public static String DatabaseConnectionValue = "Data Source = database.db; Version = 3; New = True; Compress = True";

        //plik txt z kodem SQL, tworzącym bazę danych
        public  string SqlCommandCreateDatabase = File.ReadAllText(@"createTableCode.txt");

        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
        private SQLiteCommand sqlite_cmd;
        

        public DBConnection()
        {
        }


        //Utworzenie bazy danych, tabel i podstawowych rekordów do tabeli category
        public void DatabaseCreate()
        {
            sqlite_conn.Open();
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = SqlCommandCreateDatabase;
            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
               
            }
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
