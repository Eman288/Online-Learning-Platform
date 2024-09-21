using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class Course
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int CourseId { get; set; }

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


       


        //relation between the course and the lessons

        public ICollection<Lesson>? Lessons { get; set; }

        //the relation between the course and the user
        public ICollection<UserCourse>? UserCourses { get; set; }
    }
}
