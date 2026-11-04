
using PhysLab.Models;

namespace PhysLab.Services
{
    public class ExperimentManager
    {
        private readonly List<Experiment> _experiments;

        public ExperimentManager()
        {
            _experiments = new List<Experiment>();
        }

        public void AddExperiment(Experiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            _experiments.Add(experiment);
        }

        public void RunExperiment(Experiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            if (!_experiments.Contains(experiment))
                throw new InvalidOperationException("Experiment not found.");

            experiment.CalculateResults();
        }

        public List<Experiment> GetAllExperiments()
        {
            return new List<Experiment>(_experiments);
        }
    }
}
