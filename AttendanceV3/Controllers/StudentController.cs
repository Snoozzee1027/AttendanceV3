using AttendanceV3.Data;
using AttendanceV3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AttendanceV3.Hubs;

namespace AttendanceV3.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<AttendanceHub> _hub;

        // Single constructor with both dependencies
        public StudentController(ApplicationDbContext context, IHubContext<AttendanceHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // =========================
        // QR SCAN ENTRY POINT
        // =========================
        [HttpGet]
        public async Task<IActionResult> Scan(string sessionCode)
        {
            if (string.IsNullOrWhiteSpace(sessionCode))
                return BadRequest("Session code missing.");

            // Find the active session
            var session = await _context.AttendanceSessions
                .FirstOrDefaultAsync(s => s.SessionCode == sessionCode && s.EndTime == null);

            if (session == null)
                return View("SessionClosed");

            // Get or create device ID from cookie
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

            ViewBag.SessionId = session.Id;
            ViewBag.Subject = session.Subject;
            ViewBag.ClassSection = session.ClassSection;
            ViewBag.YearLevel = session.YearLevel;
            ViewBag.DeviceId = deviceId;

            // ----------------------------
            // Check if this device already recorded attendance for this session
            // ----------------------------
            var alreadyRecorded = await _context.AttendanceRecord
                .AnyAsync(r => r.AttendanceSessionId == session.Id && r.DeviceId == deviceId);

            if (alreadyRecorded)
            {
                return View("~/Views/Attendance/AlreadyRecorded.cshtml");
            }

            // ----------------------------
            // Check if device exists globally (registered before)
            // ----------------------------
            var device = await _context.StudentDevices
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

            if (device != null)
            {
                // Auto-attend for already registered device
                var record = new AttendanceRecord
                {
                    AttendanceSessionId = session.Id,
                    DeviceId = device.DeviceId,
                    StudentName = device.StudentName,
                    StudentEmail = device.StudentEmail,
                    StudentToken = device.StudentToken,
                    TimeIn = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };
                _context.AttendanceRecord.Add(record);
                await _context.SaveChangesAsync();

                // SignalR: update live student list & count
                await _hub.Clients.All.SendAsync("ReceiveScanCount", session.Id,
                    await _context.AttendanceRecord.CountAsync(r => r.AttendanceSessionId == session.Id));
                await _hub.Clients.All.SendAsync("ReceiveStudentName", session.Id, device.StudentName);

                return RedirectToAction("Dashboard");
            }

            // If device is new → show registration form
            return View("RegisterStudent");
        }

        // =========================
        // REGISTER DEVICE (POST)
        // =========================
        [HttpPost]
        public async Task<IActionResult> RegisterStudent(int sessionId, string deviceId, string fullName, string studentEmail)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(studentEmail))
            {
                ModelState.AddModelError("", "Full name and email are required.");
                return RedirectToAction("Scan", new { sessionCode = sessionId });
            }

            // Get or create device record
            var existingDevice = await _context.StudentDevices
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

            string studentToken;

            if (existingDevice != null)
            {
                // Device already exists → update info
                studentToken = existingDevice.StudentToken;
                existingDevice.StudentName = fullName;
                existingDevice.StudentEmail = studentEmail;
                existingDevice.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            else
            {
                // New device → create record
                studentToken = Guid.NewGuid().ToString();
                var newDevice = new StudentDevices
                {
                    DeviceId = deviceId,
                    StudentName = fullName,
                    StudentEmail = studentEmail,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    StudentToken = studentToken,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };
                _context.StudentDevices.Add(newDevice);
            }

            // Record attendance for this session if not already done
            var already = await _context.AttendanceRecord
                .AnyAsync(r => r.AttendanceSessionId == sessionId && r.DeviceId == deviceId);

            if (!already)
            {
                var record = new AttendanceRecord
                {
                    DeviceId = deviceId,
                    StudentName = fullName,
                    StudentEmail = studentEmail,
                    AttendanceSessionId = sessionId,
                    TimeIn = DateTime.Now,
                    StudentToken = studentToken,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };
                _context.AttendanceRecord.Add(record);
            }

            await _context.SaveChangesAsync();

            // SignalR: update live student list & count
            await _hub.Clients.All.SendAsync("ReceiveScanCount", sessionId,
                await _context.AttendanceRecord.CountAsync(r => r.AttendanceSessionId == sessionId));
            await _hub.Clients.All.SendAsync("ReceiveStudentName", sessionId, fullName);

            // Ensure cookie exists for Dashboard
            if (string.IsNullOrEmpty(Request.Cookies["DEVICE_ID"]))
            {
                Response.Cookies.Append("DEVICE_ID", deviceId, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(5),
                    IsEssential = true
                });
            }

            return RedirectToAction("Dashboard");
        }

        // =========================
        // VIEW ALL REGISTERED STUDENTS
        // =========================
        [HttpGet]
        public async Task<IActionResult> UnenrollStudent()
        {
            var students = await _context.StudentDevices
                .OrderBy(s => s.StudentName)
                .ToListAsync();

            return View(students); // Returns UnenrollStudent.cshtml
        }

        // =========================
        // DELETE/UNENROLL STUDENT
        // =========================
        [HttpPost]
        public async Task<IActionResult> DeleteStudent(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                return BadRequest("Device ID missing.");

            var student = await _context.StudentDevices
                .FirstOrDefaultAsync(s => s.DeviceId == deviceId);

            if (student != null)
            {
                _context.StudentDevices.Remove(student);

                var attendanceRecords = _context.AttendanceRecord
                    .Where(r => r.DeviceId == deviceId);
                _context.AttendanceRecord.RemoveRange(attendanceRecords);

                await _context.SaveChangesAsync();
            }

            TempData["Message"] = $"Student {student?.StudentName} has been unenrolled.";
            return RedirectToAction("UnenrollStudent");
        }

        // =========================
        // STUDENT DASHBOARD
        // =========================
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

            return View(records);
        }
    }
}
