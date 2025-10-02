using System;
using System.Threading.Tasks;

namespace SingletonLoggerAdvanced
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = Logger.GetInstance();
            logger.LoadConfig("loggerconfig.json");

            Console.WriteLine("=== Многопоточное логирование ===");

            Parallel.For(0, 10, i =>
            {
                var log = Logger.GetInstance();
                log.Log($"Сообщение от потока {i}", LogLevel.INFO);

                if (i % 2 == 0)
                    log.Log($"Предупреждение в потоке {i}", LogLevel.WARNING);

                if (i % 3 == 0)
                    log.Log($"Ошибка в потоке {i}", LogLevel.ERROR);
            });

            Console.WriteLine("=== Чтение логов (только ошибки) ===");
            LogReader reader = new LogReader("logs/app_log.txt");
            reader.PrintLogs(LogLevel.ERROR);

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}