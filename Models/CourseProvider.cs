using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class CourseProvider
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string CourseProviderId { get; set; } = string.Empty;

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string CourseProviderName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string CourseProviderImage { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string CourseProviderDescription { get; set; } = string.Empty;

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string CourseProviderEmail { get; set; } = string.Empty;

        [Column(TypeName = "varchar(150)")]
        [Required]
        public string CourseProviderPassword { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime CourseProviderBirthday { get; set; } = default;

        // Navigation property for related courses (One-to-Many)
        public ICollection<Course>? Courses { get; set; }
    }
}
