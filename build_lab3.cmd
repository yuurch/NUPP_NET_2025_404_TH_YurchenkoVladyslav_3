@echo off
chcp 65001 > nul
echo Компіляція проєкту School...
dotnet build School.sln
if %errorlevel% neq 0 (
    echo Помилка компіляції!
    exit /b %errorlevel%
)
echo Компіляція завершена успішно!

