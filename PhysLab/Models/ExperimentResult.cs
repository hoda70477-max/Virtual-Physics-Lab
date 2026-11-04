
namespace PhysLab.Models
{
    public class ExperimentResult
    {
        public string ExperimentName { get; private set; }

        public string ResultName { get; private set; }

        public double Value { get; private set; }

        public string Unit { get; private set; }

        public ExperimentResult(
            string experimentName,
            string resultName,
            double value,
            string unit)
        {
            ExperimentName = experimentName;
            ResultName = resultName;
            Value = value;
            Unit = unit;
        }
    }
}
