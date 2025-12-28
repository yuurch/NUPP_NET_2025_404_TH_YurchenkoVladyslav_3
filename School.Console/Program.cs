using School.Console;

// Запуск Лабораторної роботи №3
await ProgramLab3.Main(args);

// Нижче закоментований код Лабораторної роботи №2
/*
using School.Common;
using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║    Лабораторна робота №2 - Асинхронний CRUD Сервіс           ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

// Створення асинхронного CRUD сервісу для викладачів
var teacherService = new CrudServiceAsync<Teacher>(t => t.Id, "teachers_data.json");

// ============================================================================
// ДЕМОНСТРАЦІЯ 1: Паралельне створення об'єктів з використанням Lock
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 1. ПАРАЛЕЛЬНЕ СТВОРЕННЯ 1000+ ОБ'ЄКТІВ TEACHER                ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

int totalTeachers = 1500;
int createdCount = 0;
int failedCount = 0;
object lockObject = new object();

var stopwatch = Stopwatch.StartNew();

Console.WriteLine($"\n⏱️  Починаємо паралельне створення {totalTeachers} викладачів...\n");

Parallel.For(0, totalTeachers, new ParallelOptions { MaxDegreeOfParallelism = 10 }, i =>
{
    var teacher = Teacher.CreateNew();
    bool success = teacherService.CreateAsync(teacher).GetAwaiter().GetResult();
    
    // Lock для безпечного оновлення лічильників
    lock (lockObject)
    {
        if (success)
        {
            createdCount++;
            if (createdCount % 100 == 0)
            {
                Console.WriteLine($"✓ Створено викладачів: {createdCount}/{totalTeachers}");
            }
        }
        else
        {
            failedCount++;
        }
    }
});

stopwatch.Stop();

Console.WriteLine($"\n✅ Створення завершено!");
Console.WriteLine($"   • Успішно створено: {createdCount}");
Console.WriteLine($"   • Невдалих спроб: {failedCount}");
Console.WriteLine($"   • Час виконання: {stopwatch.ElapsedMilliseconds} мс");
Console.WriteLine($"   • Елементів у сервісі: {teacherService.Count}");

// ============================================================================
// ДЕМОНСТРАЦІЯ 2: Використання Semaphore для обмеження паралелізму
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 2. ДЕМОНСТРАЦІЯ SEMAPHORE - ОБМЕЖЕННЯ КОНКУРЕНТНОГО ДОСТУПУ  ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

SemaphoreSlim semaphore = new SemaphoreSlim(3, 3); // Максимум 3 одночасних операції
int operationCount = 0;
object semLockObject = new object();

Console.WriteLine("\n🔒 Демонструємо Semaphore з максимум 3 одночасними операціями...\n");

var semaphoreStopwatch = Stopwatch.StartNew();

Parallel.For(0, 10, async i =>
{
    await semaphore.WaitAsync();
    try
    {
        lock (semLockObject)
        {
            operationCount++;
            Console.WriteLine($"   → Операція #{i + 1} виконується (активних: {3 - semaphore.CurrentCount})");
        }
        
        // Симуляція тривалої операції
        await Task.Delay(500);
        
        var teachers = await teacherService.ReadAllAsync(1, 10);
        
        lock (semLockObject)
        {
            Console.WriteLine($"   ← Операція #{i + 1} завершена. Прочитано {teachers.Count()} викладачів");
        }
    }
    finally
    {
        semaphore.Release();
    }
});

semaphoreStopwatch.Stop();
Console.WriteLine($"\n✅ Усі операції з Semaphore завершено за {semaphoreStopwatch.ElapsedMilliseconds} мс");

// ============================================================================
// ДЕМОНСТРАЦІЯ 3: AutoResetEvent для сигналізації між потоками
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 3. ДЕМОНСТРАЦІЯ AUTORESETEVENT - СИГНАЛІЗАЦІЯ МІЖ ПОТОКАМИ   ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

AutoResetEvent autoResetEvent = new AutoResetEvent(false);
int batchSize = 100;
int batchCount = 0;

Console.WriteLine($"\n📊 Обробка даних пакетами по {batchSize} елементів...\n");

Task.Run(async () =>
{
    var allTeachers = await teacherService.ReadAllAsync();
    var batches = allTeachers.Chunk(batchSize);
    
    foreach (var batch in batches)
    {
        batchCount++;
        Console.WriteLine($"   🔄 Обробка пакету #{batchCount} ({batch.Count()} елементів)...");
        await Task.Delay(300); // Симуляція обробки
        Console.WriteLine($"   ✓ Пакет #{batchCount} оброблено");
        
        // Сигналізуємо, що пакет оброблено
        autoResetEvent.Set();
    }
});

// Чекаємо на обробку кожного пакету
int expectedBatches = (createdCount + batchSize - 1) / batchSize;
for (int i = 0; i < expectedBatches; i++)
{
    autoResetEvent.WaitOne();
}

Console.WriteLine($"\n✅ Усі {batchCount} пакети оброблено через AutoResetEvent!");

// ============================================================================
// ДЕМОНСТРАЦІЯ 4: Monitor.Wait та Monitor.Pulse
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 4. ДЕМОНСТРАЦІЯ MONITOR - WAIT/PULSE PATTERN                  ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

object monitorLock = new object();
bool dataReady = false;
List<Teacher>? processedData = null;

Console.WriteLine("\n🔄 Споживач чекає на дані від постачальника...\n");

// Постачальник (Producer)
Task.Run(async () =>
{
    await Task.Delay(1000);
    Console.WriteLine("   📤 Постачальник: Підготовка даних...");
    
    var data = await teacherService.ReadAllAsync(1, 50);
    
    lock (monitorLock)
    {
        processedData = data.ToList();
        dataReady = true;
        Console.WriteLine($"   ✓ Постачальник: Дані готові ({processedData.Count} елементів)");
        Monitor.Pulse(monitorLock); // Сигналізуємо споживачу
    }
});

// Споживач (Consumer)
lock (monitorLock)
{
    while (!dataReady)
    {
        Console.WriteLine("   ⏳ Споживач: Очікування даних...");
        Monitor.Wait(monitorLock); // Чекаємо сигналу від постачальника
    }
    Console.WriteLine($"   📥 Споживач: Отримано {processedData?.Count ?? 0} елементів");
}

Console.WriteLine("\n✅ Monitor Wait/Pulse завершено успішно!");

// ============================================================================
// СТАТИСТИКА: Обчислення мін/макс/середніх значень
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 5. СТАТИСТИЧНИЙ АНАЛІЗ ДАНИХ                                  ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

var allTeachersForStats = await teacherService.ReadAllAsync();
var teachersList = allTeachersForStats.ToList();

if (teachersList.Any())
{
    // Статистика по зарплаті
    decimal minSalary = teachersList.Min(t => t.Salary);
    decimal maxSalary = teachersList.Max(t => t.Salary);
    decimal avgSalary = teachersList.Average(t => t.Salary);
    
    // Статистика по віку
    var ages = teachersList.Select(t => t.CalculateAge()).ToList();
    int minAge = ages.Min();
    int maxAge = ages.Max();
    double avgAge = ages.Average();
    
    // Статистика по департаментах
    var departmentStats = teachersList
        .GroupBy(t => t.Department)
        .Select(g => new { Department = g.Key, Count = g.Count() })
        .OrderByDescending(x => x.Count)
        .ToList();
    
    Console.WriteLine("\n💰 СТАТИСТИКА ПО ЗАРПЛАТІ:");
    Console.WriteLine($"   • Мінімальна зарплата: {minSalary:N2} ₴");
    Console.WriteLine($"   • Максимальна зарплата: {maxSalary:N2} ₴");
    Console.WriteLine($"   • Середня зарплата: {avgSalary:N2} ₴");
    
    Console.WriteLine("\n👤 СТАТИСТИКА ПО ВІКУ:");
    Console.WriteLine($"   • Мінімальний вік: {minAge} років");
    Console.WriteLine($"   • Максимальний вік: {maxAge} років");
    Console.WriteLine($"   • Середній вік: {avgAge:F1} років");
    
    Console.WriteLine("\n🏢 РОЗПОДІЛ ПО ДЕПАРТАМЕНТАХ:");
    foreach (var stat in departmentStats)
    {
        double percentage = (stat.Count * 100.0) / teachersList.Count;
        string bar = new string('█', (int)(percentage / 2));
        Console.WriteLine($"   {stat.Department,-20} {stat.Count,4} [{bar}] {percentage:F1}%");
    }
}

// ============================================================================
// ДЕМОНСТРАЦІЯ ПАГІНАЦІЇ
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 6. ДЕМОНСТРАЦІЯ ПАГІНАЦІЇ                                     ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

int pageSize = 5;
int currentPage = 1;

Console.WriteLine($"\n📄 Перші 3 сторінки по {pageSize} елементів:\n");

for (int page = 1; page <= 3; page++)
{
    var pageData = await teacherService.ReadAllAsync(page, pageSize);
    Console.WriteLine($"--- Сторінка {page} ---");
    foreach (var teacher in pageData)
    {
        Console.WriteLine($"  • {teacher.GetFullName()} - {teacher.Department} ({teacher.Position})");
    }
    Console.WriteLine();
}

// ============================================================================
// ЗБЕРЕЖЕННЯ ДАНИХ У ФАЙЛ
// ============================================================================
Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ 7. ЗБЕРЕЖЕННЯ ДАНИХ У JSON ФАЙЛ                               ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

Console.WriteLine($"\n💾 Зберігаємо дані у файл '{teacherService.FilePath}'...");

var saveStopwatch = Stopwatch.StartNew();
bool saved = await teacherService.SaveAsync();
saveStopwatch.Stop();

if (saved)
{
    var fileInfo = new FileInfo(teacherService.FilePath);
    Console.WriteLine($"✅ Дані успішно збережено!");
    Console.WriteLine($"   • Файл: {fileInfo.FullName}");
    Console.WriteLine($"   • Розмір: {fileInfo.Length / 1024.0:F2} КБ");
    Console.WriteLine($"   • Час збереження: {saveStopwatch.ElapsedMilliseconds} мс");
}
else
{
    Console.WriteLine("❌ Помилка при збереженні даних");
}

// ============================================================================
// ПІДСУМОК
// ============================================================================
Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ ПІДСУМОК ВИКОНАННЯ                                            ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

Console.WriteLine($"\n✨ Лабораторна робота №2 успішно виконана!");
Console.WriteLine($"   • Створено викладачів: {teacherService.Count}");
Console.WriteLine($"   • Використано синхронізацію: Lock, Semaphore, AutoResetEvent, Monitor");
Console.WriteLine($"   • Дані збережено у: {teacherService.FilePath}");
Console.WriteLine($"   • Загальний час виконання: {stopwatch.Elapsed.TotalSeconds:F2} с");

Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ Програму завершено успішно!                                   ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

// Чекаємо на клавішу тільки якщо консоль не перенаправлена
if (!Console.IsOutputRedirected)
{
    Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
    Console.ReadKey();
}
*/
