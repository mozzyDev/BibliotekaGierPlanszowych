using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace BibliotekaGierPlanszowych
{

    public partial class MainWindow : Window
    {
        private DBConnectionCreateDB dbCreation = new DBConnectionCreateDB();

        public MainWindow()
        {

            InitializeComponent();

            //utworzenie nowej bazy danych przy starcie programu
            if (!File.Exists("database.db"))
            {
                dbCreation.DatabaseCreate();
            }

            MainGrid.Children.Add(new UserControlList());
            
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
            MessageBox.Show("Biblioteka Gier Planszowych\nTomasz Mozgwa\n2018", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //minimalizacja
        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //przesuwanie okna aplikacji za pomoca kliknięcia w dowolnym miejscu okna
        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
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
                    MainGrid.Children.Add(new UserControlList());
                    break;
                case 1:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlRandom());
                    break;
                case 2:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlLoaned());
                    break;
                case 3:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlWishlist());
                    break;
                case 4:
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new UserControlStats());
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
