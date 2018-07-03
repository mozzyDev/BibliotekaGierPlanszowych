using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finisar.SQLite;

namespace BibliotekaGierPlanszowych
{
    class DBConnection
    {

        public SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
        public SQLiteCommand sqlite_cmd;
        public SQLiteDataReader sqlite_datareader;

        public DBConnection()
        {
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
        }
      

    }
}
