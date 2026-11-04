
using PhysLab.Models;

namespace PhysLab.Experiments
{
    public class PendulumExperiment : Experiment
    {
        public double Length { get; private set; }

        public double Gravity { get; private set; }

        public double Period { get; private set; }

        public PendulumExperiment(double length, double gravity)
            : base("Simple Pendulum", "A simple pendulum experiment")
        {
            if (length <= 0)
                throw new ArgumentException("Length must be positive.");

            if (gravity <= 0)
                throw new ArgumentException("Gravity must be positive.");

            Length = length;
            Gravity = gravity;
        }

        public override void CalculateResults()
        {
            Period = 2 * Math.PI * Math.Sqrt(Length / Gravity);

            Results.Clear();

            Results.Add(new ExperimentResult(
                Name,
                "Time Period",
                Period,
                "Seconds"
            ));
        }
    }
}
