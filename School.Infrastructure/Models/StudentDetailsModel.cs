namespace School.Infrastructure.Models;

// Модель для демонстрації зв'язку один-до-одного
public class StudentDetailsModel
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmergencyContact { get; set; } = string.Empty;

    // Зовнішній ключ для Student (один-до-одного)
    public Guid StudentId { get; set; }

    // Навігаційна властивість
    public StudentModel Student { get; set; } = null!;
}

