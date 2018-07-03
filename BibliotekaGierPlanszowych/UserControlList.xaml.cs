using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Logika interakcji dla klasy UserControlList.xaml
    /// </summary>
    public partial class UserControlList : UserControl
    {
        DBConnection db = new DBConnection();

        public UserControlList()
        {
            InitializeComponent();
            
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // create a new SQL command:
            db.sqlite_cmd = db.sqlite_conn.CreateCommand();

            // Let the SQLiteCommand object know our SQL-Query:
            db.sqlite_cmd.CommandText = "CREATE TABLE test (id integer primary key, text varchar(100));";

            // Now lets execute the SQL ;D
            db.sqlite_cmd.ExecuteNonQuery();

            // Lets insert something into our new table:
            db.sqlite_cmd.CommandText = "INSERT INTO test (id, text) VALUES (1, 'Test Text 1');";

            // And execute this again ;D
            db.sqlite_cmd.ExecuteNonQuery();

            // ...and inserting another line:
            db.sqlite_cmd.CommandText = "INSERT INTO test (id, text) VALUES (2, 'Test Text 2');";

            // And execute this again ;D
            db.sqlite_cmd.ExecuteNonQuery();

            // But how do we read something out of our table ?
            // First lets build a SQL-Query again:
            db.sqlite_cmd.CommandText = "SELECT * FROM test";

            // Now the SQLiteCommand object can give us a DataReader-Object:
            db.sqlite_datareader = db.sqlite_cmd.ExecuteReader();

            // The SQLiteDataReader allows us to run through the result lines:
            while (db.sqlite_datareader.Read()) // Read() returns true if there is still a result line to read
            {
                // Print out the content of the text field:
                System.Console.WriteLine(db.sqlite_datareader["text"]);
            }

            // We are ready, now lets cleanup and close our connection:
            db.sqlite_conn.Close();
        }
    }
}
