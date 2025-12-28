namespace School.Infrastructure.Models;

// TeacherModel наслідує PersonModel (Table-per-Type)
public class TeacherModel : PersonModel
{
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }

    // Навігаційні властивості
    
    // Зв'язок один-до-багатьох: Teacher веде багато курсів
    public ICollection<CourseModel> Courses { get; set; } = new List<CourseModel>();
}

