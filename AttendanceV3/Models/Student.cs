
namespace AttendanceV3.Models
    {
        public class Student
        {
            public int Id { get; set; }
            public string StudentCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
            public string FullName { get; set; }
            public string YearLevel { get; set; }
            public string ClassSection { get; set; }

            public int StudentId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;

    }
    }


