namespace AttendanceV3.Models
{
    public class RegisterStudentViewModel
    {
        public string SessionCode { get; set; } = string.Empty; // from QR
        public string StudentCode { get; set; } = string.Empty; // generated or scanned
        public string StudentName { get; set; } = string.Empty; // entered by student
    }
}
