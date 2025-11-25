using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class Patient
    {
        [Key]                      // ✅ primary key
        public int PatientID { get; set; }

        public int UserID { get; set; }   // FK -> User

        public DateTime DateOfBirth { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string MedicalNotes { get; set; }
    }
}
