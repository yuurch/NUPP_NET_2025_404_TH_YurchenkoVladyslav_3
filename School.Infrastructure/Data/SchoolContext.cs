using Microsoft.EntityFrameworkCore;
using School.Infrastructure.Models;

namespace School.Infrastructure.Data;

public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
    {
    }

    // DbSet властивості
    public DbSet<PersonModel> Persons { get; set; }
    public DbSet<StudentModel> Students { get; set; }
    public DbSet<TeacherModel> Teachers { get; set; }
    public DbSet<CourseModel> Courses { get; set; }
    public DbSet<GradeModel> Grades { get; set; }
    public DbSet<StudentDetailsModel> StudentDetails { get; set; }
    public DbSet<StudentCourseModel> StudentCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================================================
        // Table-per-Type (TPT) конфігурація для ієрархії Person
        // ============================================================================
        
        // PersonModel - базова таблиця
        modelBuilder.Entity<PersonModel>(entity =>
        {
            entity.ToTable("Persons");
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(p => p.DateOfBirth)
                .IsRequired();
        });

        // StudentModel - окрема таблиця для студентів (TPT)
        modelBuilder.Entity<StudentModel>(entity =>
        {
            entity.ToTable("Students");
            
            entity.Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(s => s.Year)
                .IsRequired();
            
            entity.Property(s => s.GPA)
                .HasColumnType("decimal(3,2)");

            // Індекс для швидкого пошуку за номером студента
            entity.HasIndex(s => s.StudentNumber).IsUnique();
        });

        // TeacherModel - окрема таблиця для викладачів (TPT)
        modelBuilder.Entity<TeacherModel>(entity =>
        {
            entity.ToTable("Teachers");
            
            entity.Property(t => t.Department)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(t => t.Position)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(t => t.Salary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Індекс для пошуку за департаментом
            entity.HasIndex(t => t.Department);
        });

        // ============================================================================
        // Конфігурація CourseModel
        // ============================================================================
        
        modelBuilder.Entity<CourseModel>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(c => c.Credits)
                .IsRequired();

            // Зв'язок багато-до-одного: Course -> Teacher
            entity.HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Індекс для пошуку курсів по викладачу
            entity.HasIndex(c => c.TeacherId);
        });

        // ============================================================================
        // Конфігурація GradeModel
        // ============================================================================
        
        modelBuilder.Entity<GradeModel>(entity =>
        {
            entity.ToTable("Grades");
            entity.HasKey(g => g.Id);
            
            entity.Property(g => g.Score)
                .IsRequired();
            
            entity.Property(g => g.DateAssigned)
                .IsRequired();

            // Зв'язок багато-до-одного: Grade -> Student
            entity.HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Зв'язок багато-до-одного: Grade -> Course
            entity.HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Індекси для швидкого пошуку
            entity.HasIndex(g => g.StudentId);
            entity.HasIndex(g => g.CourseId);
            
            // Композитний індекс для унікальності: один студент не може мати дві оцінки за один курс
            entity.HasIndex(g => new { g.StudentId, g.CourseId }).IsUnique();
        });

        // ============================================================================
        // Конфігурація StudentDetailsModel (зв'язок один-до-одного)
        // ============================================================================
        
        modelBuilder.Entity<StudentDetailsModel>(entity =>
        {
            entity.ToTable("StudentDetails");
            entity.HasKey(sd => sd.Id);
            
            entity.Property(sd => sd.Address)
                .HasMaxLength(500);
            
            entity.Property(sd => sd.PhoneNumber)
                .HasMaxLength(20);
            
            entity.Property(sd => sd.Email)
                .HasMaxLength(100);
            
            entity.Property(sd => sd.EmergencyContact)
                .HasMaxLength(200);

            // Зв'язок один-до-одного: StudentDetails -> Student
            entity.HasOne(sd => sd.Student)
                .WithOne(s => s.StudentDetails)
                .HasForeignKey<StudentDetailsModel>(sd => sd.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Унікальний індекс для StudentId
            entity.HasIndex(sd => sd.StudentId).IsUnique();
        });

        // ============================================================================
        // Конфігурація StudentCourseModel (зв'язок багато-до-багатьох)
        // ============================================================================
        
        modelBuilder.Entity<StudentCourseModel>(entity =>
        {
            entity.ToTable("StudentCourses");
            
            // Композитний первинний ключ
            entity.HasKey(sc => new { sc.StudentId, sc.CourseId });
            
            entity.Property(sc => sc.EnrollmentDate)
                .IsRequired();
            
            entity.Property(sc => sc.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Зв'язок багато-до-багатьох: Student <-> Course
            entity.HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Індекси для швидкого пошуку
            entity.HasIndex(sc => sc.StudentId);
            entity.HasIndex(sc => sc.CourseId);
        });
    }
}

