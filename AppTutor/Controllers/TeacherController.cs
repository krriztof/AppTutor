using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppTutor.Models; 
using AppTutor.Data;

namespace AppTutor.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Błąd: Nie znaleziono identyfikatora użytkownika.");
            }

            var teacher = await _context.Teachers
                .Include(t => t.TutorSessions)
                    .ThenInclude(ts => ts.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("Błąd: Nie znaleziono nauczyciela.");
            }

            var meetingRequests = teacher.TutorSessions
                .Where(ts => !ts.IsScheduled)
                .Select(ts => new TutorSessionViewModel
                {
                    Id = ts.Id,
                    StudentName = ts.Student != null ? ts.Student.User.FirstName + " " + ts.Student.User.LastName : "Nieznany student",
                    SessionDateTime = ts.SessionDateTime,
                })
                .ToList(); 

            return View(meetingRequests);
        }





        public async Task<IActionResult> AcceptMeeting(string sessionId)
        {
            var tutorSession = await _context.TutorSessions.FindAsync(sessionId);
            if (tutorSession != null)
            {
                tutorSession.IsScheduled = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Spotkanie zostało pomyślnie zaplanowane!";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeclineMeeting(string sessionId)
        {
            var tutorSession = await _context.TutorSessions.FindAsync(sessionId);
            if (tutorSession != null)
            {
                _context.TutorSessions.Remove(tutorSession);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Prośba o spotkanie została odrzucona.";
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> MyMeetings()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Błąd: Nie znaleziono identyfikatora użytkownika.");
            }

            var teacher = await _context.Teachers
                .Include(t => t.TutorSessions)
                    .ThenInclude(ts => ts.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("Błąd: Nie znaleziono nauczyciela.");
            }

            var meetings = teacher.TutorSessions
                .Where(ts => ts.IsScheduled)
                .Select(ts => new TutorSessionViewModel
                {
                    Id = ts.Id,
                    StudentName = ts.Student.User.FirstName + " " + ts.Student.User.LastName,
                    SessionDateTime = ts.SessionDateTime,
                    StudentPhoneNumber = ts.Student.User.PhoneNumber,
                    StudentEmail = ts.Student.User.Email
                })
                .ToList();

            return View(meetings);
        }


    }
}
