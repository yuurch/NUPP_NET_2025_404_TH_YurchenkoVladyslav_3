# Інструкції для створення PDF документації та ER-діаграми

## Що потрібно включити в PDF

### 1. Титульна сторінка
- Назва: "Лабораторна робота №3 - Entity Framework та Repository Pattern"
- ПІБ студента
- Група
- Дата виконання

### 2. Результати виконання консольного застосунку

Запустіть програму та зробіть скріншоти:

```cmd
run_lab3.cmd
```

Або:

```bash
dotnet run --project School.Console/School.Console.csproj > lab3_output.txt
```

**Скріншоти повинні включати:**
- Створення викладачів
- Створення курсів
- Створення студентів
- Додавання деталей студента
- Запис студентів на курси
- Виставлення оцінок
- Читання даних з зв'язками
- Викладачі та їх курси
- Оновлення даних
- Статистика бази даних

### 3. Скріншоти створеної бази даних

Використайте будь-який з наступних інструментів для перегляду SQLite БД:

#### Варіант 1: DB Browser for SQLite
1. Завантажте: https://sqlitebrowser.org/
2. Відкрийте файл `school.db`
3. Зробіть скріншоти:
   - Структури всіх таблиць (схема)
   - Даних у кожній таблиці
   - Відношень (Foreign Keys)

#### Варіант 2: Visual Studio Code з розширенням SQLite
1. Встановіть розширення "SQLite Viewer"
2. Відкрийте `school.db`
3. Перегляньте таблиці

#### Варіант 3: Командний рядок SQLite
```bash
sqlite3 school.db
.tables
.schema Persons
.schema Students
.schema Teachers
.schema Courses
.schema Grades
.schema StudentDetails
.schema StudentCourses
SELECT * FROM Students;
SELECT * FROM Teachers;
SELECT * FROM Courses;
```

**Необхідні скріншоти:**
1. Список всіх таблиць
2. Структура таблиці Persons
3. Структура таблиці Students (показати TPT зв'язок)
4. Структура таблиці Teachers (показати TPT зв'язок)
5. Структура таблиці Courses
6. Структура таблиці Grades
7. Структура таблиці StudentDetails (показати 1:1 зв'язок)
8. Структура таблиці StudentCourses (показати M:N зв'язок)
9. Дані в таблиці Teachers
10. Дані в таблиці Students
11. Дані в таблиці Courses
12. Дані в таблиці Grades

### 4. ER-діаграма (Entity-Relationship Diagram)

Створіть ER-діаграму, яка показує:
- Всі сущності (таблиці)
- Атрибути кожної сущності
- Первинні ключі (PK)
- Зовнішні ключі (FK)
- Зв'язки між сущностями з відповідною кардинальністю

#### Варіант 1: Використати DB Browser for SQLite
1. Відкрийте `school.db`
2. Перейдіть на вкладку "Database Structure"
3. ПКМ → Export → Export Database to SQL/CSV/JSON
4. Використайте онлайн інструмент для генерації ER-діаграми

#### Варіант 2: Використати онлайн інструмент
- https://dbdiagram.io/
- https://app.quickdatabasediagrams.com/
- https://draw.io/

#### Варіант 3: Використати Entity Framework Power Tools
1. Встановіть EF Core Power Tools в Visual Studio
2. ПКМ на проєкті School.Infrastructure → EF Core Power Tools → Add DbContext Model Diagram

#### Приклад ER-діаграми (текстовий опис):

```
PERSONS (PK: Id)
├─ Id (Guid, PK)
├─ FirstName (String)
├─ LastName (String)
└─ DateOfBirth (DateTime)
    │
    ├── STUDENTS (TPT, PK/FK: Id)
    │   ├─ Id (Guid, PK, FK → Persons)
    │   ├─ StudentNumber (String, Unique)
    │   ├─ Year (Int)
    │   └─ GPA (Decimal)
    │       │
    │       ├── 1:1 → STUDENTDETAILS (FK: StudentId)
    │       │   ├─ Id (Guid, PK)
    │       │   ├─ Address (String)
    │       │   ├─ PhoneNumber (String)
    │       │   ├─ Email (String)
    │       │   ├─ EmergencyContact (String)
    │       │   └─ StudentId (Guid, FK → Students, Unique)
    │       │
    │       ├── 1:N → GRADES (FK: StudentId)
    │       │
    │       └── M:N → STUDENTCOURSES
    │
    └── TEACHERS (TPT, PK/FK: Id)
        ├─ Id (Guid, PK, FK → Persons)
        ├─ Department (String)
        ├─ Position (String)
        └─ Salary (Decimal)
            │
            └── 1:N → COURSES (FK: TeacherId)

COURSES (PK: Id)
├─ Id (Guid, PK)
├─ Name (String)
├─ Credits (Int)
└─ TeacherId (Guid, FK → Teachers)
    │
    ├── 1:N → GRADES (FK: CourseId)
    └── M:N → STUDENTCOURSES

GRADES (PK: Id)
├─ Id (Guid, PK)
├─ Score (Int)
├─ DateAssigned (DateTime)
├─ StudentId (Guid, FK → Students)
└─ CourseId (Guid, FK → Courses)

STUDENTCOURSES (PK: StudentId, CourseId)
├─ StudentId (Guid, PK, FK → Students)
├─ CourseId (Guid, PK, FK → Courses)
├─ EnrollmentDate (DateTime)
└─ IsActive (Bool)
```

### 5. Опис реалізації

Включіть у PDF:
- Опис використаних технологій
- Опис Table-per-Type (TPT) підходу
- Опис зв'язків між моделями
- Опис паттерну Repository
- Фрагменти коду (Fluent API конфігурація, Repository, CRUD сервіс)

### 6. Висновки

Опишіть:
- Що було зроблено
- Які зв'язки реалізовано
- Які переваги використання Entity Framework
- Які переваги паттерну Repository

## Приклад структури PDF

```
1. Титульна сторінка
2. Зміст
3. Вступ / Мета роботи
4. Опис предметної області (School)
5. Структура проєкту
6. Моделі даних
   6.1. Базова модель PersonModel
   6.2. StudentModel (TPT)
   6.3. TeacherModel (TPT)
   6.4. CourseModel
   6.5. GradeModel
   6.6. StudentDetailsModel (1:1)
   6.7. StudentCourseModel (M:N)
7. Зв'язки між моделями
   7.1. Один-до-одного (Student ↔ StudentDetails)
   7.2. Один-до-багатьох (Teacher → Courses, Course → Grades, Student → Grades)
   7.3. Багато-до-багатьох (Students ↔ Courses)
8. SchoolContext та Fluent API
   8.1. Конфігурація TPT
   8.2. Конфігурація зв'язків
   8.3. Конфігурація індексів
9. Паттерн Repository
   9.1. Інтерфейс IRepository
   9.2. Реалізація Repository
10. CRUD сервіс для БД
11. Міграції Entity Framework
12. ER-діаграма бази даних
13. Скріншоти структури БД
14. Результати виконання програми
    14.1. Створення даних
    14.2. Читання даних
    14.3. Оновлення даних
    14.4. Статистика
15. Висновки
16. Додатки (фрагменти коду)
```

## Генерація PDF

### Варіант 1: З Markdown
1. Використайте LAB3_README.md як базу
2. Конвертуйте через:
   - Pandoc: `pandoc LAB3_README.md -o LAB3.pdf`
   - Typora (якщо встановлено)
   - VS Code з розширенням "Markdown PDF"

### Варіант 2: Microsoft Word
1. Скопіюйте вміст з LAB3_README.md
2. Додайте скріншоти
3. Форматуйте документ
4. Експортуйте як PDF

### Варіант 3: Google Docs
1. Створіть новий документ
2. Додайте текст та скріншоти
3. File → Download → PDF

## Чек-лист для PDF

- [ ] Титульна сторінка з ПІБ та групою
- [ ] Опис завдання
- [ ] Структура проєкту
- [ ] Опис моделей даних
- [ ] Опис зв'язків (1:1, 1:N, M:N)
- [ ] Опис TPT підходу
- [ ] Фрагменти коду Fluent API
- [ ] Фрагменти коду Repository
- [ ] ER-діаграма бази даних
- [ ] Скріншоти структури БД (мінімум 8 таблиць)
- [ ] Скріншоти даних у таблицях
- [ ] Скріншоти виконання програми (повний вивід)
- [ ] Висновки

## Додаткові матеріали

Файли для включення в PDF:
- `LAB3_README.md` - документація проєкту
- Скріншоти з `school.db`
- ER-діаграма
- Вивід програми з `lab3_output.txt`

---

**Після створення PDF, додайте його до репозиторію:**

```bash
git add LAB3_Documentation.pdf
git add school.db
git commit -m "Додано документацію лабораторної роботи №3"
git push
```

