using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace SingletonLoggerAdvanced
{
    public enum LogLevel
    {
        INFO = 0,
        WARNING = 1,
        ERROR = 2
    }

    public sealed class Logger
    {
        private static Logger _instance = null;
        private static readonly object _lock = new object();

        private string _logFilePath = "logs/app_log.txt";
        private LogLevel _currentLogLevel = LogLevel.INFO;
        private bool _logToConsole = true;
        private bool _enableRotation = false;
        private long _maxFileSizeBytes = 1048576; // 1 MB

        private Logger()
        {
            Directory.CreateDirectory("logs");
        }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Logger();
                }
            }
            return _instance;
        }

        public void LoadConfig(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var config = JsonSerializer.Deserialize<LoggerConfig>(json);
                if (config != null)
                {
                    _logFilePath = config.LogFilePath ?? _logFilePath;
                    _logToConsole = config.LogToConsole;
                    _enableRotation = config.EnableRotation;
                    _maxFileSizeBytes = config.MaxFileSizeBytes;
                    _currentLogLevel = Enum.TryParse(config.Level, out LogLevel lvl) ? lvl : LogLevel.INFO;
                }
            }
        }

        public void SetLogLevel(LogLevel level) => _currentLogLevel = level;
        public void SetLogFilePath(string path) => _logFilePath = path;

        public void Log(string message, LogLevel level)
        {
            if (level < _currentLogLevel)
                return;

            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            lock (_lock)
            {
                if (_enableRotation && File.Exists(_logFilePath))
                {
                    var info = new FileInfo(_logFilePath);
                    if (info.Length > _maxFileSizeBytes)
                    {
                        string rotated = _logFilePath.Replace(".txt", $"_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                        File.Move(_logFilePath, rotated);
                    }
                }

                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);

                if (_logToConsole)
                    Console.WriteLine(logMessage);
            }
        }
    }

    public class LoggerConfig
    {
        public string LogFilePath { get; set; }
        public string Level { get; set; }
        public long MaxFileSizeBytes { get; set; }
        public bool LogToConsole { get; set; }
        public bool EnableRotation { get; set; }
    }
}