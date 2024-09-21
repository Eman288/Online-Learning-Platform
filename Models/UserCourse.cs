using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class UserCourse
    {
        [Column(TypeName = "int")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? UserId { get; set; }

        // Navigation property
        public User? User { get; set; }

        [Column(TypeName = "int")]
        [Required]
        public int? CourseId { get; set; }

        // Navigation property
        public Course? Course { get; set; }
    }
}
