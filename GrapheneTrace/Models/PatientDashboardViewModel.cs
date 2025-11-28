using System;
using System.Collections.Generic;
using GrapheneTrace.Services;

namespace GrapheneTrace.Models
{
    public class PatientDashboardViewModel
    {
        public string PatientName { get; set; }
        public string Email { get; set; }

        public double PeakPressure { get; set; }
        public double ContactArea { get; set; }
        public DateTime? LastUpdated { get; set; }

        public double[,] Heatmap { get; set; }
        public List<TrendPoint> PeakTrend { get; set; } = new List<TrendPoint>();
    }
}
