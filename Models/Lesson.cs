using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class Lesson
    {
        [Required]
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string LessonId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string LessonName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string LessonDescription { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string LessonVideo { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string LessonImage { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime LessonCreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime LessonUpdatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "bit")]
        public bool LessonIsDone { get; set; } = false;

        [Required]
        [ForeignKey("Course")]
        [Column(TypeName = "varchar(50)")]
        public string? CourseId { get; set; }

        //navagation
        public Course? Course { get; set; }
    }
}
