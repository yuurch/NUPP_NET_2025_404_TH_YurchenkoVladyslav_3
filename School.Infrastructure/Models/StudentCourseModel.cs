namespace School.Infrastructure.Models;

// Проміжна таблиця для багато-до-багатьох зв'язку між Student та Course
public class StudentCourseModel
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }

    // Навігаційні властивості
    public StudentModel Student { get; set; } = null!;
    public CourseModel Course { get; set; } = null!;
}

