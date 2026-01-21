using AttendanceV3.Data;
using AttendanceV3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceV3.Controllers
{
    public class StudentWorkflowController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentWorkflowController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // QR SCAN ENTRY POINT
        // GET: /StudentWorkflow/Scan?sessionCode=XXXX
        // =========================
        [HttpGet]
        public async Task<IActionResult> Scan(string sessionCode)
        {
            if (string.IsNullOrWhiteSpace(sessionCode))
                return BadRequest("Session code missing.");

            // Find active session
            var session = await _context.AttendanceSessions
                .FirstOrDefaultAsync(s => s.SessionCode == sessionCode && s.EndTime == null);

            if (session == null)
                return View("InvalidSession");

            // Retrieve or generate device ID
            string deviceId = Request.Cookies["DEVICE_ID"];
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Response.Cookies.Append("DEVICE_ID", deviceId, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(5),
                    IsEssential = true
                });
            }

            // Check if device is already registered
            var device = await _context.StudentDevices
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId && d.IsActive);

            ViewBag.SessionId = session.Id;
            ViewBag.Subject = session.Subject;
            ViewBag.ClassSection = session.ClassSection;
            ViewBag.YearLevel = session.YearLevel;
            ViewBag.DeviceId = deviceId;

            // -----------------------
            // EXISTING DEVICE ? auto attendance
            // -----------------------
            if (device != null)
            {
                bool already = await _context.AttendanceRecord
                    .AnyAsync(r => r.AttendanceSessionId == session.Id && r.DeviceId == deviceId);

                if (!already)
                {
                    var record = new AttendanceRecord
                    {
                        AttendanceSessionId = session.Id,
                        DeviceId = device.DeviceId,
                        StudentName = device.StudentName,
                        StudentEmail = device.StudentEmail,
                        TimeIn = DateTime.Now
                    };
                    _context.AttendanceRecord.Add(record);
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Attendance recorded successfully.";
                return RedirectToAction("Dashboard");
            }

            // -----------------------
            // NEW DEVICE ? registration page
            // -----------------------
            return View("RegisterStudent");
        }

        // =========================
        // REGISTER DEVICE (POST)
        // =========================
        [HttpPost]
        public async Task<IActionResult> RegisterStudent(
            int sessionId,
            string deviceId,
            string fullName,
            string studentEmail)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(deviceId) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(studentEmail))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View("RegisterStudent");
            }

            // Prevent duplicate device registration
            var existingDevice = await _context.StudentDevices
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

            if (existingDevice != null)
            {
                TempData["Message"] = "This device is already registered.";
                return RedirectToAction("Dashboard");
            }

            // Save device info
            var device = new StudentDevices
            {
                DeviceId = deviceId,
                StudentName = fullName,
                StudentEmail = studentEmail,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.StudentDevices.Add(device);

            // Record initial attendance
            var record = new AttendanceRecord
            {
                AttendanceSessionId = sessionId,
                DeviceId = deviceId,
                StudentName = fullName,
                StudentEmail = studentEmail,
                TimeIn = DateTime.UtcNow
            };
            _context.AttendanceRecord.Add(record);

            await _context.SaveChangesAsync();

            TempData["Message"] = "Thank you! You are registered and attendance is recorded.";
            return RedirectToAction("Dashboard");
        }

        // =========================
        // STUDENT DASHBOARD
        // =========================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            string deviceId = Request.Cookies["DEVICE_ID"];

            if (string.IsNullOrEmpty(deviceId))
            {
                TempData["Message"] = "Device not recognized. Please scan QR code first.";
                return RedirectToAction("Scan");
            }

            var records = await _context.AttendanceRecord
                .Include(r => r.AttendanceSession)
                .Where(r => r.DeviceId == deviceId)
                .OrderByDescending(r => r.TimeIn)
                .ToListAsync();

            return View("Dashboard", records);
        }

        // =========================
        // VIEW RESULT PAGE
        // =========================
        [HttpGet]
        public IActionResult Result()
        {
            ViewBag.Message = TempData["Message"];
            return View("StudentSuccess");
        }

        // =========================
        // HELPER: Check if device already attended a session
        // =========================
        private async Task<bool> AlreadyAttended(int sessionId, string deviceId)
        {
            return await _context.AttendanceRecord
                .AnyAsync(r => r.AttendanceSessionId == sessionId && r.DeviceId == deviceId);
        }
    }
}
