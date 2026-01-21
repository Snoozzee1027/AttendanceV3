using System.ComponentModel.DataAnnotations;

namespace AttendanceV3.Models
{
    public class StudentDevices
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; } = string.Empty;

        [Required]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        public string StudentEmail { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public string StudentToken { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }
    }
}
