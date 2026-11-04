
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
    public partial class NewtonLawWindow : Window
    {
        private readonly DispatcherTimer animationTimer = new();
        private readonly ExperimentStorageService storageService = new();

        private double acceleration;
        private double initialVelocity;
        private double currentVelocity;
        private double elapsedTime;
        private double displacement;

        private double mass;
        private double appliedForce;
        private double momentum;
        private double weight;
        private double frictionForce;
        private double netForce;
        private double frictionCoefficient;

        private bool frictionEnabled;
        private bool hasCalculated;
        private bool isRunning;

        private double startX = 35;
        private double groundY;
        private double pixelsPerMeter = 1;

        private const double BlockWidth = 65;
        private const double BlockHeight = 55;
        private const double BottomMargin = 35;
        private const double SimulationDuration = 5;
        private const double Gravity = 9.81;

        public NewtonLawWindow()
        {
            InitializeComponent();

            animationTimer.Interval =
                TimeSpan.FromMilliseconds(16);

            animationTimer.Tick += AnimationTimer_Tick;

            Closed += (sender, e) => animationTimer.Stop();

            Loaded += (sender, e) =>
            {
                UpdateDrawingScale();
                ResetSimulation();
            };
        }

        private void CalculateButton_Click(
            object sender, RoutedEventArgs e)
        {
            bool validMass = double.TryParse(
                MassTextBox.Text, out double inputMass);

            bool validForce = double.TryParse(
                ForceTextBox.Text, out double force);

            bool validVelocity = double.TryParse(
                VelocityTextBox.Text, out double velocity);

            bool validCoefficient = double.TryParse(
                FrictionCoefficientTextBox.Text,
                out double coefficient);

            if (!validMass || !validForce ||
                !validVelocity || !validCoefficient)
            {
                MessageBox.Show(
                    "Please enter valid numeric values in all fields.",
                    "Invalid Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                frictionEnabled = FrictionCheckBox.IsChecked == true;
                frictionCoefficient = coefficient;

                NewtonLawExperiment experiment =
                    new NewtonLawExperiment(
                        inputMass,
                        force,
                        velocity,
                        frictionEnabled,
                        frictionCoefficient);

                experiment.CalculateResults();

                mass = inputMass;
                appliedForce = force;
                initialVelocity = velocity;
                currentVelocity = velocity;

                acceleration = experiment.Acceleration;
                momentum = experiment.Momentum;
                weight = experiment.Weight;
                frictionForce = experiment.FrictionForce;
                netForce = experiment.NetForce;

                AccelerationResult.Text =
                    acceleration.ToString("F3") + " m/s²";

                FrictionResult.Text =
                    frictionForce.ToString("F3") + " N";

                NetForceResult.Text =
                    netForce.ToString("F3") + " N";

                MomentumResult.Text =
                    momentum.ToString("F3") + " kg·m/s";

                WeightResult.Text =
                    weight.ToString("F3") + " N";

                elapsedTime = 0;
                displacement = 0;
                hasCalculated = true;

                StopAnimation();

                UpdateDrawingScale();
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

        private void SimulationCanvas_SizeChanged(
            object sender, SizeChangedEventArgs e)
        {
            UpdateDrawingScale();

            if (hasCalculated)
                UpdateSimulationDrawing();
            else
                ResetSimulation();
        }

        private void UpdateDrawingScale()
        {
            double canvasWidth = SimulationCanvas.ActualWidth;
            double canvasHeight = SimulationCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0)
                return;

            startX = 35;
            groundY = canvasHeight - BottomMargin;

            double availableWidth =
                Math.Max(1, canvasWidth - startX - BlockWidth - 45);

            double endDisplacement =
                initialVelocity * SimulationDuration +
                0.5 * Math.Max(0, acceleration) *
                SimulationDuration * SimulationDuration;

            if (endDisplacement > 0)
            {
                pixelsPerMeter =
                    availableWidth / endDisplacement;
            }
            else
            {
                pixelsPerMeter = 1;
            }

            if (double.IsNaN(pixelsPerMeter) ||
                double.IsInfinity(pixelsPerMeter) ||
                pixelsPerMeter <= 0)
            {
                pixelsPerMeter = 1;
            }

            GroundLine.X1 = 10;
            GroundLine.Y1 = groundY;
            GroundLine.X2 = canvasWidth - 10;
            GroundLine.Y2 = groundY;
        }

        private void UpdateSimulationDrawing()
        {
            double blockX =
                startX + displacement * pixelsPerMeter;

            double maxX =
                SimulationCanvas.ActualWidth - BlockWidth - 15;

            blockX = Math.Max(
                startX,
                Math.Min(blockX, maxX));

            Canvas.SetLeft(MovingBlock, blockX);
            Canvas.SetTop(
                MovingBlock,
                groundY - BlockHeight);

            double arrowY =
                groundY - BlockHeight / 2;

            // Applied force arrow points to the right.
            ForceArrowLine.X1 = blockX + BlockWidth;
            ForceArrowLine.Y1 = arrowY;
            ForceArrowLine.X2 = blockX + BlockWidth + 55;
            ForceArrowLine.Y2 = arrowY;

            ForceArrowHead.Points = new PointCollection
            {
                new Point(blockX + BlockWidth + 55, arrowY - 9),
                new Point(blockX + BlockWidth + 68, arrowY),
                new Point(blockX + BlockWidth + 55, arrowY + 9)
            };

            Canvas.SetLeft(
                ForceLabel,
                blockX + BlockWidth - 5);

            Canvas.SetTop(
                ForceLabel,
                arrowY - 32);

            // Friction arrow points to the left.
            bool showFriction =
                frictionEnabled && frictionForce > 0.001;

            FrictionArrowLine.Visibility =
                showFriction ? Visibility.Visible : Visibility.Collapsed;

            FrictionArrowHead.Visibility =
                showFriction ? Visibility.Visible : Visibility.Collapsed;

            FrictionLabel.Visibility =
                showFriction ? Visibility.Visible : Visibility.Collapsed;

            if (showFriction)
            {
                double frictionStart = blockX;
                double frictionEnd = blockX - 55;

                FrictionArrowLine.X1 = frictionStart;
                FrictionArrowLine.Y1 = arrowY + 20;
                FrictionArrowLine.X2 = frictionEnd;
                FrictionArrowLine.Y2 = arrowY + 20;

                FrictionArrowHead.Points = new PointCollection
                {
                    new Point(frictionEnd, arrowY + 11),
                    new Point(frictionEnd - 13, arrowY + 20),
                    new Point(frictionEnd, arrowY + 29)
                };

                Canvas.SetLeft(
                    FrictionLabel,
                    blockX - 65);

                Canvas.SetTop(
                    FrictionLabel,
                    arrowY + 25);
            }
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

            if (elapsedTime >= SimulationDuration)
            {
                elapsedTime = 0;
                displacement = 0;
                currentVelocity = initialVelocity;
                UpdateSimulationDrawing();
            }

            isRunning = true;
            StartButton.Content = "Pause";

            animationTimer.Start();
        }

        private void AnimationTimer_Tick(
            object? sender, EventArgs e)
        {
            double deltaTime =
                animationTimer.Interval.TotalSeconds;

            double nextVelocity =
                currentVelocity + acceleration * deltaTime;

            if (nextVelocity < 0)
            {
                // Stop at zero velocity instead of moving backwards.
                if (acceleration < 0)
                {
                    displacement +=
                        (currentVelocity * currentVelocity) /
                        (2 * -acceleration);
                }

                currentVelocity = 0;
                elapsedTime = SimulationDuration;

                UpdateSimulationDrawing();
                StopAnimation();
                return;
            }

            displacement +=
                (currentVelocity + nextVelocity) / 2 * deltaTime;

            currentVelocity = nextVelocity;
            elapsedTime += deltaTime;

            if (elapsedTime >= SimulationDuration)
            {
                elapsedTime = SimulationDuration;
                UpdateSimulationDrawing();
                StopAnimation();
                return;
            }

            UpdateSimulationDrawing();

            double maxX =
                SimulationCanvas.ActualWidth - BlockWidth - 15;

            if (startX + displacement * pixelsPerMeter >= maxX)
            {
                StopAnimation();
            }
        }

        private void ResetButton_Click(
            object sender, RoutedEventArgs e)
        {
            StopAnimation();

            elapsedTime = 0;
            displacement = 0;
            currentVelocity = initialVelocity;

            ResetSimulation();
        }

        private void ResetSimulation()
        {
            displacement = 0;

            double arrowY =
                groundY - BlockHeight / 2;

            Canvas.SetLeft(MovingBlock, startX);
            Canvas.SetTop(
                MovingBlock,
                groundY - BlockHeight);

            ForceArrowLine.X1 = startX + BlockWidth;
            ForceArrowLine.Y1 = arrowY;
            ForceArrowLine.X2 = startX + BlockWidth + 55;
            ForceArrowLine.Y2 = arrowY;

            ForceArrowHead.Points = new PointCollection
            {
                new Point(startX + BlockWidth + 55, arrowY - 9),
                new Point(startX + BlockWidth + 68, arrowY),
                new Point(startX + BlockWidth + 55, arrowY + 9)
            };

            Canvas.SetLeft(
                ForceLabel,
                startX + BlockWidth - 5);

            Canvas.SetTop(
                ForceLabel,
                arrowY - 32);

            bool showFriction =
                frictionEnabled && frictionForce > 0.001;

            FrictionArrowLine.Visibility =
                showFriction ? Visibility.Visible : Visibility.Collapsed;

            FrictionArrowHead.Visibility =
                showFriction ? Visibility.Visible : Visibility.Collapsed;

            FrictionLabel.Visibility =
                showFriction ? Visibility.Visible : Visibility.Collapsed;

            if (showFriction)
            {
                FrictionArrowLine.X1 = startX;
                FrictionArrowLine.Y1 = arrowY + 20;
                FrictionArrowLine.X2 = startX - 55;
                FrictionArrowLine.Y2 = arrowY + 20;

                FrictionArrowHead.Points = new PointCollection
                {
                    new Point(startX - 55, arrowY + 11),
                    new Point(startX - 68, arrowY + 20),
                    new Point(startX - 55, arrowY + 29)
                };

                Canvas.SetLeft(
                    FrictionLabel,
                    startX - 65);

                Canvas.SetTop(
                    FrictionLabel,
                    arrowY + 25);
            }
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
                    ExperimentName = "Newton's Second Law",
                    SavedAt = DateTime.Now,

                    Parameters = new Dictionary<string, string>
                    {
                        { "Mass", mass + " kg" },
                        { "Applied Force", appliedForce + " N" },
                        { "Initial Velocity", initialVelocity + " m/s" },
                        { "Friction Enabled", frictionEnabled.ToString() },
                        { "Friction Coefficient", frictionCoefficient.ToString() }
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
                            "Friction Force",
                            frictionForce,
                            "N"),

                        new ExperimentResult(
                            "Newton's Second Law",
                            "Net Force",
                            netForce,
                            "N"),

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
    }
}
