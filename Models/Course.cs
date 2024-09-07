using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class Course
    {
        [Required]
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string CourseId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName ="varchar(50)")]
        public string CourseName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string CourseDescription { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string CourseImage { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CourseCreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime CourseUpdatedAt { get; set; } = DateTime.Now;

        //relation between the course and the course provider

        [Required]
        [ForeignKey("CourseProvider")]
        [Column(TypeName = "varchar(50)")] 
        public string CourseProviderId { get; set; } = string.Empty;

        // Navigation property
        public CourseProvider? CourseProvider { get; set; }


        //relation between the course and the lessons

        public ICollection<Lesson>? Lessons { get; set; }

        //the relation between the course and the user
        public ICollection<UserCourse>? UserCourses { get; set; }
    }
}
