using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppTutor.Models
{
    public class TutorSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string TeacherId { get; set; }
        public string StudentId { get; set; }

        [Display(Name = "Data i godzina zajęć")]
        [DataType(DataType.DateTime)]
        public DateTime SessionDateTime { get; set; }

        [Display(Name = "Czy spotkanie zaplanowane?")]
        public bool IsScheduled { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual Student Student { get; set; }

    }
}
