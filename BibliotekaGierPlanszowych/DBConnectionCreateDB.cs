using Finisar.SQLite;
using System;
using System.Data;
using System.IO;

namespace BibliotekaGierPlanszowych 
{
    public class DBConnectionCreateDB : DBConnection
    {
        //plik .txt z kodem SQL - utworzenie nowej bazy danych
        private string SqlCommandCreateDatabase = File.ReadAllText(@"createTableCode.txt");
        private static String DatabaseConnectionValue = "Data Source = database.db; New = True; Version = 3;Compress = True";
        private SQLiteConnection sqlite_conn = new SQLiteConnection(DatabaseConnectionValue);
       
        //utworzenie bazy danych, jeśli nie istnieje
        public void DatabaseCreate()
        {
            sqlite_conn.Open();
            try
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = SqlCommandCreateDatabase;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ReadOnlyException exReadOnly)
            {
                Console.WriteLine(exReadOnly.Message);
            }
            catch (SQLiteException exSql)
            {
                Console.WriteLine(exSql.Message);
            }
            catch (ArgumentException exSql)
            {
                Console.WriteLine(exSql.Message);
            }
            finally
            {
                sqlite_conn.Close();
            }
        }

    }
}
