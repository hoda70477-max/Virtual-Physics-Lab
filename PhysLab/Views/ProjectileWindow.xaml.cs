
using PhysLab.Experiments;
using PhysLab.Models;
using PhysLab.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PhysLab.Views
{
    public partial class ProjectileWindow : Window
    {
        private readonly DispatcherTimer animationTimer = new();
        private readonly ExperimentStorageService storageService = new();

        private double initialVelocity;
        private double launchAngle;
        private double gravity;

        private double flightTime;
        private double maximumHeight;
        private double horizontalRange;

        private double elapsedTime;

        private double horizontalVelocity;
        private double verticalVelocity;

        private double drawingScale = 1;
        private double groundY = 0;
        private double startX = 35;

        private const double BottomMargin = 35;
        private const double TopMargin = 35;
        private const double SideMargin = 35;

        private bool hasCalculated = false;
        private bool isRunning = false;

        public ProjectileWindow()
        {
            InitializeComponent();

            animationTimer.Interval =
                TimeSpan.FromMilliseconds(16);

            animationTimer.Tick += AnimationTimer_Tick;

            Closed += (sender, e) => animationTimer.Stop();

            Loaded += (sender, e) =>
            {
                UpdateDrawingScale();
                ResetProjectile();
            };
        }

        private void CalculateButton_Click(
            object sender, RoutedEventArgs e)
        {
            bool validVelocity = double.TryParse(
                VelocityTextBox.Text, out double velocity);

            bool validAngle = double.TryParse(
                AngleTextBox.Text, out double angle);

            bool validGravity = double.TryParse(
                GravityTextBox.Text, out double g);

            if (!validVelocity || !validAngle || !validGravity)
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
                ProjectileExperiment experiment =
                    new ProjectileExperiment(velocity, angle, g);

                experiment.CalculateResults();

                initialVelocity = experiment.InitialVelocity;
                launchAngle = experiment.LaunchAngle;
                gravity = experiment.Gravity;

                flightTime = experiment.FlightTime;
                maximumHeight = experiment.MaximumHeight;
                horizontalRange = experiment.Range;

                double angleRadians =
                    launchAngle * Math.PI / 180;

                horizontalVelocity =
                    initialVelocity * Math.Cos(angleRadians);

                verticalVelocity =
                    initialVelocity * Math.Sin(angleRadians);

                FlightTimeResult.Text =
                    flightTime.ToString("F3") + " s";

                HeightResult.Text =
                    maximumHeight.ToString("F3") + " m";

                RangeResult.Text =
                    horizontalRange.ToString("F3") + " m";

                hasCalculated = true;

                StopAnimation();

                elapsedTime = 0;

                UpdateDrawingScale();
                DrawProjectilePath();
                UpdateProjectilePosition(0);
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

        private void SimulationCanvas_SizeChanged(
            object sender, SizeChangedEventArgs e)
        {
            UpdateDrawingScale();

            if (hasCalculated)
            {
                DrawProjectilePath();
                UpdateProjectilePosition(elapsedTime);
            }
            else
            {
                ResetProjectile();
            }
        }

        private void UpdateDrawingScale()
        {
            double canvasWidth = SimulationCanvas.ActualWidth;
            double canvasHeight = SimulationCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0)
                return;

            startX = SideMargin;
            groundY = canvasHeight - BottomMargin;

            double availableWidth =
                Math.Max(1, canvasWidth - (SideMargin * 2));

            double availableHeight =
                Math.Max(1, canvasHeight - TopMargin - BottomMargin);

            double horizontalScale = horizontalRange > 0
                ? availableWidth / horizontalRange
                : double.MaxValue;

            double verticalScale = maximumHeight > 0
                ? availableHeight / maximumHeight
                : double.MaxValue;

            drawingScale = Math.Min(
                horizontalScale,
                verticalScale);

            if (double.IsInfinity(drawingScale) ||
                double.IsNaN(drawingScale) ||
                drawingScale <= 0)
            {
                drawingScale = 1;
            }

            GroundLine.X1 = 10;
            GroundLine.Y1 = groundY;
            GroundLine.X2 = canvasWidth - 10;
            GroundLine.Y2 = groundY;
        }

        private void DrawProjectilePath()
        {
            if (!hasCalculated)
                return;

            if (SimulationCanvas.ActualWidth <= 0 ||
                SimulationCanvas.ActualHeight <= 0)
                return;

            PointCollection points = new();

            const int steps = 150;

            for (int i = 0; i <= steps; i++)
            {
                double time = flightTime * i / steps;

                double x = horizontalVelocity * time;

                double y = verticalVelocity * time -
                           0.5 * gravity * time * time;

                double canvasX = startX + x * drawingScale;
                double canvasY = groundY - y * drawingScale;

                points.Add(new Point(canvasX, canvasY));
            }

            ProjectilePath.Points = points;
        }

        private void UpdateProjectilePosition(double time)
        {
            double x = horizontalVelocity * time;

            double y = verticalVelocity * time -
                       0.5 * gravity * time * time;

            double canvasX = startX + x * drawingScale;
            double canvasY = groundY - y * drawingScale;

            Canvas.SetLeft(
                ProjectileBall,
                canvasX - ProjectileBall.Width / 2);

            Canvas.SetTop(
                ProjectileBall,
                canvasY - ProjectileBall.Height / 2);
        }

        private void StartButton_Click(
            object sender, RoutedEventArgs e)
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

            if (elapsedTime >= flightTime)
            {
                elapsedTime = 0;
                UpdateProjectilePosition(0);
            }

            isRunning = true;
            StartButton.Content = "Pause";

            animationTimer.Start();
        }

        private void AnimationTimer_Tick(
            object? sender, EventArgs e)
        {
            elapsedTime += animationTimer.Interval.TotalSeconds;

            if (elapsedTime >= flightTime)
            {
                elapsedTime = flightTime;

                UpdateProjectilePosition(elapsedTime);

                StopAnimation();
                return;
            }

            UpdateProjectilePosition(elapsedTime);
        }

        private void ResetButton_Click(
            object sender, RoutedEventArgs e)
        {
            StopAnimation();

            elapsedTime = 0;

            ProjectilePath.Points = new PointCollection();

            hasCalculated = false;

            FlightTimeResult.Text = "--";
            HeightResult.Text = "--";
            RangeResult.Text = "--";

            ResetProjectile();
        }

        private void ResetProjectile()
        {
            Canvas.SetLeft(
                ProjectileBall,
                startX - ProjectileBall.Width / 2);

            Canvas.SetTop(
                ProjectileBall,
                groundY - ProjectileBall.Height / 2);
        }

        private void StopAnimation()
        {
            animationTimer.Stop();

            isRunning = false;

            StartButton.Content = "Start";
        }

        private void SaveButton_Click(
            object sender, RoutedEventArgs e)
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
                SavedExperiment savedExperiment = new()
                {
                    ExperimentName = "Projectile Motion",
                    SavedAt = DateTime.Now,

                    Parameters = new Dictionary<string, string>
                    {
                        { "Initial Velocity", initialVelocity + " m/s" },
                        { "Launch Angle", launchAngle + " degrees" },
                        { "Gravity", gravity + " m/s²" }
                    },

                    Results = new List<ExperimentResult>
                    {
                        new ExperimentResult(
                            "Projectile Motion",
                            "Flight Time",
                            flightTime,
                            "Seconds"),

                        new ExperimentResult(
                            "Projectile Motion",
                            "Maximum Height",
                            maximumHeight,
                            "Meters"),

                        new ExperimentResult(
                            "Projectile Motion",
                            "Horizontal Range",
                            horizontalRange,
                            "Meters")
                    }
                };

                storageService.SaveExperiment(savedExperiment);

                MessageBox.Show(
                    "Projectile experiment saved successfully!",
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
    }
}
