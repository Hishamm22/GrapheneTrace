using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class Comment
    {
        public int CommentID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int SessionID { get; set; }

        public int? FrameID { get; set; } // optional – note might be for whole session

        public int AuthorUserID { get; set; } // who wrote it (patient or clinician)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        // If you later add clinician replies, this flag helps distinguish them.
        public bool IsClinicianReply { get; set; }

        // Navigation
        public Patient Patient { get; set; }
        public SensorSession Session { get; set; }
        public SensorFrame? Frame { get; set; }
    }
}
