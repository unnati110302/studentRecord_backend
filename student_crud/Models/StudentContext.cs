using Microsoft.EntityFrameworkCore;
using student_crud.Models;

namespace student_crud.Data
{
    public class StudentContext :   DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {
            
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<StudentDAO> StudentDAOs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInput> UserS { get; set; }
        public DbSet<UserInput> users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<ClassSubjects> ClassSubjects { get; set; }
        public DbSet<CourseDuration> CourseDurations { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        public DbSet<TeacherSubjects> TeacherSubjects { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
        public DbSet<ClassSections> ClassSections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.Id);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

           /* modelBuilder.Entity<Class>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);*/

            /*modelBuilder.Entity<ClassSubjects>()
                .HasOne(c => c.Class)
                .WithMany()
                .HasForeignKey(c => c.ClassId)
                .OnDelete(DeleteBehavior.Cascade);*/

            modelBuilder.Entity<CourseDuration>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassStudent>()
                .HasOne(c => c.Class)
                .WithMany()
                .HasForeignKey(c => c.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassStudent>()
                .HasOne(cs => cs.Student)
                .WithMany()
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherSubjects>()
                .HasOne(cs => cs.Teacher)
                .WithMany()
                .HasForeignKey(cs => cs.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherSubjects>()
                .HasOne(cs => cs.Subject)
                .WithMany()
                .HasForeignKey(cs => cs.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Course)
                .WithMany()
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Student)
                .WithMany()
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<ClassSections>()
                .HasOne(cs => cs.Class)
                .WithMany()
                .HasForeignKey(cs => cs.ClassId)
                .OnDelete(DeleteBehavior.Cascade);*/




            base.OnModelCreating(modelBuilder);
        }
    }


}
