using AttendanceV3.Data;
using AttendanceV3.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/* ===============================
   LAN + HTTP ONLY (NO SSL)
   =============================== */
builder.WebHost.UseUrls("http://0.0.0.0:5000");

/* ===============================
   DATABASE (SQLite)
   =============================== */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=AttendanceV3.db"));

/* ===============================
   IDENTITY (Teacher + Student)
   =============================== */
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

/* ===============================
   MVC + AUTH
   =============================== */
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

/* ===============================
   SIGNALR
   =============================== */
builder.Services.AddSignalR();

/* ===============================
   AUTH COOKIE PATHS
   =============================== */
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();



/* ===============================
   MIDDLEWARE
   =============================== */
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

/* ===============================
   ROUTING
   =============================== */
app.MapControllerRoute(
    name: "login",
    pattern: "",
    defaults: new { controller = "Account", action = "Login" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();                 // REQUIRED for Identity UI

// -----------------------------
// SIGNALR HUB MAPPING
// -----------------------------
app.MapHub<AttendanceHub>("/attendanceHub");

/* ===============================
   SEED ROLES + USERS
   =============================== */
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Teacher role
    if (!await roleManager.RoleExistsAsync("Teacher"))
        await roleManager.CreateAsync(new IdentityRole("Teacher"));

    // Student role
    if (!await roleManager.RoleExistsAsync("Student"))
        await roleManager.CreateAsync(new IdentityRole("Student"));

    // Teacher account
    var teacherEmail = "teacher@school.com";
    var teacher = await userManager.FindByEmailAsync(teacherEmail);
    if (teacher == null)
    {
        teacher = new IdentityUser
        {
            UserName = teacherEmail,
            Email = teacherEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(teacher, "Teacher123!");
        await userManager.AddToRoleAsync(teacher, "Teacher");
    }

    // Student account
    var studentEmail = "student1@school.com";
    var student = await userManager.FindByEmailAsync(studentEmail);
    if (student == null)
    {
        student = new IdentityUser
        {
            UserName = studentEmail,
            Email = studentEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(student, "Student123!");
        await userManager.AddToRoleAsync(student, "Student");
    }
}

app.Run();

// Reset Teacher Account - TEMPORARY
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string teacherEmail = "teacher@school.com";
    string teacherPassword = "Teacher123!";

    // 1. Ensure Teacher role exists
    if (!await roleManager.RoleExistsAsync("Teacher"))
    {
        await roleManager.CreateAsync(new IdentityRole("Teacher"));
    }

    // 2. Find the teacher account
    var teacher = await userManager.FindByEmailAsync(teacherEmail);

    if (teacher == null)
    {
        // If teacher doesn't exist, create it
        teacher = new IdentityUser { UserName = teacherEmail, Email = teacherEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(teacher, teacherPassword);
        if (!result.Succeeded) throw new Exception("Failed to create teacher: " + string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    else
    {
        // Reset password
        var token = await userManager.GeneratePasswordResetTokenAsync(teacher);
        var result = await userManager.ResetPasswordAsync(teacher, token, teacherPassword);
        if (!result.Succeeded) throw new Exception("Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // 3. Ensure Teacher role is assigned
    if (!await userManager.IsInRoleAsync(teacher, "Teacher"))
    {
        await userManager.AddToRoleAsync(teacher, "Teacher");
    }
}