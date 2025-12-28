# ER-діаграма бази даних School

## Візуальне представлення

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              SCHOOL DATABASE SCHEMA                                 │
└─────────────────────────────────────────────────────────────────────────────────────┘


┌──────────────────────────┐
│       PERSONS            │  ◄─── Базова таблиця (Table-per-Type)
├──────────────────────────┤
│ 🔑 Id (Guid, PK)         │
│    FirstName (String)    │
│    LastName (String)     │
│    DateOfBirth (DateTime)│
└────────┬─────────────────┘
         │
         │ TPT (Table-per-Type)
         │
         ├─────────────────────────────────────────────────────────┐
         │                                                         │
         ▼                                                         ▼
┌──────────────────────────┐                            ┌──────────────────────────┐
│       STUDENTS           │                            │       TEACHERS           │
├──────────────────────────┤                            ├──────────────────────────┤
│ 🔑 Id (Guid, PK, FK)     │                            │ 🔑 Id (Guid, PK, FK)     │
│    StudentNumber (String)│                            │    Department (String)   │
│    Year (Int)            │                            │    Position (String)     │
│    GPA (Decimal)         │                            │    Salary (Decimal)      │
└────────┬─────────────────┘                            └─────┬────────────────────┘
         │                                                     │
         │                                                     │
         │                                                     │ 1:N (один-до-багатьох)
         │                                                     │
         │                                                     ▼
         │                                              ┌──────────────────────────┐
         │                                              │       COURSES            │
         │                                              ├──────────────────────────┤
         │                                              │ 🔑 Id (Guid, PK)         │
         │                                              │    Name (String)         │
         │                                              │    Credits (Int)         │
         │                                              │ 🔗 TeacherId (Guid, FK) │
         │                                              └─────┬────────────────────┘
         │                                                     │
         │                                                     │
         │                                                     │ 1:N
         │ 1:1 (один-до-одного)                               │
         │                                                     │
         ▼                                                     │
┌──────────────────────────┐                                  │
│   STUDENTDETAILS         │                                  │
├──────────────────────────┤                                  │
│ 🔑 Id (Guid, PK)         │                                  │
│    Address (String)      │                                  │
│    PhoneNumber (String)  │                                  │
│    Email (String)        │                                  │
│    EmergencyContact (Str)│                                  │
│ 🔗 StudentId (Guid, FK)  │◄─ Unique (забезпечує 1:1)       │
└──────────────────────────┘                                  │
                                                              │
                                                              │
         ┌────────────────────────────────────────────────────┤
         │                                                    │
         │ M:N (багато-до-багатьох)                          │ 1:N
         │                                                    │
         ▼                                                    ▼
┌──────────────────────────┐                          ┌──────────────────────────┐
│   STUDENTCOURSES         │                          │       GRADES             │
├──────────────────────────┤                          ├──────────────────────────┤
│ 🔑 StudentId (Guid, PK)  │                          │ 🔑 Id (Guid, PK)         │
│ 🔑 CourseId (Guid, PK)   │                          │    Score (Int)           │
│ 🔗 StudentId (FK)        │───┐                      │    DateAssigned (DateTime│
│ 🔗 CourseId (FK)         │───┤                      │ 🔗 StudentId (Guid, FK)  │
│    EnrollmentDate (Date) │   │                      │ 🔗 CourseId (Guid, FK)   │
│    IsActive (Bool)       │   │                      └────┬─────────────────────┘
└──────────────────────────┘   │                           │
         │                     │                           │
         │                     │                           │
         │                     │                           │
         └─────────────────────┴───────────────────────────┘
                      │                       │
                      ▼                       ▼
                  STUDENTS                COURSES
```

## Детальний опис зв'язків

### 1. Table-per-Type (TPT) наслідування

**PERSONS → STUDENTS** (TPT)
- Зв'язок: 1:1 (кожен запис у Students має відповідний запис у Persons)
- Тип: Наслідування через окремі таблиці
- PK Students.Id = FK → Persons.Id

**PERSONS → TEACHERS** (TPT)
- Зв'язок: 1:1 (кожен запис у Teachers має відповідний запис у Persons)
- Тип: Наслідування через окремі таблиці
- PK Teachers.Id = FK → Persons.Id

### 2. Один-до-одного (1:1)

**STUDENTS ↔ STUDENTDETAILS**
- Кардинальність: 1:1
- Зв'язок: Student може мати один запис StudentDetails
- FK: StudentDetails.StudentId → Students.Id (Unique)
- Каскадне видалення: CASCADE

### 3. Один-до-багатьох (1:N)

**TEACHERS → COURSES**
- Кардинальність: 1:N
- Зв'язок: Teacher може вести багато Courses
- FK: Courses.TeacherId → Teachers.Id
- Каскадне видалення: RESTRICT

**STUDENTS → GRADES**
- Кардинальність: 1:N
- Зв'язок: Student може мати багато Grades
- FK: Grades.StudentId → Students.Id
- Каскадне видалення: CASCADE

**COURSES → GRADES**
- Кардинальність: 1:N
- Зв'язок: Course може мати багато Grades
- FK: Grades.CourseId → Courses.Id
- Каскадне видалення: CASCADE

### 4. Багато-до-багатьох (M:N)

**STUDENTS ↔ COURSES** (через STUDENTCOURSES)
- Кардинальність: M:N
- Зв'язок: Student може бути записаний на багато Courses, Course може мати багато Students
- Проміжна таблиця: STUDENTCOURSES
- PK: (StudentId, CourseId)
- FK1: StudentCourses.StudentId → Students.Id
- FK2: StudentCourses.CourseId → Courses.Id
- Каскадне видалення: CASCADE на обидва FK

## Індекси

### Унікальні індекси:
- `IX_Students_StudentNumber` (Students.StudentNumber) - Unique
- `IX_StudentDetails_StudentId` (StudentDetails.StudentId) - Unique
- `IX_Grades_StudentId_CourseId` (Grades.StudentId, Grades.CourseId) - Unique

### Звичайні індекси:
- `IX_Teachers_Department` (Teachers.Department)
- `IX_Courses_TeacherId` (Courses.TeacherId)
- `IX_Grades_StudentId` (Grades.StudentId)
- `IX_Grades_CourseId` (Grades.CourseId)
- `IX_StudentCourses_StudentId` (StudentCourses.StudentId)
- `IX_StudentCourses_CourseId` (StudentCourses.CourseId)

## Типи даних (SQLite)

| Таблиця | Колонка | Тип | Обмеження |
|---------|---------|-----|-----------|
| Persons | Id | TEXT (Guid) | PRIMARY KEY |
| Persons | FirstName | TEXT | NOT NULL, MaxLength(100) |
| Persons | LastName | TEXT | NOT NULL, MaxLength(100) |
| Persons | DateOfBirth | TEXT (DateTime) | NOT NULL |
| Students | Id | TEXT (Guid) | PRIMARY KEY, FOREIGN KEY |
| Students | StudentNumber | TEXT | NOT NULL, UNIQUE, MaxLength(50) |
| Students | Year | INTEGER | NOT NULL |
| Students | GPA | NUMERIC (decimal) | NOT NULL |
| Teachers | Id | TEXT (Guid) | PRIMARY KEY, FOREIGN KEY |
| Teachers | Department | TEXT | NOT NULL, MaxLength(100) |
| Teachers | Position | TEXT | NOT NULL, MaxLength(100) |
| Teachers | Salary | NUMERIC (decimal) | NOT NULL |
| Courses | Id | TEXT (Guid) | PRIMARY KEY |
| Courses | Name | TEXT | NOT NULL, MaxLength(200) |
| Courses | Credits | INTEGER | NOT NULL |
| Courses | TeacherId | TEXT (Guid) | FOREIGN KEY |
| Grades | Id | TEXT (Guid) | PRIMARY KEY |
| Grades | Score | INTEGER | NOT NULL |
| Grades | DateAssigned | TEXT (DateTime) | NOT NULL |
| Grades | StudentId | TEXT (Guid) | FOREIGN KEY |
| Grades | CourseId | TEXT (Guid) | FOREIGN KEY |
| StudentDetails | Id | TEXT (Guid) | PRIMARY KEY |
| StudentDetails | Address | TEXT | MaxLength(500) |
| StudentDetails | PhoneNumber | TEXT | MaxLength(20) |
| StudentDetails | Email | TEXT | MaxLength(100) |
| StudentDetails | EmergencyContact | TEXT | MaxLength(200) |
| StudentDetails | StudentId | TEXT (Guid) | FOREIGN KEY, UNIQUE |
| StudentCourses | StudentId | TEXT (Guid) | PRIMARY KEY, FOREIGN KEY |
| StudentCourses | CourseId | TEXT (Guid) | PRIMARY KEY, FOREIGN KEY |
| StudentCourses | EnrollmentDate | TEXT (DateTime) | NOT NULL |
| StudentCourses | IsActive | INTEGER (bool) | NOT NULL, DEFAULT(1) |

## Приклад SQL запитів

### Отримати всіх студентів з їх деталями:
```sql
SELECT 
    p.FirstName, p.LastName, p.DateOfBirth,
    s.StudentNumber, s.Year, s.GPA,
    sd.Email, sd.PhoneNumber
FROM Students s
INNER JOIN Persons p ON s.Id = p.Id
LEFT JOIN StudentDetails sd ON s.Id = sd.StudentId;
```

### Отримати викладачів з їх курсами:
```sql
SELECT 
    p.FirstName, p.LastName,
    t.Department, t.Position,
    c.Name AS CourseName, c.Credits
FROM Teachers t
INNER JOIN Persons p ON t.Id = p.Id
LEFT JOIN Courses c ON t.Id = c.TeacherId;
```

### Отримати студентів та їх курси:
```sql
SELECT 
    p.FirstName, p.LastName,
    c.Name AS CourseName,
    sc.EnrollmentDate, sc.IsActive
FROM StudentCourses sc
INNER JOIN Students s ON sc.StudentId = s.Id
INNER JOIN Persons p ON s.Id = p.Id
INNER JOIN Courses c ON sc.CourseId = c.Id;
```

### Отримати оцінки студентів:
```sql
SELECT 
    p.FirstName, p.LastName,
    c.Name AS CourseName,
    g.Score, g.DateAssigned
FROM Grades g
INNER JOIN Students s ON g.StudentId = s.Id
INNER JOIN Persons p ON s.Id = p.Id
INNER JOIN Courses c ON g.CourseId = c.Id
ORDER BY p.LastName, c.Name;
```

## Переваги даної схеми

1. **Чиста реалізація наслідування (TPT)**
   - Немає NULL-полів у базовій таблиці
   - Легко додавати нові типи осіб

2. **Нормалізація**
   - Мінімальна надмірність даних
   - Легко підтримувати цілісність

3. **Гнучкість зв'язків**
   - Різні типи зв'язків (1:1, 1:N, M:N)
   - Легко розширювати

4. **Індекси**
   - Оптимізовані запити
   - Унікальність даних

5. **Каскадні операції**
   - Автоматичне підтримання цілісності
   - Контрольоване видалення

