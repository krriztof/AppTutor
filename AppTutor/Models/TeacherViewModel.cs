namespace AppTutor.Models
{
    public class TeacherViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsApproved { get; set; } = false;
        public string DocumentPath { get; set; }

    }
}
