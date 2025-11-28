using System;
using System.Collections.Generic;

namespace GrapheneTrace.Services
{
    public class HeatmapResult
    {
        public double[,] Values { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class TrendPoint
    {
        public DateTime? Date { get; set; }
        public double PeakPressure { get; set; }
    }

    public interface IHeatDataService
    {
        HeatmapResult GetLatestHeatmap(string deviceId);
        List<TrendPoint> GetPeakTrend(string deviceId);
    }
}
