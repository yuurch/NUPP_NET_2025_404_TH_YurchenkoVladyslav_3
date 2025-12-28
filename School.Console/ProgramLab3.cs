using Microsoft.EntityFrameworkCore;
using School.Common;
using School.Infrastructure.Data;
using School.Infrastructure.Models;
using School.Infrastructure.Repositories;
using School.Infrastructure.Services;
using System.Diagnostics;

namespace School.Console;

public class ProgramLab3
{
    public static async Task Main(string[] args)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║    Лабораторна робота №3 - Entity Framework та Repository     ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        // ============================================================================
        // ІНІЦІАЛІЗАЦІЯ БД ТА РЕПОЗИТОРІЇВ
        // ============================================================================
        
        System.Console.WriteLine("📦 Ініціалізація бази даних...\n");

        var options = new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlite("Data Source=school.db")
            .Options;

        using var context = new SchoolContext(options);
        
        // Створення бази даних та застосування міграцій
        await context.Database.EnsureCreatedAsync();
        System.Console.WriteLine("✅ База даних створена/підключена\n");

        // Створення репозиторіїв
        var teacherRepository = new Repository<TeacherModel>(context);
        var studentRepository = new Repository<StudentModel>(context);
        var courseRepository = new Repository<CourseModel>(context);
        var gradeRepository = new Repository<GradeModel>(context);
        var studentDetailsRepository = new Repository<StudentDetailsModel>(context);

        // Створення CRUD сервісів
        var teacherService = new CrudServiceAsyncDb<TeacherModel>(
            teacherRepository, 
            t => t.Id);
        
        var studentService = new CrudServiceAsyncDb<StudentModel>(
            studentRepository, 
            s => s.Id);
        
        var courseService = new CrudServiceAsyncDb<CourseModel>(
            courseRepository, 
            c => c.Id);

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 1: Створення та збереження викладачів
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 1. СТВОРЕННЯ ТА ЗБЕРЕЖЕННЯ ВИКЛАДАЧІВ                         ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var stopwatch = Stopwatch.StartNew();

        var teachers = new List<TeacherModel>
        {
            new TeacherModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Іван",
                LastName = "Петренко",
                DateOfBirth = new DateTime(1975, 5, 15),
                Department = "Математика",
                Position = "Професор",
                Salary = 35000m
            },
            new TeacherModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Марія",
                LastName = "Коваленко",
                DateOfBirth = new DateTime(1980, 8, 22),
                Department = "Інформатика",
                Position = "Доцент",
                Salary = 28000m
            },
            new TeacherModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Петро",
                LastName = "Шевченко",
                DateOfBirth = new DateTime(1985, 3, 10),
                Department = "Фізика",
                Position = "Викладач",
                Salary = 22000m
            }
        };

        foreach (var teacher in teachers)
        {
            await teacherService.CreateAsync(teacher);
            System.Console.WriteLine($"✓ Створено викладача: {teacher.FirstName} {teacher.LastName} - {teacher.Department}");
        }

        stopwatch.Stop();
        System.Console.WriteLine($"\n⏱️  Час виконання: {stopwatch.ElapsedMilliseconds} мс\n");

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 2: Створення курсів (зв'язок один-до-багатьох з Teacher)
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 2. СТВОРЕННЯ КУРСІВ (зв'язок один-до-багатьох)                ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var courses = new List<CourseModel>
        {
            new CourseModel
            {
                Id = Guid.NewGuid(),
                Name = "Вища математика",
                Credits = 5,
                TeacherId = teachers[0].Id
            },
            new CourseModel
            {
                Id = Guid.NewGuid(),
                Name = "Програмування на C#",
                Credits = 4,
                TeacherId = teachers[1].Id
            },
            new CourseModel
            {
                Id = Guid.NewGuid(),
                Name = "Бази даних",
                Credits = 4,
                TeacherId = teachers[1].Id
            },
            new CourseModel
            {
                Id = Guid.NewGuid(),
                Name = "Фізика",
                Credits = 3,
                TeacherId = teachers[2].Id
            }
        };

        foreach (var course in courses)
        {
            await courseService.CreateAsync(course);
            var teacher = teachers.First(t => t.Id == course.TeacherId);
            System.Console.WriteLine($"✓ Створено курс: {course.Name} ({course.Credits} кредитів) - викладає {teacher.FirstName} {teacher.LastName}");
        }

        System.Console.WriteLine();

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 3: Створення студентів
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 3. СТВОРЕННЯ СТУДЕНТІВ                                        ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var students = new List<StudentModel>
        {
            new StudentModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Олександр",
                LastName = "Іваненко",
                DateOfBirth = new DateTime(2002, 6, 15),
                StudentNumber = "ST2024001",
                Year = 2,
                GPA = 4.5
            },
            new StudentModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Анна",
                LastName = "Мельник",
                DateOfBirth = new DateTime(2003, 9, 20),
                StudentNumber = "ST2024002",
                Year = 1,
                GPA = 4.8
            },
            new StudentModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Дмитро",
                LastName = "Ткаченко",
                DateOfBirth = new DateTime(2002, 1, 5),
                StudentNumber = "ST2024003",
                Year = 2,
                GPA = 4.2
            }
        };

        foreach (var student in students)
        {
            await studentService.CreateAsync(student);
            System.Console.WriteLine($"✓ Створено студента: {student.FirstName} {student.LastName} ({student.StudentNumber}) - Курс: {student.Year}, GPA: {student.GPA}");
        }

        System.Console.WriteLine();

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 4: Додавання деталей студента (зв'язок один-до-одного)
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 4. ДОДАВАННЯ ДЕТАЛЕЙ СТУДЕНТА (зв'язок один-до-одного)       ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var studentDetails = new StudentDetailsModel
        {
            Id = Guid.NewGuid(),
            StudentId = students[0].Id,
            Address = "м. Київ, вул. Хрещатик, 1",
            PhoneNumber = "+380501234567",
            Email = "oleksandr.ivanenko@example.com",
            EmergencyContact = "Мати: +380509876543"
        };

        await studentDetailsRepository.AddAsync(studentDetails);
        await studentDetailsRepository.SaveChangesAsync();
        
        System.Console.WriteLine($"✓ Додано деталі для студента {students[0].FirstName} {students[0].LastName}");
        System.Console.WriteLine($"  Адреса: {studentDetails.Address}");
        System.Console.WriteLine($"  Телефон: {studentDetails.PhoneNumber}");
        System.Console.WriteLine($"  Email: {studentDetails.Email}\n");

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 5: Запис студентів на курси (зв'язок багато-до-багатьох)
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 5. ЗАПИС СТУДЕНТІВ НА КУРСИ (зв'язок багато-до-багатьох)     ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var studentCourses = new[]
        {
            new { StudentId = students[0].Id, CourseId = courses[0].Id },
            new { StudentId = students[0].Id, CourseId = courses[1].Id },
            new { StudentId = students[0].Id, CourseId = courses[3].Id },
            new { StudentId = students[1].Id, CourseId = courses[1].Id },
            new { StudentId = students[1].Id, CourseId = courses[2].Id },
            new { StudentId = students[2].Id, CourseId = courses[0].Id },
            new { StudentId = students[2].Id, CourseId = courses[2].Id },
            new { StudentId = students[2].Id, CourseId = courses[3].Id }
        };

        foreach (var enrollment in studentCourses)
        {
            var studentCourse = new StudentCourseModel
            {
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = DateTime.Now,
                IsActive = true
            };

            await context.StudentCourses.AddAsync(studentCourse);
            
            var student = students.First(s => s.Id == enrollment.StudentId);
            var course = courses.First(c => c.Id == enrollment.CourseId);
            System.Console.WriteLine($"✓ Студента {student.FirstName} {student.LastName} записано на курс '{course.Name}'");
        }

        await context.SaveChangesAsync();
        System.Console.WriteLine();

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 6: Виставлення оцінок
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 6. ВИСТАВЛЕННЯ ОЦІНОК                                         ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var random = new Random();
        var grades = new List<GradeModel>();

        foreach (var enrollment in studentCourses)
        {
            var grade = new GradeModel
            {
                Id = Guid.NewGuid(),
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                Score = random.Next(60, 101),
                DateAssigned = DateTime.Now
            };

            await gradeRepository.AddAsync(grade);
            grades.Add(grade);
            
            var student = students.First(s => s.Id == enrollment.StudentId);
            var course = courses.First(c => c.Id == enrollment.CourseId);
            System.Console.WriteLine($"✓ Оцінка {grade.Score} виставлена студенту {student.FirstName} {student.LastName} за курс '{course.Name}'");
        }

        await gradeRepository.SaveChangesAsync();
        System.Console.WriteLine();

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 7: Читання даних з використанням Include (Eager Loading)
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 7. ЧИТАННЯ ДАНИХ З ЗВ'ЯЗКАМИ (Eager Loading)                 ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        // Читання студентів з їх деталями та оцінками
        var studentsWithDetails = await context.Students
            .Include(s => s.StudentDetails)
            .Include(s => s.Grades)
                .ThenInclude(g => g.Course)
            .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
            .ToListAsync();

        foreach (var student in studentsWithDetails)
        {
            System.Console.WriteLine($"\n👨‍🎓 Студент: {student.FirstName} {student.LastName}");
            System.Console.WriteLine($"   Номер: {student.StudentNumber}, Курс: {student.Year}, GPA: {student.GPA}");
            
            if (student.StudentDetails != null)
            {
                System.Console.WriteLine($"   📧 Контакт: {student.StudentDetails.Email}");
            }

            System.Console.WriteLine($"   📚 Записано на курси ({student.StudentCourses.Count}):");
            foreach (var sc in student.StudentCourses)
            {
                System.Console.WriteLine($"      • {sc.Course.Name} ({sc.Course.Credits} кредитів)");
            }

            if (student.Grades.Any())
            {
                System.Console.WriteLine($"   📊 Оцінки:");
                foreach (var grade in student.Grades)
                {
                    System.Console.WriteLine($"      • {grade.Course.Name}: {grade.Score}");
                }
                var avgScore = student.Grades.Average(g => g.Score);
                System.Console.WriteLine($"   📈 Середня оцінка: {avgScore:F2}");
            }
        }

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 8: Читання викладачів з їх курсами
        // ============================================================================
        
        System.Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 8. ВИКЛАДАЧІ ТА ЇХ КУРСИ                                      ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var teachersWithCourses = await context.Teachers
            .Include(t => t.Courses)
                .ThenInclude(c => c.StudentCourses)
            .ToListAsync();

        foreach (var teacher in teachersWithCourses)
        {
            System.Console.WriteLine($"\n👨‍🏫 Викладач: {teacher.FirstName} {teacher.LastName}");
            System.Console.WriteLine($"   Кафедра: {teacher.Department}, Посада: {teacher.Position}");
            System.Console.WriteLine($"   Зарплата: {teacher.Salary:N2} ₴");
            
            if (teacher.Courses.Any())
            {
                System.Console.WriteLine($"   📚 Веде курси ({teacher.Courses.Count}):");
                foreach (var course in teacher.Courses)
                {
                    var enrolledCount = course.StudentCourses.Count;
                    System.Console.WriteLine($"      • {course.Name} ({course.Credits} кредитів) - записано студентів: {enrolledCount}");
                }
            }
        }

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 9: Оновлення даних
        // ============================================================================
        
        System.Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 9. ОНОВЛЕННЯ ДАНИХ                                            ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var studentToUpdate = students[0];
        studentToUpdate.GPA = 4.7;
        await studentService.UpdateAsync(studentToUpdate);
        System.Console.WriteLine($"✓ Оновлено GPA студента {studentToUpdate.FirstName} {studentToUpdate.LastName}: {studentToUpdate.GPA}");

        var teacherToUpdate = teachers[0];
        teacherToUpdate.Salary += 3000;
        await teacherService.UpdateAsync(teacherToUpdate);
        System.Console.WriteLine($"✓ Оновлено зарплату викладача {teacherToUpdate.FirstName} {teacherToUpdate.LastName}: {teacherToUpdate.Salary:N2} ₴\n");

        // ============================================================================
        // ДЕМОНСТРАЦІЯ 10: Статистика
        // ============================================================================
        
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ 10. СТАТИСТИКА БАЗИ ДАНИХ                                     ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        var totalTeachers = await context.Teachers.CountAsync();
        var totalStudents = await context.Students.CountAsync();
        var totalCourses = await context.Courses.CountAsync();
        var totalGrades = await context.Grades.CountAsync();
        var totalEnrollments = await context.StudentCourses.CountAsync();

        System.Console.WriteLine($"📊 Статистика:");
        System.Console.WriteLine($"   • Викладачів: {totalTeachers}");
        System.Console.WriteLine($"   • Студентів: {totalStudents}");
        System.Console.WriteLine($"   • Курсів: {totalCourses}");
        System.Console.WriteLine($"   • Оцінок: {totalGrades}");
        System.Console.WriteLine($"   • Записів на курси: {totalEnrollments}");

        if (totalGrades > 0)
        {
            var avgGrade = await context.Grades.AverageAsync(g => g.Score);
            var maxGrade = await context.Grades.MaxAsync(g => g.Score);
            var minGrade = await context.Grades.MinAsync(g => g.Score);
            
            System.Console.WriteLine($"\n📈 Статистика оцінок:");
            System.Console.WriteLine($"   • Середня оцінка: {avgGrade:F2}");
            System.Console.WriteLine($"   • Максимальна оцінка: {maxGrade}");
            System.Console.WriteLine($"   • Мінімальна оцінка: {minGrade}");
        }

        // ============================================================================
        // ПІДСУМОК
        // ============================================================================
        
        System.Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ ПІДСУМОК ВИКОНАННЯ                                            ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        System.Console.WriteLine($"\n✨ Лабораторна робота №3 успішно виконана!");
        System.Console.WriteLine($"   ✓ Створено проєкт School.Infrastructure");
        System.Console.WriteLine($"   ✓ Реалізовано Table-per-Type (TPT) наслідування");
        System.Console.WriteLine($"   ✓ Додано зв'язки: один-до-одного, один-до-багатьох, багато-до-багатьох");
        System.Console.WriteLine($"   ✓ Використано Fluent API для конфігурації");
        System.Console.WriteLine($"   ✓ Реалізовано паттерн Repository");
        System.Console.WriteLine($"   ✓ Оновлено CRUD сервіс для роботи з базою даних");
        System.Console.WriteLine($"   ✓ База даних SQLite: school.db");

        System.Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║ Програму завершено успішно!                                   ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        // Чекаємо на клавішу тільки якщо консоль не перенаправлена
        if (!System.Console.IsOutputRedirected)
        {
            System.Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            System.Console.ReadKey();
        }
    }
}

