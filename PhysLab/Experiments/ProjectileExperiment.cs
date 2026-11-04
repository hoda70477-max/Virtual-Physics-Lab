
using PhysLab.Models;

namespace PhysLab.Experiments
{
    public class ProjectileExperiment : Experiment
    {
        public double InitialVelocity { get; private set; }
        public double LaunchAngle { get; private set; }
        public double Gravity { get; private set; }

        public double FlightTime { get; private set; }
        public double MaximumHeight { get; private set; }
        public double Range { get; private set; }

        public ProjectileExperiment(
            double initialVelocity,
            double launchAngle,
            double gravity)
            : base("Projectile Motion", "Motion of an object launched at an angle")
        {
            if (initialVelocity <= 0)
                throw new ArgumentException("Initial velocity must be positive.");

            if (launchAngle <= 0 || launchAngle >= 90)
                throw new ArgumentException("Angle must be between 0 and 90 degrees.");

            if (gravity <= 0)
                throw new ArgumentException("Gravity must be positive.");

            InitialVelocity = initialVelocity;
            LaunchAngle = launchAngle;
            Gravity = gravity;
        }

        public override void CalculateResults()
        {
            double angleRadians = LaunchAngle * Math.PI / 180;

            double horizontalVelocity =
                InitialVelocity * Math.Cos(angleRadians);

            double verticalVelocity =
                InitialVelocity * Math.Sin(angleRadians);

            FlightTime = 2 * verticalVelocity / Gravity;

            MaximumHeight =
                Math.Pow(verticalVelocity, 2) / (2 * Gravity);

            Range = horizontalVelocity * FlightTime;

            Results.Clear();

            Results.Add(new ExperimentResult(
                Name, "Flight Time", FlightTime, "Seconds"));

            Results.Add(new ExperimentResult(
                Name, "Maximum Height", MaximumHeight, "Meters"));

            Results.Add(new ExperimentResult(
                Name, "Horizontal Range", Range, "Meters"));
        }
    }
}
