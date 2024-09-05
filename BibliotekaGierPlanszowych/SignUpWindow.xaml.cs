using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.IO;

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private DBConnectionCreateDB dbCreation = new DBConnectionCreateDB();
        public SignUpWindow()
        {
            
            //utworzenie nowej bazy danych przy starcie programu
            if (!File.Exists("database.db"))
            {
                dbCreation.DatabaseCreate();
            }

            InitializeComponent();

            //Pobieranie ostatniego wybranego loginu
            List<String> lastUser = new List<string>();
            List<String> lastPass = new List<string>();
            String lastUserQuery = "SELECT login FROM users where lastUsed = 1 LIMIT 1";
            String lastPasswordQuery = "SELECT password FROM users where lastUsed = 1 LIMIT 1";
            lastUser = db.DatabasQueryExecute(lastUserQuery);
            lastPass = db.DatabasQueryExecute(lastPasswordQuery);

            if (lastUser.Count > 0)
            {
                TextBox_Login.Text = lastUser[0];
                TextBox_Pass.Password = lastPass[0];
                TextBox_RepPass.Password = "";
            }
            else
            {
                TextBox_Login.Text = "";
                TextBox_Pass.Password = "";
                TextBox_RepPass.Password = "";
            }
        }

        private DBConnection db = new DBConnection();

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            bool LoginOk = true;
            if(TextBox_Pass.Password != TextBox_RepPass.Password)
            {
                MessageBox.Show("Pola: Hasło i Powtórz hasło muszą być takie same", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginOk = false;
            }
            if (String.IsNullOrEmpty(TextBox_Pass.Password) || String.IsNullOrEmpty(TextBox_RepPass.Password) || String.IsNullOrEmpty(TextBox_Login.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być uzupełnione", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginOk = false;
            }
            //sprawdzamy czy nie mamy juz takiego loginu
            List<String> usersList= new List<string>();
            String queryUsers = "SELECT login FROM users";
            bool niepoprawnyLogin = false;
            usersList = db.DatabasQueryExecute(queryUsers);
          
            foreach (String item in usersList)
            {
                if (item.Equals(this.TextBox_Login.Text))
                {
                    niepoprawnyLogin = true;
                }
            }
            if (niepoprawnyLogin)
            {
                MessageBox.Show("W bazie istnieje już użytkownik o tym loginie", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginOk = false;
            }


            if (LoginOk)
            {
                String Query = "INSERT INTO users (login, password) VALUES ('" + this.TextBox_Login.Text + "', '" + this.TextBox_Pass.Password + "')";

                try
                {
                    db.DatabaseDataChange(Query);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                MessageBox.Show("Zapisano dane logowania", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            //sprawdzamy czy hasło jest poprawne            
            String queryLogin = "SELECT password FROM users where login = " +"'"+this.TextBox_Login.Text+"'";
            String queryUserId = "SELECT id_users FROM users where login = " + "'" + this.TextBox_Login.Text + "' limit 1";
            List<String> loginList = new List<string>();
            List<String> users = new List<string>();
            int userID;
            loginList = db.DatabasQueryExecute(queryLogin);
            bool logIn = true;
            bool wrongPass = true;
            bool noLogin = true;
            string updateQuery = "UPDATE users set lastUsed = 1 where login = "+"'"+TextBox_Login.Text+"'";
            string updateAllQuery = "UPDATE users set lastUsed = 0";

            foreach (String item in loginList)
            {
                noLogin = false;
                if (item.Equals(this.TextBox_Pass.Password))
                {
                    wrongPass = false;
                }
            }
            if (wrongPass && !noLogin)
            {
                MessageBox.Show("Niepoprawne hasło", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                logIn = false;
            }
            if (wrongPass && noLogin)
            {
                MessageBox.Show("Nie znaleziono użytkownika", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
                logIn = false;
            }

            if (logIn)
            {
                //oznaczamy ostatnio zalogowanego użytkownika
                try
                {
                    if (!String.IsNullOrEmpty(TextBox_Login.Text))
                    {
                        db.DatabaseDataChange(updateAllQuery);
                        db.DatabaseDataChange(updateQuery);

                        //otwieramy okno główne
                        users = db.DatabasQueryExecute(queryUserId);
                        userID = Convert.ToInt32(users[0]);
                        MainWindow main = new MainWindow(userID);
                        main.Show();
                        this.Close();
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }


        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool invert = parameter != null && bool.Parse(parameter.ToString());
                return (boolValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter != null && bool.Parse(parameter.ToString());
                return visibility == Visibility.Visible ^ invert;
            }
            return false;
        }
    }
}
