using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class Alert
    {
        [Key]                      // ✅ primary key
        public int AlertID { get; set; }

        public int PatientID { get; set; }        // FK -> Patient
        public int SessionID { get; set; }        // FK -> SensorSession
        public int FrameID { get; set; }          // FK -> SensorFrame

        public DateTime AlertTime { get; set; }
        public string AlertType { get; set; }
        public string Severity { get; set; }

        public bool IsAcknowledged { get; set; }
        public int? AcknowledgedByUserID { get; set; } // FK -> User (nullable)
        public DateTime? AcknowledgedAt { get; set; }
    }
}
