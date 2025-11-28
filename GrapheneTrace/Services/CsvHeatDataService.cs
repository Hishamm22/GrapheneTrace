using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace GrapheneTrace.Services
{
    public class CsvHeatDataService : IHeatDataService
    {
        private readonly string _heatDataFolder;

        public CsvHeatDataService(IWebHostEnvironment env)
        {
            // HeatData folder in project root
            _heatDataFolder = Path.Combine(env.ContentRootPath, "HeatData");
        }

        public HeatmapResult GetLatestHeatmap(string deviceId)
        {
            var files = GetDeviceFiles(deviceId);
            if (!files.Any())
                return null;

            var latest = files
                .OrderByDescending(f => f.Date ?? DateTime.MinValue)
                .First();

            var matrix = LoadMatrix(latest.Path);

            return new HeatmapResult
            {
                Values = matrix,
                Timestamp = latest.Date
            };
        }

        public List<TrendPoint> GetPeakTrend(string deviceId)
        {
            var files = GetDeviceFiles(deviceId);
            var result = new List<TrendPoint>();

            foreach (var file in files.OrderBy(f => f.Date))
            {
                var matrix = LoadMatrix(file.Path);
                double peak = 0;

                var rows = matrix.GetLength(0);
                var cols = matrix.GetLength(1);

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        if (matrix[r, c] > peak)
                            peak = matrix[r, c];
                    }

                result.Add(new TrendPoint
                {
                    Date = file.Date,
                    PeakPressure = peak
                });
            }

            return result;
        }

        // ------------- helpers -------------

        private (string Path, DateTime? Date)[] GetDeviceFiles(string deviceId)
        {
            if (!Directory.Exists(_heatDataFolder))
                return Array.Empty<(string, DateTime?)>();

            var pattern = $"{deviceId}_*.csv";

            return Directory
                .GetFiles(_heatDataFolder, pattern)
                .Select(path =>
                {
                    var name = Path.GetFileNameWithoutExtension(path); // e.g. d13043b3_20251011
                    var parts = name.Split('_');
                    DateTime? dt = null;

                    if (parts.Length >= 2 &&
                        DateTime.TryParseExact(parts[1],
                            "yyyyMMdd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var parsed))
                    {
                        dt = parsed;
                    }

                    return (path, (DateTime?)dt);
                })
                .ToArray();
        }

        private double[,] LoadMatrix(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0)
                return new double[0, 0];

            // Assume first row is header
            var headerParts = lines[0].Split(',');
            int cols = headerParts.Length;
            int rows = lines.Length - 1;

            var matrix = new double[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                var parts = lines[r + 1].Split(',');
                for (int c = 0; c < cols; c++)
                {
                    if (c < parts.Length &&
                        double.TryParse(parts[c],
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out var value))
                    {
                        matrix[r, c] = value;
                    }
                    else
                    {
                        matrix[r, c] = 0;
                    }
                }
            }

            return matrix;
        }
    }
}
