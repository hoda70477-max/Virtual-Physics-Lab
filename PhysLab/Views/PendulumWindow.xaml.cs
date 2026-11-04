
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
    public partial class PendulumWindow : Window
    {
        private DispatcherTimer animationTimer;
        private readonly ExperimentStorageService storageService = new();

        private double angle = 0;
        private double elapsedTime = 0;

        private double length = 1;
        private double gravity = 9.81;
        private double period = 2.006;

        private const double Amplitude = 0.35;
        private const double PivotX = 215;
        private const double PivotY = 42;
        private const double VisualLength = 180;

        private bool isRunning = false;
        private bool hasCalculated = false;

        public PendulumWindow()
        {
            InitializeComponent();

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(16);
            animationTimer.Tick += AnimationTimer_Tick;

            Closed += (sender, e) => animationTimer.Stop();

            DrawPendulum(0);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            bool validLength = double.TryParse(
                LengthTextBox.Text, out double inputLength);

            bool validGravity = double.TryParse(
                GravityTextBox.Text, out double inputGravity);

            if (!validLength || !validGravity)
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
                PendulumExperiment experiment =
                    new PendulumExperiment(inputLength, inputGravity);

                experiment.CalculateResults();

                length = inputLength;
                gravity = inputGravity;
                period = experiment.Period;

                PeriodResultText.Text = period.ToString("F3");

                elapsedTime = 0;
                hasCalculated = true;

                StopAnimation();
                DrawPendulum(0);
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
                    ExperimentName = "Simple Pendulum",
                    SavedAt = DateTime.Now,

                    Parameters = new Dictionary<string, string>
                    {
                        { "Length", length + " m" },
                        { "Gravity", gravity + " m/s²" }
                    },

                    Results = new List<ExperimentResult>
                    {
                        new ExperimentResult(
                            "Simple Pendulum",
                            "Time Period",
                            period,
                            "Seconds")
                    }
                };

                storageService.SaveExperiment(savedExperiment);

                MessageBox.Show(
                    "Experiment saved successfully!",
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

            double angularFrequency = 2 * Math.PI / period;

            angle = Amplitude *
                    Math.Cos(angularFrequency * elapsedTime);

            DrawPendulum(angle);
        }

        private void DrawPendulum(double currentAngle)
        {
            double bobX = PivotX +
                          VisualLength * Math.Sin(currentAngle);

            double bobY = PivotY +
                          VisualLength * Math.Cos(currentAngle);

            PendulumString.X1 = PivotX;
            PendulumString.Y1 = PivotY;

            PendulumString.X2 = bobX;
            PendulumString.Y2 = bobY;

            Canvas.SetLeft(
                PendulumBob,
                bobX - PendulumBob.Width / 2);

            Canvas.SetTop(
                PendulumBob,
                bobY - PendulumBob.Height / 2);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();

            elapsedTime = 0;
            angle = 0;

            DrawPendulum(0);
        }

        private void StopAnimation()
        {
            animationTimer.Stop();

            isRunning = false;
            StartButton.Content = "Start";
        }
    }
}
