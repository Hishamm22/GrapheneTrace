using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class Patient
    {
        public int PatientID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(50)]
        public string? EmergencyContactNumber { get; set; }

        public string? MedicalNotes { get; set; }

        // Navigation
        public User User { get; set; }
        public ICollection<SensorSession> Sessions { get; set; } = new List<SensorSession>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
