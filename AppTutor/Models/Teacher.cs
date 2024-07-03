using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppTutor.Models
{


    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Specialization { get; set; }
        public bool IsApproved { get; set; } = false;
        public string DocumentPath { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<TutorSession> TutorSessions { get; set; }
        
    }


}
