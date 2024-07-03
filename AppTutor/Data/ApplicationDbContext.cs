using AppTutor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppTutor.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TutorSession> TutorSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>()
                .HasOne<ApplicationUser>(s => s.User)
                .WithOne(au => au.Student)
                .HasForeignKey<Student>(s => s.UserId);

            builder.Entity<Teacher>()
                .HasOne<ApplicationUser>(t => t.User)
                .WithOne(au => au.Teacher)
                .HasForeignKey<Teacher>(t => t.UserId);

            builder.Entity<TutorSession>()
                .HasOne<Teacher>(ts => ts.Teacher)
                .WithMany(t => t.TutorSessions)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TutorSession>()
                .HasOne<Student>(ts => ts.Student)
                .WithMany(s => s.TutorSessions)
                .HasForeignKey(ts => ts.StudentId)
                .OnDelete(DeleteBehavior.Cascade); 



            builder.Entity<Student>().ToTable("Students");
            builder.Entity<Teacher>().ToTable("Teachers");
            builder.Entity<TutorSession>().ToTable("TutorSessions");



        }
    }
}