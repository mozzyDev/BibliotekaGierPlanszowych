using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BibliotekaGierPlanszowych
{

    public partial class MainWindow : Window
    {
       
        private int user;


        public MainWindow(int userID)
        {
            user = userID;
            InitializeComponent();

            MainGrid.Children.Add(new UserControlList(user));

        }

        //wyjście z programu
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Czy na pewno chcesz opuścić program?", "Wyjście", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        //o programie
        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUp = new SignUpWindow();
            signUp.Show();
            this.Close();
        }

        //minimalizacja
        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //przesuwanie okna aplikacji za pomoca kliknięcia w dowolnym miejscu okna
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        //wybór paneli z menu bocznego
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlList(user));
                    break;
                case 1:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlRandom(user));
                    break;
                case 2:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlLoaned(user));
                    break;
                case 3:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlWishlist(user));
                    break;
                case 4:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlStats(user));
                    break;
                default:
                    break;
            }
        }

        //zmiana menu po kliknięciu w jedno z pól
        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }
    }
}
