using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class Clinician
    {
        [Key]                      // ✅ primary key
        public int ClinicianID { get; set; }

        public int UserID { get; set; }   // FK -> User
        public string LicenseNumber { get; set; }
        public string Speciality { get; set; }
    }
}
