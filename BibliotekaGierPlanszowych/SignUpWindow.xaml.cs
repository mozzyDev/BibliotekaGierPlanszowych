using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }
    }
}
