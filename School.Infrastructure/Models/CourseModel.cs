namespace School.Infrastructure.Models;

public class CourseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }

    // Зовнішній ключ для Teacher
    public Guid TeacherId { get; set; }

    // Навігаційні властивості
    
    // Зв'язок багато-до-одного: Course належить одному Teacher
    public TeacherModel Teacher { get; set; } = null!;

    // Зв'язок один-до-багатьох: Course має багато оцінок
    public ICollection<GradeModel> Grades { get; set; } = new List<GradeModel>();

    // Зв'язок багато-до-багатьох: Course має багато студентів
    public ICollection<StudentCourseModel> StudentCourses { get; set; } = new List<StudentCourseModel>();
}

