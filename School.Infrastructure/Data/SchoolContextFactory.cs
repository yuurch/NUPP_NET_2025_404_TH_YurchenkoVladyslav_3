using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace School.Infrastructure.Data;

// Фабрика для створення DbContext під час міграцій
public class SchoolContextFactory : IDesignTimeDbContextFactory<SchoolContext>
{
    public SchoolContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
        
        // Використовуємо SQLite для зберігання даних
        optionsBuilder.UseSqlite("Data Source=school.db");

        return new SchoolContext(optionsBuilder.Options);
    }
}

