using System;
using System.Collections.Generic;

public class TV
{
    private string _location;

    public TV(string location)
    {
        _location = location;
    }

    public void TurnOn()
    {
        Console.WriteLine($"Телевизор в {_location} включен");
    }

    public void TurnOff()
    {
        Console.WriteLine($"Телевизор в {_location} выключен");
    }

    public void SetChannel(int channel)
    {
        Console.WriteLine($"Телевизор переключен на канал {channel}");
    }

    public void SetInput(string input)
    {
        Console.WriteLine($"Вход телевизора установлен на {input}");
    }

    public void AdjustVolume(int level)
    {
        Console.WriteLine($"Громкость телевизора установлена на {level}");
    }
}

public class AudioSystem
{
    private string _location;

    public AudioSystem(string location)
    {
        _location = location;
    }

    public void TurnOn()
    {
        Console.WriteLine($"Аудиосистема в {_location} включена");
    }

    public void TurnOff()
    {
        Console.WriteLine($"Аудиосистема в {_location} выключена");
    }

    public void SetVolume(int level)
    {
        Console.WriteLine($"Громкость аудиосистемы установлена на {level}");
    }

    public void SetInput(string input)
    {
        Console.WriteLine($"Вход аудиосистемы установлен на {input}");
    }

    public void SetSurroundSound()
    {
        Console.WriteLine("Объемный звук активирован");
    }

    public void SetStereoMode()
    {
        Console.WriteLine("Стерео режим активирован");
    }
}

public class DVDPlayer
{
    private string _location;

    public DVDPlayer(string location)
    {
        _location = location;
    }

    public void TurnOn()
    {
        Console.WriteLine($"DVD-проигрыватель в {_location} включен");
    }

    public void TurnOff()
    {
        Console.WriteLine($"DVD-проигрыватель в {_location} выключен");
    }

    public void Play()
    {
        Console.WriteLine("DVD-проигрыватель воспроизводит фильм");
    }

    public void Pause()
    {
        Console.WriteLine("DVD-проигрыватель поставлен на паузу");
    }

    public void Stop()
    {
        Console.WriteLine("DVD-проигрыватель остановлен");
    }

    public void Eject()
    {
        Console.WriteLine("Диск извлечен из DVD-проигрывателя");
    }
}

public class GameConsole
{
    private string _location;

    public GameConsole(string location)
    {
        _location = location;
    }

    public void TurnOn()
    {
        Console.WriteLine($"Игровая консоль в {_location} включена");
    }

    public void TurnOff()
    {
        Console.WriteLine($"Игровая консоль в {_location} выключена");
    }

    public void StartGame(string gameTitle)
    {
        Console.WriteLine($"Запущена игра: {gameTitle}");
    }

    public void SetInput(string input)
    {
        Console.WriteLine($"Вход консоли установлен на {input}");
    }
}

public class HomeTheaterFacade
{
    private TV _tv;
    private AudioSystem _audio;
    private DVDPlayer _dvdPlayer;
    private GameConsole _gameConsole;
    private string _location;

    public HomeTheaterFacade(string location)
    {
        _location = location;
        _tv = new TV(location);
        _audio = new AudioSystem(location);
        _dvdPlayer = new DVDPlayer(location);
        _gameConsole = new GameConsole(location);
    }

    public void WatchMovie(string movieTitle)
    {
        Console.WriteLine($"=== Начинаем просмотр фильма: {movieTitle} ===");
        
        _tv.TurnOn();
        _tv.SetInput("HDMI 1");
        _tv.SetChannel(1);
        
        _audio.TurnOn();
        _audio.SetInput("DVD");
        _audio.SetVolume(20);
        _audio.SetSurroundSound();
        
        _dvdPlayer.TurnOn();
        _dvdPlayer.Play();
        
        Console.WriteLine($"Фильм '{movieTitle}' начался. Приятного просмотра!\n");
    }

    public void PlayGame(string gameTitle)
    {
        Console.WriteLine($"=== Запускаем игровую сессию: {gameTitle} ===");
        
        _tv.TurnOn();
        _tv.SetInput("HDMI 2");
        
        _audio.TurnOn();
        _audio.SetInput("Console");
        _audio.SetVolume(25);
        _audio.SetStereoMode();
        
        _gameConsole.TurnOn();
        _gameConsole.StartGame(gameTitle);
        
        Console.WriteLine($"Игра '{gameTitle}' запущена. Удачи!\n");
    }

    public void ListenToMusic()
    {
        Console.WriteLine("=== Включаем режим прослушивания музыки ===");
        
        _tv.TurnOn();
        _tv.SetInput("Audio");
        
        _audio.TurnOn();
        _audio.SetInput("TV");
        _audio.SetVolume(15);
        _audio.SetStereoMode();
        
        Console.WriteLine("Система готова для прослушивания музыки\n");
    }

    public void WatchTV(int channel)
    {
        Console.WriteLine($"=== Смотрим телевизор, канал {channel} ===");
        
        _tv.TurnOn();
        _tv.SetChannel(channel);
        _tv.AdjustVolume(18);
        
        _audio.TurnOn();
        _audio.SetInput("TV");
        _audio.SetVolume(18);
        
        Console.WriteLine($"Телевизор настроен на канал {channel}\n");
    }

    public void TurnOffAll()
    {
        Console.WriteLine("=== Выключаем всю систему ===");
        
        _dvdPlayer.Stop();
        _dvdPlayer.TurnOff();
        
        _gameConsole.TurnOff();
        
        _audio.TurnOff();
        
        _tv.TurnOff();
        
        Console.WriteLine("Вся система выключена\n");
    }

    public void SetVolume(int level)
    {
        if (level < 0) level = 0;
        if (level > 100) level = 100;
        
        _audio.SetVolume(level);
        Console.WriteLine($"Громкость установлена на {level}\n");
    }

    public void Pause()
    {
        _dvdPlayer.Pause();
        Console.WriteLine("Воспроизведение поставлено на паузу\n");
    }

    public void Resume()
    {
        _dvdPlayer.Play();
        Console.WriteLine("Воспроизведение продолжено\n");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== СИСТЕМА УПРАВЛЕНИЯ МУЛЬТИМЕДИА ЦЕНТРОМ С ПАТТЕРНОМ ФАСАД ===\n");

        HomeTheaterFacade emirTheater = new HomeTheaterFacade("Талдыкорган, мкрн 3, дом 15");
        HomeTheaterFacade aliTheater = new HomeTheaterFacade("Талдыкорган, мкрн 5, дом 22");
        HomeTheaterFacade amirTheater = new HomeTheaterFacade("Талдыкорган, мкрн 1, дом 8");

        Console.WriteLine("Молдахулов Эмир использует систему:");
        emirTheater.WatchMovie("Форсаж 10");
        emirTheater.Pause();
        emirTheater.SetVolume(25);
        emirTheater.Resume();
        emirTheater.TurnOffAll();

        Console.WriteLine(new string('=', 60) + "\n");

        Console.WriteLine("Кожабек Али использует систему:");
        aliTheater.PlayGame("Need for Speed");
        aliTheater.SetVolume(30);
        aliTheater.TurnOffAll();

        Console.WriteLine(new string('=', 60) + "\n");

        Console.WriteLine("Байжан Амир использует систему:");
        amirTheater.ListenToMusic();
        amirTheater.SetVolume(12);
        amirTheater.WatchTV(5);
        amirTheater.TurnOffAll();

        Console.WriteLine(new string('=', 60) + "\n");

        HomeTheaterFacade diasTheater = new HomeTheaterFacade("Талдыкорган, мкрн 2, дом 12");
        Console.WriteLine("Изатов Диас использует систему:");
        diasTheater.WatchMovie("Миссия невыполнима");
        diasTheater.TurnOffAll();

        Console.WriteLine(new string('=', 60) + "\n");

        HomeTheaterFacade kazimirTheater = new HomeTheaterFacade("Талдыкорган, мкрн 4, дом 18");
        Console.WriteLine("Казимир Казимирович использует систему:");
        kazimirTheater.WatchTV(8);
        kazimirTheater.SetVolume(22);
        kazimirTheater.TurnOffAll();

        Console.WriteLine(new string('=', 60) + "\n");

        HomeTheaterFacade dmitrySnowTheater = new HomeTheaterFacade("Талдыкорган, мкрн 6, дом 25");
        Console.WriteLine("Дмитрий Снег использует систему:");
        dmitrySnowTheater.PlayGame("FIFA 2024");
        dmitrySnowTheater.TurnOffAll();

        Console.WriteLine(new string('=', 60) + "\n");

        HomeTheaterFacade dmitryDovgeshkoTheater = new HomeTheaterFacade("Талдыкорган, мкрн 7, дом 33");
        Console.WriteLine("Дмитрий Довгешко использует систему:");
        dmitryDovgeshkoTheater.WatchMovie("Интерстеллар");
        dmitryDovgeshkoTheater.SetVolume(28);
        dmitryDovgeshkoTheater.TurnOffAll();

        Console.WriteLine("\nДемонстрация работы фасада завершена успешно!");
    }
}
