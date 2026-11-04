
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
    public partial class NewtonLawWindow : Window
    {
        private DispatcherTimer animationTimer;

        private readonly ExperimentStorageService storageService = new();

        private double acceleration;
        private double initialVelocity;
        private double currentVelocity;
        private double elapsedTime;

        private double mass;
        private double appliedForce;
        private double momentum;
        private double weight;

        private const double StartX = 45;
        private const double GroundY = 450;
        private const double BlockWidth = 65;
        private const double BlockHeight = 55;
        private const double PixelsPerMeter = 15;

        private bool hasCalculated = false;
        private bool isRunning = false;

        public NewtonLawWindow()
        {
            InitializeComponent();

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(16);
            animationTimer.Tick += AnimationTimer_Tick;

            Closed += (sender, e) => animationTimer.Stop();

            ResetSimulation();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            bool validMass = double.TryParse(
                MassTextBox.Text, out double inputMass);

            bool validForce = double.TryParse(
                ForceTextBox.Text, out double force);

            bool validVelocity = double.TryParse(
                VelocityTextBox.Text, out double velocity);

            if (!validMass || !validForce || !validVelocity)
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
                NewtonLawExperiment experiment =
                    new NewtonLawExperiment(inputMass, force, velocity);

                experiment.CalculateResults();

                mass = inputMass;
                appliedForce = force;
                initialVelocity = velocity;
                currentVelocity = velocity;

                acceleration = experiment.Acceleration;
                momentum = experiment.Momentum;
                weight = experiment.Weight;

                AccelerationResult.Text =
                    acceleration.ToString("F3") + " m/s²";

                MomentumResult.Text =
                    momentum.ToString("F3") + " kg·m/s";

                WeightResult.Text =
                    weight.ToString("F3") + " N";

                elapsedTime = 0;
                hasCalculated = true;

                StopAnimation();
                ResetSimulation();
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
                    ExperimentName = "Newton's Second Law",
                    SavedAt = DateTime.Now,

                    Parameters = new Dictionary<string, string>
                    {
                        { "Mass", mass + " kg" },
                        { "Applied Force", appliedForce + " N" },
                        { "Initial Velocity", initialVelocity + " m/s" }
                    },

                    Results = new List<ExperimentResult>
                    {
                        new ExperimentResult(
                            "Newton's Second Law",
                            "Acceleration",
                            acceleration,
                            "m/s²"),

                        new ExperimentResult(
                            "Newton's Second Law",
                            "Momentum",
                            momentum,
                            "kg·m/s"),

                        new ExperimentResult(
                            "Newton's Second Law",
                            "Weight",
                            weight,
                            "N")
                    }
                };

                storageService.SaveExperiment(savedExperiment);

                MessageBox.Show(
                    "Newton's Second Law experiment saved successfully!",
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
            double deltaTime = animationTimer.Interval.TotalSeconds;

            elapsedTime += deltaTime;

            currentVelocity = initialVelocity + acceleration * elapsedTime;

            double displacement =
                initialVelocity * elapsedTime +
                0.5 * acceleration * elapsedTime * elapsedTime;

            double blockX = StartX + displacement * PixelsPerMeter;

            double maxX = SimulationCanvas.ActualWidth - BlockWidth - 10;

            if (blockX >= maxX)
            {
                blockX = maxX;
                StopAnimation();
            }

            Canvas.SetLeft(MovingBlock, blockX);

            ForceArrowLine.X1 = blockX + BlockWidth;
            ForceArrowLine.X2 = blockX + BlockWidth + 70;

            ForceArrowHead.Points = new PointCollection
            {
                new Point(blockX + BlockWidth + 70, GroundY - 40),
                new Point(blockX + BlockWidth + 85, GroundY - 30),
                new Point(blockX + BlockWidth + 70, GroundY - 20)
            };
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();

            elapsedTime = 0;
            currentVelocity = initialVelocity;

            ResetSimulation();
        }

        private void ResetSimulation()
        {
            Canvas.SetLeft(MovingBlock, StartX);
            Canvas.SetTop(MovingBlock, GroundY - BlockHeight);

            ForceArrowLine.X1 = StartX + BlockWidth;
            ForceArrowLine.Y1 = GroundY - 30;

            ForceArrowLine.X2 = StartX + BlockWidth + 70;
            ForceArrowLine.Y2 = GroundY - 30;

            ForceArrowHead.Points = new PointCollection
            {
                new Point(StartX + BlockWidth + 70, GroundY - 40),
                new Point(StartX + BlockWidth + 85, GroundY - 30),
                new Point(StartX + BlockWidth + 70, GroundY - 20)
            };
        }

        private void StopAnimation()
        {
            animationTimer.Stop();

            isRunning = false;
            StartButton.Content = "Start";
        }
    }
}
