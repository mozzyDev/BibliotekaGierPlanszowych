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
using Finisar.SQLite;


namespace BibliotekaGierPlanszowych
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.Children.Add(new UserControlList());
           
        }



        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

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
        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }
    }
}
