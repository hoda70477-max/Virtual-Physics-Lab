
using System;
using System.Windows;
using PhysLab.Services;

namespace PhysLab.Views
{
    public partial class SavedResultsWindow : Window
    {
        private readonly ExperimentStorageService storageService = new();

        public SavedResultsWindow()
        {
            InitializeComponent();
            LoadSavedExperiments();
        }

        private void LoadSavedExperiments()
        {
            try
            {
                var experiments = storageService.GetAllSavedExperiments();

                SavedExperimentsGrid.ItemsSource = experiments;

                ExperimentsCountText.Text =
                    $"Saved Experiments: {experiments.Count}";

                EmptyMessage.Visibility =
                    experiments.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load saved experiments.\n" + ex.Message,
                    "Loading Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
