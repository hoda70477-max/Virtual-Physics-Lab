
using PhysLab.Models;

namespace PhysLab.Experiments
{
    public class SpringExperiment : Experiment
    {
        public double Mass { get; private set; }

        public double SpringConstant { get; private set; }

        public double Period { get; private set; }

        public double AngularFrequency { get; private set; }

        public SpringExperiment(double mass, double springConstant)
            : base("Spring-Mass System", "Simple harmonic motion of a spring")
        {
            if (mass <= 0)
                throw new ArgumentException("Mass must be positive.");

            if (springConstant <= 0)
                throw new ArgumentException("Spring constant must be positive.");

            Mass = mass;
            SpringConstant = springConstant;
        }

        public override void CalculateResults()
        {
            AngularFrequency = Math.Sqrt(SpringConstant / Mass);

            Period = 2 * Math.PI * Math.Sqrt(Mass / SpringConstant);

            Results.Clear();

            Results.Add(new ExperimentResult(
                Name,
                "Time Period",
                Period,
                "Seconds"
            ));

            Results.Add(new ExperimentResult(
                Name,
                "Angular Frequency",
                AngularFrequency,
                "rad/s"
            ));
        }
    }
}
