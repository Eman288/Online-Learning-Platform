using Microsoft.EntityFrameworkCore;
using Online_Learning_Platform.Models;


namespace Online_Learning_Platform.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<CourseProvider> CourseProviders { get; set; }
        public DbSet<Lesson> Lessons { get; set; }



        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary key for UserCourse
            modelBuilder.Entity<UserCourse>()
                .HasKey(uc => new { uc.UserId, uc.CourseId });

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            // Configure Course entity
            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);

            modelBuilder.Entity<Course>()
                .Property(c => c.CourseName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Course>()
                .Property(c => c.CourseDescription)
                .HasMaxLength(250);

            // Configure Course Provider entity
            modelBuilder.Entity<CourseProvider>()
                .HasKey(cp => cp.CourseProviderId);

            modelBuilder.Entity<CourseProvider>()
                .Property(cp => cp.CourseProviderName)
                .IsRequired()
                .HasMaxLength(50);

            // Configure Lesson entity
            modelBuilder.Entity<Lesson>()
                .HasKey(l => l.LessonId);

            // Configure relationships
            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCourses)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.UserCourses)
                .HasForeignKey(uc => uc.CourseId);
        }
    }
}
