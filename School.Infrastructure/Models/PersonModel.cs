namespace School.Infrastructure.Models;

// Базовий клас для Person - використовуємо Table-per-Type (TPT)
public class PersonModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

