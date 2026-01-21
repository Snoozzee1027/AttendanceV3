using Microsoft.AspNetCore.SignalR;

namespace AttendanceV3.Hubs
{
    public class AttendanceHub : Hub
    {
        // Send scan count to all clients
        public async Task SendScanCount(int sessionId, int count)
        {
            await Clients.All.SendAsync("ReceiveScanCount", sessionId, count);
        }

        // Optionally send new student names
        public async Task SendStudentName(int sessionId, string studentName)
        {
            await Clients.All.SendAsync("ReceiveStudentName", sessionId, studentName);
        }
    }
}
