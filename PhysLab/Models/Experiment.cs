namespace PhysLab.Models
{
    public abstract class Experiment
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<ExperimentResult> Results { get; private set; }

        protected Experiment(string name, string description)
        {
            Name = name;
            Description = description;
            Results = new List<ExperimentResult>();
        }

        public abstract void CalculateResults();
    }
}