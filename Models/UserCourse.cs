using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class UserCourse
    {
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string? UserId { get; set; }

        // Navigation property
        public User? User { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string? CourseId { get; set; }

        // Navigation property
        public Course? Course { get; set; }
    }
}
