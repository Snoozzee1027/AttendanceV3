using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceV3.Data;
using AttendanceV3.Models;

namespace AttendanceV3.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // QR SCAN ENTRY POINT
        // =========================
        [HttpGet("/scan/{code}")]
        public async Task<IActionResult> Scan(string code)
        {
            var session = await _context.AttendanceSessions
                .FirstOrDefaultAsync(s => s.SessionCode == code && s.EndTime == null);

            if (session == null)
                return View("InvalidSession");

            // identify device (simple + reliable)
            var deviceId = Request.Headers["User-Agent"].ToString();

            var existing = await _context.AttendanceRecord
                .FirstOrDefaultAsync(r =>
                    r.SessionId == session.Id &&
                    r.DeviceId == deviceId);

            if (existing != null)
            {
                return View("AlreadyRecorded");
            }

            // not registered → ask for name
            ViewBag.SessionId = session.Id;
            ViewBag.DeviceId = deviceId;

            return View("Register");
        }

        // =========================
        // REGISTER + RECORD ATTENDANCE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Register(int sessionId, string studentName, string deviceId)
        {
            var session = await _context.AttendanceSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound();

            var record = new AttendanceRecord
            {
                SessionId = sessionId,
                StudentName = studentName,
                DeviceId = deviceId,
                TimeIn = DateTime.Now
            };

            _context.AttendanceRecord.Add(record);
            await _context.SaveChangesAsync();

            return View("ThankYou");
        }
    }
}
