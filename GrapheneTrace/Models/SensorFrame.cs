using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class SensorFrame
    {
        [Key]                      // ✅ primary key
        public int FrameID { get; set; }

        public int SessionID { get; set; }   // FK -> SensorSession

        public int FrameIndex { get; set; }
        public DateTime Timestamp { get; set; }

        public int PeakPressureIndex { get; set; }
        public float AveragePressure { get; set; }
        public float ContactAreaPercent { get; set; }

        public float ThermalReactivityScore { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
    }
}
