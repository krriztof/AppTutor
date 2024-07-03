using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppTutor.Models;
using AppTutor.Data;
using Microsoft.AspNetCore.Identity;


namespace AppTutor.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string specialization)
        {
            var teachersQuery = _context.Teachers.AsQueryable();

            if (!string.IsNullOrEmpty(specialization))
            {
                teachersQuery = teachersQuery.Where(t => t.Specialization == specialization);
            }

            var teachers = await teachersQuery
                .Where(t => t.IsApproved) 
                .Include(t => t.User)
                .Select(t => new TeacherViewModel
                {
                    Id = t.Id,
                    Name = t.User.FirstName + " " + t.User.LastName,
                    Specialization = t.Specialization
                    
                })
                .ToListAsync();

            return View(teachers);
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleMeeting(string teacherId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher == null)
            {
                return NotFound();
            }

            var teacherViewModel = new TeacherViewModel
            {
                Id = teacher.Id,
                Name = teacher.User.FirstName + " " + teacher.User.LastName,
                Specialization = teacher.Specialization
                
            };

           
            return View(teacherViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleMeeting(string teacherId, DateTime sessionDate, string sessionTime)
        {
            
            bool hasErrors = false;

            
            if (sessionDate.Date < DateTime.Now.AddDays(1).Date)
            {
                TempData["ErrorMessage"] = "Nie można umawiać spotkań na dzisiejszy dzień.";
                hasErrors = true;
            }

            
            if (sessionDate.DayOfWeek == DayOfWeek.Saturday || sessionDate.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["ErrorMessage"] = "Spotkania można umawiać tylko w dni robocze.";
                hasErrors = true;
            }

            if (!hasErrors && TimeSpan.TryParse(sessionTime, out TimeSpan time))
            {
                var sessionDateTime = sessionDate.Add(time);
                if (!IsTeacherAvailable(teacherId, sessionDateTime))
                {
                    TempData["ErrorMessage"] = "Nauczyciel nie jest dostępny w wybranym terminie.";
                    hasErrors = true;
                }
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {

                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (student != null)
                    {
                        var tutorSession = new TutorSession
                        {
                            TeacherId = teacherId,
                            StudentId = student.Id,
                            SessionDateTime = sessionDateTime,
                            IsScheduled = false
                        };

                        _context.TutorSessions.Add(tutorSession);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Spotkanie zostało pomyślnie umówione!";
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Nieprawidłowy format godziny.";
                hasErrors = true;
            }


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MyMeetings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null)
            {
                
                return NotFound();
            }

            var studentId = user.Student.Id; 
            var myMeetings = await _context.TutorSessions
                .Include(ts => ts.Teacher)
                .ThenInclude(t => t.User)
                .Where(ts => ts.StudentId == studentId)
                .Select(ts => new TutorSessionViewModel
                {
                    Id = ts.Id,
                    TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName,
                    SessionDateTime = ts.SessionDateTime,
                    IsScheduled = ts.IsScheduled,
                    TeacherPhoneNumber = ts.Teacher.User.PhoneNumber,
                    TeacherEmail = ts.Teacher.User.Email
                   
                })
                .ToListAsync();

            return View(myMeetings);
        }

        private bool IsTeacherAvailable(string teacherId, DateTime sessionDateTime)
        {
            var sessionEndTime = sessionDateTime.AddMinutes(90);

            return !_context.TutorSessions.Any(ts =>
                ts.TeacherId == teacherId &&
                (
                    (ts.SessionDateTime <= sessionDateTime && ts.SessionDateTime.AddMinutes(90) > sessionDateTime) ||
                    (ts.SessionDateTime < sessionEndTime && ts.SessionDateTime.AddMinutes(90) >= sessionEndTime) ||
                    (ts.SessionDateTime >= sessionDateTime && ts.SessionDateTime.AddMinutes(90) <= sessionEndTime)
                )
            );
        }




    }




}
