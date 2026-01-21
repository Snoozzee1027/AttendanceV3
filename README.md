====================================================
                 AttendanceV3
====================================================

AttendanceV3 is a web-based attendance management system built with:
- ASP.NET Core MVC
- Entity Framework Core
- SQLite

It allows teachers to create and manage attendance sessions, 
scan student QR codes, track attendance, export CSVs, and 
manage student devices.

----------------------------------------------------
REQUIREMENTS
----------------------------------------------------
- Windows 10/11 or macOS/Linux
- .NET 7 SDK (https://dotnet.microsoft.com/download/dotnet/7.0)
- Optional: Visual Studio 2022 / VS Code
- Git
- PowerShell

----------------------------------------------------
OPENING POWERSHELL
----------------------------------------------------
1. Press Win+R
2. Type "powershell" and hit Enter
3. You now have a PowerShell window to run commands

----------------------------------------------------
INSTALLATION & RUNNING THE PROJECT
----------------------------------------------------
Step 1: Clone the repository (replace YOUR_GITHUB_URL with the repo URL)
    git clone YOUR_GITHUB_URL
    cd AttendanceV3

Step 2: Restore NuGet dependencies
    dotnet restore

Step 3: Apply database migrations
    dotnet ef database update

    Note: If "dotnet ef" is not recognized:
        dotnet tool install --global dotnet-ef

Step 4: Run the project
    dotnet run --project AttendanceV3.csproj

Step 5: Open your browser
    Visit:
        http://localhost:5000
        OR
        https://localhost:5001
        
        IF NEITHER WORKS USE THIS!
        ===============================================================================
        =http://localhost:5000/Identity/Account/Login?ReturnUrl=%2FTeacher%2FDashboard 
        ===============================================================================
----------------------------------------------------
DEFAULT TEACHER ACCOUNT
----------------------------------------------------
Email: teacher@school.com
Password: Teacher123!

----------------------------------------------------
TEACHER MANUAL
----------------------------------------------------
1. LOGIN
   - Open the browser at localhost URL
   - Enter the teacher credentials above

2. DASHBOARD
   - Create Attendance Session
       * Enter Subject, Class Section, Year Level
       * Click "Start Attendance"
   - Session Actions:
       * Open QR: Show QR code for student check-in
       * Stop QR: Close session
       * View Students: See who attended
       * Export CSV: Download attendance records
       * Delete: Remove session completely

3. MANAGE STUDENTS
   - Click "Unenroll Students" to remove devices

4. CHANGE CREDENTIALS
   - Button located top-right near your email
   - Update email and password securely

5. LOGOUT
   - Button top-right next to Change Credentials

----------------------------------------------------
STUDENT MANUAL
----------------------------------------------------
1. ACCESS
   - Students do not log in
   - Use session code or QR code to check in

2. CHECK-IN
   - Open student page URL provided by teacher
   - Scan QR code or enter session code
   - Attendance is recorded automatically

----------------------------------------------------
GIT USAGE (OPTIONAL)
----------------------------------------------------
Add files to Git:
    git add .

Commit changes:
    git commit -m "Your commit message"

Push to GitHub:
    git push origin master

----------------------------------------------------
NOTES
----------------------------------------------------
- Database file: AttendanceV3.db (SQLite)
- .vs/, bin/, obj/ folders are ignored in Git
- Only teacher accounts are supported (no admin account)

====================================================
