using System;
using System.IO;

namespace SingletonLoggerAdvanced
{
    public class LogReader
    {
        private readonly string _filePath;

        public LogReader(string path)
        {
            _filePath = path;
        }

        public void PrintLogs(LogLevel minLevel)
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("Файл логов отсутствует.");
                return;
            }

            string[] lines = File.ReadAllLines(_filePath);
            foreach (string line in lines)
            {
                if (line.Contains("[ERROR]") && minLevel == LogLevel.ERROR)
                    Console.WriteLine(line);
                else if (line.Contains("[WARNING]") && (minLevel == LogLevel.WARNING || minLevel == LogLevel.INFO))
                    Console.WriteLine(line);
                else if (line.Contains("[INFO]") && minLevel == LogLevel.INFO)
                    Console.WriteLine(line);
            }
        }
    }
}