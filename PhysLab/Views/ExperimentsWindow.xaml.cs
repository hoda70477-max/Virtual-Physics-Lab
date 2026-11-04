
using System.Windows;

namespace PhysLab.Views
{
    public partial class ExperimentsWindow : Window
    {
        public ExperimentsWindow()
        {
            InitializeComponent();
        }

        private void PendulumButton_Click(object sender, RoutedEventArgs e)
        {
            new PendulumWindow().ShowDialog();
        }

        private void SpringButton_Click(object sender, RoutedEventArgs e)
        {
            new SpringWindow().ShowDialog();
        }

        private void ProjectileButton_Click(object sender, RoutedEventArgs e)
        {
            new ProjectileWindow().ShowDialog();
        }

        private void NewtonButton_Click(object sender, RoutedEventArgs e)
        {
            new NewtonLawWindow().ShowDialog();
        }
    }
}
