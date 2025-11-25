using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class SensorSession
    {
        [Key]                      // ✅ primary key
        public int SessionID { get; set; }

        public int PatientID { get; set; }   // FK -> Patient

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string RecordingArea { get; set; }
        public string SourceDeviceID { get; set; }
        public string DataFilePath { get; set; }
        public string AlgorithmDescription { get; set; }
    }
}
