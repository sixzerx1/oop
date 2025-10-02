using System;
using System.IO;
public enum LogLevel
{
    Error,
    Warning,
    Info
}

public class Logger
{
    public void Log(string message, LogLevel level)
    {
        Console.WriteLine($"{level.ToString().ToUpper()}: {message}");
    }
}

public static class Config
{
    public static string ConnectionString = "Server=myServer;Database=myDb;User Id=myUser;Password=myPass;";
}

public class DatabaseService
{
    public void Connect()
    {
        string connectionString = Config.ConnectionString;
        Console.WriteLine("Подключение к базе...");
    }
}

public class LoggingService
{
    public void Log(string message)
    {
        string connectionString = Config.ConnectionString;
        Console.WriteLine($"Лог записан: {message}");
    }
}
public class NumberProcessor
{
    public void ProcessNumbers(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
            return;

        foreach (var number in numbers)
        {
            if (number > 0)
                Console.WriteLine(number);
        }
    }

    public void PrintPositiveNumbers(int[] numbers)
    {
        foreach (var number in numbers)
        {
            if (number > 0)
                Console.WriteLine(number);
        }
    }

    public int Divide(int a, int b)
    {
        if (b == 0)
            return 0;

        return a / b;
    }
}

public class User
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class FileReader
{
    public string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }
}

public class ReportGenerator
{
    public void GenerateReport(string format)
    {
        Console.WriteLine($"Report generated in format: {format}");
    }
}

class Program
{
    static void Main()
    {
        Logger logger = new Logger();
        logger.Log("Ошибка подключения", LogLevel.Error);

        NumberProcessor np = new NumberProcessor();
        np.ProcessNumbers(new int[] { -1, 0, 5, 10 });

        User user = new User { Name = "Alice", Email = "alice@example.com" };
        Console.WriteLine($"User: {user.Name}, {user.Email}");

        ReportGenerator rg = new ReportGenerator();
        rg.GenerateReport("PDF");
    }
}