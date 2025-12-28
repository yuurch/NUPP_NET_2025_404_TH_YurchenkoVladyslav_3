# Лабораторна робота №3 - Entity Framework та Repository Pattern

## Опис завдання

Створити проєкт School.Infrastructure з використанням Entity Framework Core, реалізувати паттерн Repository, створити міграції та застосувати їх до бази даних SQLite.

## Виконані завдання

### 1. Створено проєкт School.Infrastructure

Проєкт містить:
- **Models/** - моделі даних для Entity Framework
- **Data/** - контекст бази даних
- **Repositories/** - реалізація паттерну Repository
- **Services/** - CRUD сервіси для роботи з базою даних

### 2. Створені моделі даних (Models/)

#### Базова модель:
- **PersonModel** - базовий клас для осіб (використовується Table-per-Type підхід)

#### Моделі-нащадки:
- **StudentModel** - модель студента (наслідує PersonModel)
- **TeacherModel** - модель викладача (наслідує PersonModel)

#### Інші моделі:
- **CourseModel** - модель курсу
- **GradeModel** - модель оцінки
- **StudentDetailsModel** - додаткові деталі студента
- **StudentCourseModel** - проміжна таблиця для багато-до-багатьох

### 3. Реалізовані зв'язки між моделями

#### Один-до-одного (1:1):
- **Student ↔ StudentDetails** - кожен студент може мати одну запис з додатковими деталями

#### Один-до-багатьох (1:N):
- **Teacher → Courses** - один викладач може вести багато курсів
- **Course → Grades** - один курс може мати багато оцінок
- **Student → Grades** - один студент може мати багато оцінок

#### Багато-до-багатьох (M:N):
- **Students ↔ Courses** - студенти можуть бути записані на багато курсів, і кожен курс може мати багато студентів (через проміжну таблицю StudentCourses)

### 4. Table-per-Type (TPT) наслідування

Використано підхід TPT для ієрархії Person:
- **Persons** - базова таблиця з загальними полями (Id, FirstName, LastName, DateOfBirth)
- **Students** - окрема таблиця зі специфічними полями студента (StudentNumber, Year, GPA)
- **Teachers** - окрема таблиця зі специфічними полями викладача (Department, Position, Salary)

### 5. SchoolContext з Fluent API

Створено клас `SchoolContext`, який наслідується від `DbContext` та використовує **Fluent API** для конфігурації:

```csharp
public class SchoolContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфігурація TPT наслідування
        // Конфігурація зв'язків
        // Конфігурація індексів
        // Конфігурація обмежень
    }
}
```

### 6. Паттерн Repository

Реалізовано інтерфейс та клас Repository:

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task Update(T entity);
    Task Delete(T entity);
    Task<int> SaveChangesAsync();
}
```

### 7. Оновлений CRUD сервіс

Створено `CrudServiceAsyncDb<T>`, який реалізує `ICrudServiceAsync<T>` та використовує `IRepository<T>` для доступу до даних з бази даних.

### 8. База даних SQLite

Використано SQLite як реляційну СУБД. Файл бази даних: **school.db**

### 9. Міграції

Створено міграцію `InitialCreate`, яка містить:
- Створення всіх таблиць
- Налаштування первинних ключів
- Налаштування зовнішніх ключів
- Створення індексів

### 10. Оновлений консольний застосунок

Консольний застосунок (`ProgramLab3.cs`) демонструє:
1. Створення та збереження викладачів
2. Створення курсів (зв'язок один-до-багатьох)
3. Створення студентів
4. Додавання деталей студента (зв'язок один-до-одного)
5. Запис студентів на курси (зв'язок багато-до-багатьох)
6. Виставлення оцінок
7. Читання даних з зв'язками (Eager Loading)
8. Викладачі та їх курси
9. Оновлення даних
10. Статистика бази даних

## Структура проєкту

```
School.Infrastructure/
├── Data/
│   ├── SchoolContext.cs          # DbContext з Fluent API
│   └── SchoolContextFactory.cs   # Фабрика для міграцій
├── Models/
│   ├── PersonModel.cs            # Базова модель
│   ├── StudentModel.cs           # Модель студента (TPT)
│   ├── TeacherModel.cs           # Модель викладача (TPT)
│   ├── CourseModel.cs            # Модель курсу
│   ├── GradeModel.cs             # Модель оцінки
│   ├── StudentDetailsModel.cs    # Деталі студента (1:1)
│   └── StudentCourseModel.cs     # Проміжна таблиця (M:N)
├── Repositories/
│   ├── IRepository.cs            # Інтерфейс репозиторію
│   └── Repository.cs             # Реалізація репозиторію
├── Services/
│   └── CrudServiceAsyncDb.cs     # CRUD сервіс для БД
└── Migrations/
    └── 20251228_InitialCreate.cs # Початкова міграція
```

## Схема бази даних

### Таблиці:

1. **Persons** - базова таблиця (TPT)
   - Id (PK, Guid)
   - FirstName
   - LastName
   - DateOfBirth

2. **Students** - таблиця студентів (TPT, наслідує Persons)
   - Id (PK, FK → Persons)
   - StudentNumber (Unique)
   - Year
   - GPA

3. **Teachers** - таблиця викладачів (TPT, наслідує Persons)
   - Id (PK, FK → Persons)
   - Department
   - Position
   - Salary

4. **Courses** - таблиця курсів
   - Id (PK, Guid)
   - Name
   - Credits
   - TeacherId (FK → Teachers) - зв'язок один-до-багатьох

5. **Grades** - таблиця оцінок
   - Id (PK, Guid)
   - Score
   - DateAssigned
   - StudentId (FK → Students) - зв'язок один-до-багатьох
   - CourseId (FK → Courses) - зв'язок один-до-багатьох

6. **StudentDetails** - додаткові деталі студентів
   - Id (PK, Guid)
   - Address
   - PhoneNumber
   - Email
   - EmergencyContact
   - StudentId (FK → Students, Unique) - зв'язок один-до-одного

7. **StudentCourses** - проміжна таблиця для багато-до-багатьох
   - StudentId (PK, FK → Students)
   - CourseId (PK, FK → Courses)
   - EnrollmentDate
   - IsActive

## Як запустити

### Компіляція:
```bash
dotnet build School.sln
```

або

```cmd
build_lab3.cmd
```

### Запуск:
```bash
dotnet run --project School.Console/School.Console.csproj
```

або

```cmd
run_lab3.cmd
```

## Використані технології

- **.NET 8.0** - платформа розробки
- **Entity Framework Core 8.0** - ORM для роботи з базою даних
- **SQLite** - реляційна база даних
- **Fluent API** - конфігурація моделей
- **Repository Pattern** - паттерн для абстракції доступу до даних
- **Table-per-Type (TPT)** - підхід до наслідування в БД

## Особливості реалізації

1. **Fluent API** використовується замість анотацій для більш гнучкої конфігурації
2. **Table-per-Type (TPT)** для чистої реалізації наслідування
3. **Eager Loading** для ефективного завантаження зв'язаних даних
4. **Композитні індекси** для оптимізації запитів
5. **Каскадне видалення** налаштоване для відповідних зв'язків

## Автор

Юрченко Владислав, група 404-ТХ

## Дата виконання

28 грудня 2025

