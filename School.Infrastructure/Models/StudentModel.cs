namespace School.Infrastructure.Models;

// StudentModel наслідує PersonModel (Table-per-Type)
public class StudentModel : PersonModel
{
    public string StudentNumber { get; set; } = string.Empty;
    public int Year { get; set; }
    public double GPA { get; set; }

    // Навігаційні властивості
    
    // Зв'язок один-до-одного: Student має один StudentDetails (якщо існує)
    public StudentDetailsModel? StudentDetails { get; set; }

    // Зв'язок один-до-багатьох: Student має багато оцінок
    public ICollection<GradeModel> Grades { get; set; } = new List<GradeModel>();

    // Зв'язок багато-до-багатьох: Student записаний на багато курсів
    public ICollection<StudentCourseModel> StudentCourses { get; set; } = new List<StudentCourseModel>();
}

