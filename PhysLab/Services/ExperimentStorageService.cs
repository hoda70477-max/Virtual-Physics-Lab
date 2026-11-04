
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PhysLab.Models;

namespace PhysLab.Services
{
    public class ExperimentStorageService
    {
        private readonly string _filePath;

        public ExperimentStorageService()
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "PhysLab");

            Directory.CreateDirectory(folderPath);

            _filePath = Path.Combine(
                folderPath,
                "saved_experiments.json");
        }

        public void SaveExperiment(SavedExperiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            List<SavedExperiment> experiments =
                GetAllSavedExperiments();

            experiments.Add(experiment);

            string json = JsonSerializer.Serialize(
                experiments,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_filePath, json);
        }

        public List<SavedExperiment> GetAllSavedExperiments()
        {
            if (!File.Exists(_filePath))
                return new List<SavedExperiment>();

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<SavedExperiment>();

            return JsonSerializer.Deserialize<List<SavedExperiment>>(json)
                   ?? new List<SavedExperiment>();
        }
    }
}
