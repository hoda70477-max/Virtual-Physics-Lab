
using PhysLab.Experiments;
using PhysLab.Models;
using PhysLab.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PhysLab.Views
{
    public partial class SpringWindow : Window
    {
        private DispatcherTimer animationTimer;
        private readonly ExperimentStorageService storageService = new();

        private double elapsedTime = 0;
        private double angularFrequency = 2;
        private double period = 3.142;

        private double mass = 1;
        private double springConstant = 4;

        private const double EquilibriumY = 185;
        private const double Amplitude = 45;

        private bool isRunning = false;
        private bool hasCalculated = false;

        public SpringWindow()
        {
            InitializeComponent();

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(16);
            animationTimer.Tick += AnimationTimer_Tick;

            Closed += (sender, e) => animationTimer.Stop();

            DrawSpring(0);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            bool validMass = double.TryParse(
                MassTextBox.Text, out double inputMass);

            bool validConstant = double.TryParse(
                SpringConstantTextBox.Text, out double inputConstant);

            if (!validMass || !validConstant)
            {
                MessageBox.Show(
                    "Please enter valid numeric values.",
                    "Invalid Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                SpringExperiment experiment =
                    new SpringExperiment(inputMass, inputConstant);

                experiment.CalculateResults();

                mass = inputMass;
                springConstant = inputConstant;

                period = experiment.Period;
                angularFrequency = experiment.AngularFrequency;

                PeriodResultText.Text = period.ToString("F3");

                FrequencyResultText.Text =
                    angularFrequency.ToString("F3");

                elapsedTime = 0;
                hasCalculated = true;

                StopAnimation();
                DrawSpring(0);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Invalid Parameters",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!hasCalculated)
            {
                MessageBox.Show(
                    "Please calculate the results before saving.",
                    "PhysLab",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            try
            {
                SavedExperiment savedExperiment = new SavedExperiment
                {
                    ExperimentName = "Spring-Mass System",
                    SavedAt = DateTime.Now,

                    Parameters = new Dictionary<string, string>
                    {
                        { "Mass", mass + " kg" },
                        { "Spring Constant", springConstant + " N/m" }
                    },

                    Results = new List<ExperimentResult>
                    {
                        new ExperimentResult(
                            "Spring-Mass System",
                            "Time Period",
                            period,
                            "Seconds"),

                        new ExperimentResult(
                            "Spring-Mass System",
                            "Angular Frequency",
                            angularFrequency,
                            "rad/s")
                    }
                };

                storageService.SaveExperiment(savedExperiment);

                MessageBox.Show(
                    "Spring experiment saved successfully!",
                    "PhysLab",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not save the experiment.\n" + ex.Message,
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!hasCalculated)
            {
                MessageBox.Show(
                    "Please calculate the results first.",
                    "PhysLab",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            if (isRunning)
            {
                StopAnimation();
                return;
            }

            isRunning = true;
            StartButton.Content = "Pause";

            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            elapsedTime += animationTimer.Interval.TotalSeconds;

            double displacement =
                Amplitude * Math.Cos(angularFrequency * elapsedTime);

            DrawSpring(displacement);
        }

        private void DrawSpring(double displacement)
        {
            double blockY = EquilibriumY + displacement;

            double springStartY = 47;
            double springEndY = blockY;

            PointCollection points = new PointCollection();

            points.Add(new Point(215, springStartY));

            int segments = 12;
            double segmentHeight =
                (springEndY - springStartY) / segments;

            for (int i = 1; i < segments; i++)
            {
                double y = springStartY + i * segmentHeight;

                double x = (i % 2 == 0) ? 235 : 195;

                points.Add(new Point(x, y));
            }

            points.Add(new Point(215, springEndY));

            SpringLine.Points = points;

            Canvas.SetTop(MassBlock, blockY);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();

            elapsedTime = 0;

            DrawSpring(0);
        }

        private void StopAnimation()
        {
            animationTimer.Stop();

            isRunning = false;
            StartButton.Content = "Start";
        }
    }
}
