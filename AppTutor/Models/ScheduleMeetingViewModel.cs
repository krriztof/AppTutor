namespace AppTutor.Models
{
    public class ScheduleMeetingViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string TeacherId { get; set; }
        public List<int> UnavailableHours { get; set; }
    }
}
