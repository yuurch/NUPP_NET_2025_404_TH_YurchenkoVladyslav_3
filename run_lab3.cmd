@echo off
chcp 65001 > nul
echo Запуск Лабораторної роботи №3...
echo.
dotnet run --project School.Console\School.Console.csproj
if %errorlevel% neq 0 (
    echo.
    echo Помилка виконання!
    exit /b %errorlevel%
)
echo.
echo Програма завершена.
pause

