using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class SensorFrame
    {
        public int FrameID { get; set; }

        [Required]
        public int SessionID { get; set; }

        public int FrameIndex { get; set; } // used to paginate / step through frames

        public DateTime Timestamp { get; set; }

        public int PeakPressureIndex { get; set; }
        public double AveragePressure { get; set; }
        public double ContactAreaPercent { get; set; }
        public double ThermalReactivityScore { get; set; }

        // In a full system you might store the raw matrix too,
        // but for this assignment we only use summary metrics.

        // Navigation
        public SensorSession Session { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
