namespace School.Infrastructure.Models;

public class GradeModel
{
    public Guid Id { get; set; }
    public int Score { get; set; }
    public DateTime DateAssigned { get; set; }

    // Зовнішні ключі
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }

    // Навігаційні властивості
    
    // Зв'язок багато-до-одного: Grade належить одному студенту
    public StudentModel Student { get; set; } = null!;

    // Зв'язок багато-до-одного: Grade належить одному курсу
    public CourseModel Course { get; set; } = null!;
}

