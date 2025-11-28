using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class SensorSession
    {
        public int SessionID { get; set; }

        [Required]
        public int PatientID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [StringLength(100)]
        public string? RecordingArea { get; set; } // e.g. "Seat", "Back", etc.

        // Navigation
        public Patient Patient { get; set; }
        public ICollection<SensorFrame> Frames { get; set; } = new List<SensorFrame>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
