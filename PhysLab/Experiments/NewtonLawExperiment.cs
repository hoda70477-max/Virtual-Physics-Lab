
using System;
using PhysLab.Models;

namespace PhysLab.Experiments
{
    public class NewtonLawExperiment : Experiment
    {
        public double Mass { get; private set; }
        public double AppliedForce { get; private set; }
        public double InitialVelocity { get; private set; }

        public bool FrictionEnabled { get; private set; }
        public double FrictionCoefficient { get; private set; }

        public double Acceleration { get; private set; }
        public double Momentum { get; private set; }
        public double Weight { get; private set; }
        public double FrictionForce { get; private set; }
        public double NetForce { get; private set; }

        public NewtonLawExperiment(
            double mass,
            double appliedForce,
            double initialVelocity,
            bool frictionEnabled = false,
            double frictionCoefficient = 0.2)
            : base(
                "Newton's Second Law",
                "Study the relationship between force, mass, friction, and acceleration")
        {
            if (mass <= 0)
                throw new ArgumentException("Mass must be positive.");

            if (appliedForce < 0)
                throw new ArgumentException("Applied force cannot be negative.");

            if (initialVelocity < 0)
                throw new ArgumentException("Initial velocity cannot be negative.");

            if (frictionCoefficient < 0)
                throw new ArgumentException("Friction coefficient cannot be negative.");

            Mass = mass;
            AppliedForce = appliedForce;
            InitialVelocity = initialVelocity;
            FrictionEnabled = frictionEnabled;
            FrictionCoefficient = frictionCoefficient;
        }

        public override void CalculateResults()
        {
            Weight = Mass * 9.81;
            Momentum = Mass * InitialVelocity;

            double maximumFriction =
                FrictionEnabled
                    ? FrictionCoefficient * Weight
                    : 0;

            if (!FrictionEnabled)
            {
                FrictionForce = 0;
            }
            else if (InitialVelocity == 0)
            {
                // At rest, friction balances the applied force
                // until the maximum friction is exceeded.
                FrictionForce = Math.Min(
                    AppliedForce,
                    maximumFriction);
            }
            else
            {
                // While moving, friction acts opposite to motion.
                FrictionForce = maximumFriction;
            }

            NetForce = AppliedForce - FrictionForce;
            Acceleration = NetForce / Mass;

            Results.Clear();

            Results.Add(new ExperimentResult(
                Name, "Acceleration", Acceleration, "m/s²"));

            Results.Add(new ExperimentResult(
                Name, "Momentum", Momentum, "kg·m/s"));

            Results.Add(new ExperimentResult(
                Name, "Weight", Weight, "N"));

            Results.Add(new ExperimentResult(
                Name, "Friction Force", FrictionForce, "N"));

            Results.Add(new ExperimentResult(
                Name, "Net Force", NetForce, "N"));
        }
    }
}
