using AttendanceV3.Data;
using AttendanceV3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using AttendanceV3.Hubs;
using AttendanceV3.Helpers;
using Microsoft.AspNetCore.Identity;

namespace AttendanceV3.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<AttendanceHub> _hub;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TeacherController(ApplicationDbContext context,
                                 IHubContext<AttendanceHub> hub,
                                 UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hub = hub;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // =====================
        // DASHBOARD
        // =====================
        public async Task<IActionResult> Dashboard()
        {
            var sessions = await _context.AttendanceSessions
                .Include(s => s.AttendanceRecords)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // =====================
        // CHANGE CREDENTIALS
        // =====================
        [HttpGet]
        public async Task<IActionResult> ChangeCredentials()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ChangeCredentialsViewModel
            {
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeCredentials(ChangeCredentialsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Update Email
            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }

                // Optional: sync username with email
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);
            }

            // Update Password
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }

            TempData["Message"] = "Credentials updated successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        // =====================
        // CREATE SESSION
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSession(string subject, string classSection, string yearLevel)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(classSection) || string.IsNullOrWhiteSpace(yearLevel))
            {
                ModelState.AddModelError("", "All fields are required.");
                return RedirectToAction(nameof(Dashboard));
            }

            var session = new AttendanceSessions
            {
                Subject = subject,
                ClassSection = classSection,
                YearLevel = yearLevel,
                StartTime = DateTime.UtcNow,
                SessionCode = Guid.NewGuid().ToString("N")[..6].ToUpper(),
                TeacherEmail = User.Identity!.Name!,
                DeviceId = NetworkHelper.GetLocalIPAddress(),
            };

            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ScanQRCode), new { id = session.Id });
        }

        // =====================
        // VIEW STUDENTS
        // =====================
        public async Task<IActionResult> ViewStudents(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.AttendanceRecords)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            return View(session);
        }

        // =====================
        // SCAN / DISPLAY QR
        // =====================
        public async Task<IActionResult> ScanQRCode(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.AttendanceRecords)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            var qrUrl = $"http://{NetworkHelper.GetLocalIPAddress()}:5000/Student/Scan?sessionCode={session.SessionCode}";
            ViewBag.QRUrl = qrUrl;
            ViewBag.SessionCode = session.SessionCode;

            return View(session);
        }

        // =====================
        // DELETE SESSION
        // =====================
        [HttpPost]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.AttendanceRecords)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Dashboard");
            }

            if (session.AttendanceRecords != null && session.AttendanceRecords.Any())
            {
                _context.AttendanceRecord.RemoveRange(session.AttendanceRecords);
            }

            _context.AttendanceSessions.Remove(session);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Session deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        // =====================
        // UNENROLL STUDENTS PAGE
        // =====================
        public async Task<IActionResult> UnenrollStudent()
        {
            var students = await _context.StudentDevices
                .OrderBy(d => d.StudentName)
                .ToListAsync();
            return View(students);
        }

        // =====================
        // DELETE STUDENT DEVICE (UNENROLL)
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentDevice(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                return BadRequest("Device ID required.");

            var student = await _context.StudentDevices
                .FirstOrDefaultAsync(s => s.DeviceId == deviceId);

            if (student != null)
            {
                _context.StudentDevices.Remove(student);

                var records = _context.AttendanceRecord
                    .Where(r => r.DeviceId == deviceId);
                _context.AttendanceRecord.RemoveRange(records);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UnenrollStudent");
        }

        // =====================
        // REMOVE / UN-ENROLL STUDENT
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int recordId)
        {
            var record = await _context.AttendanceRecord
                .Include(r => r.AttendanceSession)
                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
                return NotFound();

            int sessionId = record.AttendanceSessionId;

            _context.AttendanceRecord.Remove(record);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewStudents", new { id = sessionId });
        }

        // =====================
        // CLOSE SESSION
        // =====================
        [HttpPost]
        public async Task<IActionResult> CloseSession(int id)
        {
            var session = await _context.AttendanceSessions.FindAsync(id);
            if (session == null) return NotFound();

            session.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        // =====================
        // EXPORT CSV
        // =====================
        public async Task<IActionResult> ExportCsv(int id)
        {
            var records = await _context.AttendanceRecord
                .Include(r => r.AttendanceSession)
                .Where(r => r.AttendanceSessionId == id)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("StudentCode,Name,Email,TimeIn,IP,Subject,YearLevel");

            foreach (var r in records)
            {
                sb.AppendLine($"{r.StudentCode},{r.StudentName},{r.StudentEmail},{r.TimeIn},{r.IpAddress},{r.AttendanceSession.Subject},{r.AttendanceSession.YearLevel}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "attendance.csv");
        }
    }
}
