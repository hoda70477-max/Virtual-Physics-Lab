
using System;
using PhysLab.Models;

namespace PhysLab.Experiments
{
    public class NewtonLawExperiment : Experiment
    {
        public double Mass { get; private set; }

        public double AppliedForce { get; private set; }

        public double InitialVelocity { get; private set; }

        public double Acceleration { get; private set; }

        public double Momentum { get; private set; }

        public double Weight { get; private set; }

        public NewtonLawExperiment(
            double mass,
            double appliedForce,
            double initialVelocity)
            : base(
                "Newton's Second Law",
                "Study the relationship between force, mass, and acceleration")
        {
            if (mass <= 0)
                throw new ArgumentException("Mass must be positive.");

            if (appliedForce < 0)
                throw new ArgumentException("Applied force cannot be negative.");

            if (initialVelocity < 0)
                throw new ArgumentException("Initial velocity cannot be negative.");

            Mass = mass;
            AppliedForce = appliedForce;
            InitialVelocity = initialVelocity;
        }

        public override void CalculateResults()
        {
            Acceleration = AppliedForce / Mass;

            Momentum = Mass * InitialVelocity;

            Weight = Mass * 9.81;

            Results.Clear();

            Results.Add(new ExperimentResult(
                Name,
                "Acceleration",
                Acceleration,
                "m/s²"));

            Results.Add(new ExperimentResult(
                Name,
                "Momentum",
                Momentum,
                "kg·m/s"));

            Results.Add(new ExperimentResult(
                Name,
                "Weight",
                Weight,
                "N"));
        }
    }
}
