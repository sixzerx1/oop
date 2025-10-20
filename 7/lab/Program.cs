using System;
using System.Collections.Generic;
using System.Linq;

public interface ICommand
{
    void Execute();
    void Undo();
    string Name { get; }
}

public static class Logger
{
    private static readonly List<string> _log = new List<string>();
    public static void Info(string msg)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}";
        _log.Add(entry);
        Console.WriteLine(entry);
    }
    public static IEnumerable<string> GetLog() => _log.AsReadOnly();
}

public class Light
{
    public string Location { get; }
    private bool _isOn;
    public Light(string location) => Location = location;
    public void On()
    {
        _isOn = true;
        Logger.Info($"Light '{Location}' ON.");
    }
    public void Off()
    {
        _isOn = false;
        Logger.Info($"Light '{Location}' OFF.");
    }
    public bool IsOn => _isOn;
}

public class Television
{
    public string Model { get; }
    private bool _isOn;
    public Television(string model) => Model = model;
    public void On()
    {
        _isOn = true;
        Logger.Info($"TV '{Model}' ON.");
    }
    public void Off()
    {
        _isOn = false;
        Logger.Info($"TV '{Model}' OFF.");
    }
    public bool IsOn => _isOn;
}

public class AirConditioner
{
    public string Name { get; }
    private bool _isOn;
    private double _temp;
    private bool _ecoMode;
    public AirConditioner(string name, double initialTemp = 24.0)
    {
        Name = name; _temp = initialTemp;
    }
    public void On()
    {
        _isOn = true;
        Logger.Info($"AC '{Name}' ON. Temp={_temp:F1}°C EcoMode={_ecoMode}");
    }
    public void Off()
    {
        _isOn = false;
        Logger.Info($"AC '{Name}' OFF.");
    }
    public void SetTemperature(double t)
    {
        _temp = t;
        Logger.Info($"AC '{Name}' set temperature to {_temp:F1}°C");
    }
    public void SetEcoMode(bool on)
    {
        _ecoMode = on;
        Logger.Info($"AC '{Name}' eco mode {(on ? "ENABLED" : "DISABLED")}");
    }
    public bool IsOn => _isOn;
}

public class LightOnCommand : ICommand
{
    private readonly Light _light;
    public string Name => $"LightOn({_light.Location})";
    public LightOnCommand(Light light) => _light = light;
    public void Execute() => _light.On();
    public void Undo() => _light.Off();
}

public class LightOffCommand : ICommand
{
    private readonly Light _light;
    public string Name => $"LightOff({_light.Location})";
    public LightOffCommand(Light light) => _light = light;
    public void Execute() => _light.Off();
    public void Undo() => _light.On();
}

public class TVOnCommand : ICommand
{
    private readonly Television _tv;
    public string Name => $"TVOn({_tv.Model})";
    public TVOnCommand(Television tv) => _tv = tv;
    public void Execute() => _tv.On();
    public void Undo() => _tv.Off();
}

public class TVOffCommand : ICommand
{
    private readonly Television _tv;
    public string Name => $"TVOff({_tv.Model})";
    public TVOffCommand(Television tv) => _tv = tv;
    public void Execute() => _tv.Off();
    public void Undo() => _tv.On();
}

public class ACOnCommand : ICommand
{
    private readonly AirConditioner _ac;
    public string Name => $"ACOn({_ac.Name})";
    public ACOnCommand(AirConditioner ac) => _ac = ac;
    public void Execute() => _ac.On();
    public void Undo() => _ac.Off();
}

public class ACOffCommand : ICommand
{
    private readonly AirConditioner _ac;
    public string Name => $"ACOff({_ac.Name})";
    public ACOffCommand(AirConditioner ac) => _ac = ac;
    public void Execute() => _ac.Off();
    public void Undo() => _ac.On();
}

public class ACSetTempCommand : ICommand
{
    private readonly AirConditioner _ac;
    private readonly double _newTemp;
    private double _previousTemp;
    public string Name => $"ACSetTemp({_ac.Name}, {_newTemp:F1})";
    public ACSetTempCommand(AirConditioner ac, double temp) { _ac = ac; _newTemp = temp; }
    public void Execute()
    {
        _previousTemp = _newTemp - 1.0;
        _ac.SetTemperature(_newTemp);
    }
    public void Undo() => _ac.SetTemperature(_previousTemp);
}

public class ACSetEcoModeCommand : ICommand
{
    private readonly AirConditioner _ac;
    private readonly bool _eco;
    public string Name => $"ACEco({_ac.Name} => {(_eco ? "ON" : "OFF")})";
    public ACSetEcoModeCommand(AirConditioner ac, bool eco) { _ac = ac; _eco = eco; }
    public void Execute() => _ac.SetEcoMode(_eco);
    public void Undo() => _ac.SetEcoMode(!_eco);
}

public class MacroCommand : ICommand
{
    private readonly IList<ICommand> _commands;
    public string Name { get; }
    public MacroCommand(IEnumerable<ICommand> commands, string name = "Macro") 
    { 
        _commands = commands?.ToList() ?? new List<ICommand>();
        Name = name;
    }
    public void Execute()
    {
        Logger.Info($"Executing macro '{Name}' with {_commands.Count} commands.");
        foreach (var c in _commands) c.Execute();
    }
    public void Undo()
    {
        Logger.Info($"Undoing macro '{Name}'.");
        for (int i = _commands.Count - 1; i >= 0; i--) _commands[i].Undo();
    }
}

public class NoCommand : ICommand
{
    public string Name => "NoCommand";
    public void Execute() => Logger.Info("NoCommand: кнопка не назначена.");
    public void Undo() => Logger.Info("NoCommand: нет команды для отмены.");
}

public class RemoteControl
{
    private readonly ICommand[] _onCommands;
    private readonly ICommand[] _offCommands;
    private readonly Stack<ICommand> _history = new Stack<ICommand>();
    private readonly int _maxHistory;
    public RemoteControl(int slots = 7, int maxHistory = 20)
    {
        _onCommands = Enumerable.Repeat<ICommand>(new NoCommand(), slots).ToArray();
        _offCommands = Enumerable.Repeat<ICommand>(new NoCommand(), slots).ToArray();
        _maxHistory = Math.Max(1, maxHistory);
    }

    public void SetCommand(int slot, ICommand onCommand, ICommand offCommand)
    {
        if (slot < 0 || slot >= _onCommands.Length) throw new ArgumentOutOfRangeException(nameof(slot));
        _onCommands[slot] = onCommand ?? new NoCommand();
        _offCommands[slot] = offCommand ?? new NoCommand();
        Logger.Info($"Remote: Set slot {slot} ON={_onCommands[slot].Name} OFF={_offCommands[slot].Name}");
    }

    public void PressOnButton(int slot)
    {
        if (!IsValidSlot(slot)) return;
        var cmd = _onCommands[slot];
        try
        {
            cmd.Execute();
            PushHistory(cmd);
        }
        catch (Exception ex)
        {
            Logger.Info($"Error executing {cmd?.Name}: {ex.Message}");
        }
    }

    public void PressOffButton(int slot)
    {
        if (!IsValidSlot(slot)) return;
        var cmd = _offCommands[slot];
        try
        {
            cmd.Execute();
            PushHistory(cmd);
        }
        catch (Exception ex)
        {
            Logger.Info($"Error executing {cmd?.Name}: {ex.Message}");
        }
    }

    public void PressUndoButton()
    {
        if (_history.Count == 0)
        {
            Logger.Info("Undo: история пуста.");
            return;
        }
        var cmd = _history.Pop();
        try
        {
            cmd.Undo();
            Logger.Info($"Undo executed for {cmd.Name}");
        }
        catch (Exception ex)
        {
            Logger.Info($"Error undoing {cmd.Name}: {ex.Message}");
        }
    }

    public void PressUndoMultiple(int n)
    {
        if (n <= 0) return;
        for (int i = 0; i < n; i++)
        {
            if (_history.Count == 0) { Logger.Info($"UndoMultiple: история закончилась после {i} отмен."); break; }
            PressUndoButton();
        }
    }

    private void PushHistory(ICommand cmd)
    {
        _history.Push(cmd);
        while (_history.Count > _maxHistory) _history.ToList().RemoveAt(0);
    }

    private bool IsValidSlot(int slot)
    {
        if (slot < 0 || slot >= _onCommands.Length)
        {
            Logger.Info($"Slot {slot} не существует (доступно 0..{_onCommands.Length - 1}).");
            return false;
        }
        return true;
    }

    public void PrintAssignedCommands()
    {
        Logger.Info("Remote assignments:");
        for (int i = 0; i < _onCommands.Length; i++)
            Console.WriteLine($" Slot {i}: ON={_onCommands[i].Name}, OFF={_offCommands[i].Name}");
    }
}

public abstract class Beverage
{
    public void PrepareRecipe()
    {
        BoilWater();
        Brew();
        PourInCup();
        if (CustomerWantsCondiments())
            AddCondiments();
        Finish();
    }

    protected void BoilWater() => Console.WriteLine("Кипячение воды...");
    protected void PourInCup() => Console.WriteLine("Наливание в чашку...");
    protected void Finish() => Console.WriteLine("Напиток готов!\n");

    protected abstract void Brew();
    protected abstract void AddCondiments();

    protected virtual bool CustomerWantsCondiments()
    {
        return true;
    }
}

public class Tea : Beverage
{
    protected override void Brew() => Console.WriteLine("Заваривание чая...");
    protected override void AddCondiments() => Console.WriteLine("Добавление лимона...");
}

public class Coffee : Beverage
{
    private readonly Func<string> _inputProvider;
    public Coffee(Func<string> inputProvider = null) => _inputProvider = inputProvider ?? (() => Console.ReadLine());
    protected override void Brew() => Console.WriteLine("Заваривание кофе...");
    protected override void AddCondiments() => Console.WriteLine("Добавление сахара и молока...");
    protected override bool CustomerWantsCondiments()
    {
        Console.Write("Добавить добавки в кофе? (yes/no): ");
        var answer = _inputProvider()?.Trim().ToLower() ?? "no";
        int tries = 0;
        while (answer != "yes" && answer != "no" && tries < 2)
        {
            Console.Write("Пожалуйста, введите 'yes' или 'no': ");
            answer = _inputProvider()?.Trim().ToLower() ?? "no";
            tries++;
        }
        if (answer != "yes" && answer != "no")
        {
            Console.WriteLine("Неверный ввод — добавки не будут добавлены.");
            return false;
        }
        return answer == "yes";
    }
}

public class HotChocolate : Beverage
{
    protected override void Brew() => Console.WriteLine("Смешивание какао и горячего молока...");
    protected override void AddCondiments() => Console.WriteLine("Добавление взбитых сливок и маршмеллоу...");
    protected override bool CustomerWantsCondiments()
    {
        Console.Write("Хотите добавки в горячий шоколад? (yes/no): ");
        var ans = Console.ReadLine()?.Trim().ToLower() ?? "no";
        return ans == "yes";
    }
}

public interface IMediator
{
    void Register(User user);
    void Unregister(User user);
    void SendMessage(string message, User from, bool isPrivate = false, User to = null);
}

public class ChatRoom : IMediator
{
    private readonly HashSet<User> _users = new HashSet<User>();
    public void Register(User user)
    {
        if (user == null) return;
        if (_users.Add(user))
        {
            user.SetMediator(this);
            BroadcastSystemMessage($"{user.Name} присоединился к чату.", user);
            Logger.Info($"ChatRoom: {user.Name} registered.");
        }
    }
    public void Unregister(User user)
    {
        if (user == null) return;
        if (_users.Remove(user))
        {
            BroadcastSystemMessage($"{user.Name} покинул чат.", user);
            user.SetMediator(null);
            Logger.Info($"ChatRoom: {user.Name} unregistered.");
        }
    }
    public void SendMessage(string message, User from, bool isPrivate = false, User to = null)
    {
        if (!_users.Contains(from))
        {
            Logger.Info($"ChatRoom: Ошибка — {from.Name} не в чате.");
            return;
        }
        if (isPrivate)
        {
            if (to == null || !_users.Contains(to))
            {
                Logger.Info($"ChatRoom: Ошибка — приватный получатель не найден.");
                return;
            }
            to.Receive($"(Private) {from.Name}: {message}");
            Logger.Info($"ChatRoom: Private from {from.Name} to {to.Name}");
            return;
        }
        foreach (var u in _users)
        {
            if (u != from) u.Receive($"{from.Name}: {message}");
        }
        Logger.Info($"ChatRoom: Broadcast from {from.Name}");
    }
    private void BroadcastSystemMessage(string msg, User except = null)
    {
        foreach (var u in _users) if (u != except) u.Receive($"[System]: {msg}");
    }
}

public class User
{
    public string Name { get; }
    private IMediator _mediator;
    public User(string name) => Name = name;
    public void SetMediator(IMediator m) => _mediator = m;
    public void Send(string message)
    {
        if (_mediator == null) { Logger.Info($"{Name} не подключён к чату."); return; }
        _mediator.SendMessage(message, this);
    }
    public void SendPrivate(string message, User to)
    {
        if (_mediator == null) { Logger.Info($"{Name} не подключён к чату."); return; }
        _mediator.SendMessage(message, this, isPrivate: true, to: to);
    }
    public void Receive(string message) => Console.WriteLine($"[{Name} receives] {message}");
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Command Pattern (Remote Control) Demo ===\n");

        var livingLight = new Light("Living Room");
        var kitchenLight = new Light("Kitchen");
        var tv = new Television("Samsung-55");
        var ac = new AirConditioner("BedroomAC");

        var livingOn = new LightOnCommand(livingLight);
        var livingOff = new LightOffCommand(livingLight);
        var kitchenOn = new LightOnCommand(kitchenLight);
        var kitchenOff = new LightOffCommand(kitchenLight);

        var tvOn = new TVOnCommand(tv);
        var tvOff = new TVOffCommand(tv);

        var acOn = new ACOnCommand(ac);
        var acOff = new ACOffCommand(ac);
        var acSet24 = new ACSetTempCommand(ac, 24.0);
        var acEcoOn = new ACSetEcoModeCommand(ac, true);

        var remote = new RemoteControl(slots: 4, maxHistory: 10);
        remote.SetCommand(0, livingOn, livingOff);
        remote.SetCommand(1, kitchenOn, kitchenOff);
        remote.SetCommand(2, tvOn, tvOff);
        var leaveHomeMacro = new MacroCommand(new ICommand[] { kitchenOff, livingOff, tvOff, acSet24, acEcoOn }, "LeaveHome");
        remote.SetCommand(3, leaveHomeMacro, new NoCommand());

        remote.PrintAssignedCommands();

        Console.WriteLine("\n-- Выполнение команд --");
        remote.PressOnButton(0);
        remote.PressOnButton(1);
        remote.PressOnButton(2); 
        remote.PressOnButton(3);

        Console.WriteLine("\n-- Отмена двух последних команд --");
        remote.PressUndoMultiple(2);

        Console.WriteLine("\n-- Попытка использовать несуществующий слот и пустую кнопку --");
        remote.PressOnButton(9);
        remote.PressOffButton(3);

        Console.WriteLine("\n=== Template Method Demo ===\n");
        Beverage tea = new Tea();
        Console.WriteLine("- Готовим чай:");
        tea.PrepareRecipe();

        Beverage coffee = new Coffee(() => { Console.WriteLine("(Simulated input) yes"); return "yes"; });
        Console.WriteLine("- Готовим кофе (симулированный ввод yes):");
        coffee.PrepareRecipe();

        Beverage choc = new HotChocolate();
        Console.WriteLine("- Готовим горячий шоколад (введите yes/no):");
        choc.PrepareRecipe();

        Console.WriteLine("\n=== Mediator (ChatRoom) Demo ===\n");
        var chat = new ChatRoom();
        var alice = new User("Alice");
        var bob = new User("Bob");
        var charlie = new User("Charlie");

        chat.Register(alice);
        chat.Register(bob);
        chat.Register(charlie);

        alice.Send("Привет всем!");
        bob.Send("Привет, Alice!");
        charlie.SendPrivate("Привет, Bob — приватно.", bob);

        var outsider = new User("Outsider");
        outsider.Send("Пытаюсь писать в чат...");

        chat.Unregister(charlie);
        bob.Send("Где Чарли?");

        Console.WriteLine("\n=== Лог команд (последние записи) ===");
        foreach (var entry in Logger.GetLog().TakeLast(15)) Console.WriteLine(entry);

        Console.WriteLine("\n=== Demo complete ===");
    }
}
