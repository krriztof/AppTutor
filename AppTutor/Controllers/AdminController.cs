using AppTutor.Data;
using AppTutor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var unapprovedTeachers = await _context.Teachers
            .Include(t => t.User)
            .Where(t => !t.IsApproved)
            .Select(t => new TeacherViewModel
            {
                Id = t.Id,
                Email = t.User.Email,
                FirstName = t.User.FirstName,
                LastName = t.User.LastName,
                PhoneNumber = t.User.PhoneNumber,
                DateOfBirth = t.User.DateOfBirth,
                Specialization = t.Specialization,
                DocumentPath = t.DocumentPath
            }).ToListAsync();

        var scheduledSessions = await _context.TutorSessions
            .Include(ts => ts.Teacher)
                .ThenInclude(t => t.User)
            .Include(ts => ts.Student)
                .ThenInclude(s => s.User)
            .Where(ts => ts.IsScheduled)
            .Select(ts => new TutorSessionViewModel
            {
                TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName,
                StudentName = ts.Student.User.FirstName + " " + ts.Student.User.LastName,
                SessionDateTime = ts.SessionDateTime
            }).ToListAsync();

        var model = new AdminViewModel
        {
            UnapprovedTeachers = unapprovedTeachers,
            ScheduledSessions = scheduledSessions
        };

        return View(model);
    }

    public async Task<IActionResult> ApproveTeacher(string teacherId)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == teacherId);

        if (teacher != null)
        {
            teacher.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RejectTeacher(string teacherId)
    {
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == teacherId);

        if (teacher != null)
        {
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Nauczyciel został odrzucony i usunięty z systemu.";
        }
        else
        {
            TempData["ErrorMessage"] = "Nie znaleziono nauczyciela do odrzucenia.";
        }

        return RedirectToAction(nameof(Index));
    }
}
