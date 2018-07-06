﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Text.RegularExpressions;


namespace BibliotekaGierPlanszowych
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //utworzenie nowej bazy danych przy starcie programu
            DBConnection db = new DBConnection();
            if (!File.Exists("database.db"))
            {
                db.DatabaseCreate();
            }
            MainGrid.Children.Add(new UserControlList());

        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Czy na pewno chcesz opuścić program?", "Wyjście", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Biblioteka Gier Planszowych\nTomasz Mozgwa\n2018", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
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
