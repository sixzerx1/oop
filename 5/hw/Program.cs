using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SingletonExample
{
 
    public sealed class ConfigurationManager
    {
        
        private static ConfigurationManager instance = null;
        private static readonly object locker = new object();

       
        private Dictionary<string, string> settings;

        
        private ConfigurationManager()
        {
            settings = new Dictionary<string, string>();
        }

       
        public static ConfigurationManager GetInstance()
        {
            
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new ConfigurationManager();
                    }
                }
            }
            return instance;
        }

       
        public void SetSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
                settings[key] = value;
            else
                settings.Add(key, value);
        }

     
        public string GetSetting(string key)
        {
            if (settings.ContainsKey(key))
                return settings[key];
            else
                throw new KeyNotFoundException($"Ключ {key} не найден в настройках.");
        }

     
        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл с настройками не найден.", filePath);

            settings.Clear();
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    settings[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

     
        public void SaveToFile(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var kvp in settings)
                {
                    writer.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }
        }
    }

   
    class Program
    {
        static void Main(string[] args)
        {
        
            var config1 = ConfigurationManager.GetInstance();
            config1.SetSetting("Theme", "Dark");
            config1.SetSetting("Language", "RU");

            var config2 = ConfigurationManager.GetInstance();
            Console.WriteLine($"Тема: {config2.GetSetting("Theme")}");
            Console.WriteLine($"Язык: {config2.GetSetting("Language")}");

          
            Thread t1 = new Thread(() =>
            {
                var c = ConfigurationManager.GetInstance();
                c.SetSetting("Thread1", "OK");
                Console.WriteLine("Поток 1 закончил работу.");
            });

            Thread t2 = new Thread(() =>
            {
                var c = ConfigurationManager.GetInstance();
                c.SetSetting("Thread2", "OK");
                Console.WriteLine("Поток 2 закончил работу.");
            });

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

         
            config1.SaveToFile("config.txt");

        
            var config3 = ConfigurationManager.GetInstance();
            config3.LoadFromFile("config.txt");
            Console.WriteLine("Настройки после загрузки из файла:");
            Console.WriteLine($"Theme: {config3.GetSetting("Theme")}");
            Console.WriteLine($"Language: {config3.GetSetting("Language")}");
            Console.WriteLine("Нажмите любую клавишу, чтобы выйти...");
            Console.ReadKey();

        }
    }
}