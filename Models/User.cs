using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Learning_Platform.Models
{
    public class User
    {
        [Key]
        [Column(TypeName ="varchar(50)")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Column(TypeName ="varchar(50)")]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(250)")]
        public string? UserImage { get; set; } = null;

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string UserEmail { get; set; } = string.Empty;

        [Column(TypeName = "varchar(150)")]
        [Required]
        public string UserPassword { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime UserBirthday { get; set; } = default;

        //the relation between the course and the user
        public ICollection<UserCourse>? UserCourses { get; set; }

    }
}
