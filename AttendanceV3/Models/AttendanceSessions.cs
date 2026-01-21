using System.ComponentModel.DataAnnotations;

namespace AttendanceV3.Models
{
    public class AttendanceSessions
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string ClassSection { get; set; } = string.Empty;

        [Required]
        public string YearLevel { get; set; } = string.Empty;

        [Required]
        public string SessionCode { get; set; } = string.Empty;

        [Required]
        public string TeacherEmail { get; set; } = string.Empty;

        public string? DeviceId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
      
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
            = new List<AttendanceRecord>();
    }
}
