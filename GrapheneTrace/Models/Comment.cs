using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class Comment
    {
        [Key]                      // ✅ primary key
        public int CommentID { get; set; }

        public int PatientID { get; set; }   // FK -> Patient
        public int SessionID { get; set; }   // FK -> SensorSession
        public int? FrameID { get; set; }    // FK -> SensorFrame (optional)
        public int AuthorUserID { get; set; } // FK -> User

        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public bool IsClinicianReply { get; set; }
    }
}
