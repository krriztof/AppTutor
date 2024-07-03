namespace AppTutor.Models
{
    public class TutorSessionViewModel
    {
        public string Id { get; set; }
        public string StudentName { get; set; }
        public string StudentPhoneNumber { get; set; } 
        public string StudentEmail { get; set; }
        public DateTime SessionDateTime { get; set; }
        public string TeacherName { get; set; }
        public string TeacherPhoneNumber { get; set; }
        public string TeacherEmail {  get; set; }
        public bool IsScheduled { get; set; }

        public StudentViewModel Student { get; set; }
  

    }

}
