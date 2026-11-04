
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhysLab.Models
{
    public class SavedExperiment
    {
        public string ExperimentName { get; set; } = string.Empty;

        public DateTime SavedAt { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = new();

        public List<ExperimentResult> Results { get; set; } = new();

        [JsonIgnore]
        public string ParametersSummary =>
            string.Join(" | ",
                Parameters.Select(p => $"{p.Key}: {p.Value}"));

        [JsonIgnore]
        public string ResultsSummary =>
            string.Join(" | ",
                Results.Select(r =>
                    $"{r.ResultName}: {r.Value:F3} {r.Unit}"));
    }
}
