
using System.Windows;
using System.Windows.Input;
using PhysLab.Views;

namespace PhysLab
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PendulumCard_Click(object sender, MouseButtonEventArgs e)
        {
            new PendulumWindow().ShowDialog();
        }

        private void SpringCard_Click(object sender, MouseButtonEventArgs e)
        {
            new SpringWindow().ShowDialog();
        }

        private void ProjectileCard_Click(object sender, MouseButtonEventArgs e)
        {
            new ProjectileWindow().ShowDialog();
        }

        private void NewtonLawCard_Click(object sender, MouseButtonEventArgs e)
        {
            new NewtonLawWindow().ShowDialog();
        }

        private void ExperimentsButton_Click(object sender, RoutedEventArgs e)
        {
            new ExperimentsWindow().ShowDialog();
        }

        private void SavedResultsButton_Click(object sender, RoutedEventArgs e)
        {
            new SavedResultsWindow().ShowDialog();
        }
    }
}
