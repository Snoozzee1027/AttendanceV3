using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceV3.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentCode { get; set; } = string.Empty;

        public DateTime TimeIn { get; set; } = DateTime.Now;

        public string? IpAddress { get; set; }

        [Required]
        public string StudentName { get; set; } = string.Empty;

        public string? StudentEmail { get; set; } // optional

        public string? DeviceId { get; set; } // optional

        [Required]
        public string DeviceFingerprint { get; set; } = string.Empty; // required to prevent duplicates
        public int SessionId { get; set; }
        public string StudentToken { get; set; } = null!;
     

        [Required]
        public int AttendanceSessionId { get; set; }



        public string? Password { get; set; } // optional for uniqueness login

        [Required]
 
        public AttendanceSessions AttendanceSession { get; set; } = null!;
    }
}
