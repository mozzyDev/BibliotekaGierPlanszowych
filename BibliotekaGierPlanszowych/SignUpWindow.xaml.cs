using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;

namespace BibliotekaGierPlanszowych
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
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
            List<String> loginList = new List<string>();
            loginList = db.DatabasQueryExecute(queryLogin);
            bool logIn = true;
            bool wrongPass = true;
            bool noLogin = true;

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
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
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
