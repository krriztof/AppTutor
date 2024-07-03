namespace AppTutor.Models
{
    public class AdminViewModel
    {
        public IEnumerable<TeacherViewModel> UnapprovedTeachers { get; set; }
        public IEnumerable<TutorSessionViewModel> ScheduledSessions { get; set; }
    }
}
