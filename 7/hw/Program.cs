using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


interface ICommand
{
    void Execute();
    void Undo();
    string Name { get; }
}

class Light
{
    public string Location { get; }
    private bool _isOn = false;
    public Light(string location) => Location = location;
    public void On()
    {
        _isOn = true;
        Console.WriteLine($"[Light] {Location} turned ON.");
    }
    public void Off()
    {
        _isOn = false;
        Console.WriteLine($"[Light] {Location} turned OFF.");
    }
    public bool IsOn => _isOn;
}

class Door
{
    public string Name { get; }
    private bool _isOpen = false;
    public Door(string name) => Name = name;
    public void Open()
    {
        _isOpen = true;
        Console.WriteLine($"[Door] {Name} opened.");
    }
    public void Close()
    {
        _isOpen = false;
        Console.WriteLine($"[Door] {Name} closed.");
    }
    public bool IsOpen => _isOpen;
}

class Thermostat
{
    public double Temperature { get; private set; }
    public Thermostat(double initial = 22.0) => Temperature = initial;
    public void Increase(double delta)
    {
        Temperature += delta;
        Console.WriteLine($"[Thermostat] Temperature increased by {delta:F1}° -> {Temperature:F1}°C");
    }
    public void Decrease(double delta)
    {
        Temperature -= delta;
        Console.WriteLine($"[Thermostat] Temperature decreased by {delta:F1}° -> {Temperature:F1}°C");
    }
}

class LightOnCommand : ICommand
{
    private Light _light;
    public string Name => $"LightOn({_light.Location})";
    public LightOnCommand(Light light) => _light = light;
    public void Execute() => _light.On();
    public void Undo() => _light.Off();
}

class LightOffCommand : ICommand
{
    private Light _light;
    public string Name => $"LightOff({_light.Location})";
    public LightOffCommand(Light light) => _light = light;
    public void Execute() => _light.Off();
    public void Undo() => _light.On();
}

class DoorOpenCommand : ICommand
{
    private Door _door;
    public string Name => $"DoorOpen({_door.Name})";
    public DoorOpenCommand(Door door) => _door = door;
    public void Execute() => _door.Open();
    public void Undo() => _door.Close();
}

class DoorCloseCommand : ICommand
{
    private Door _door;
    public string Name => $"DoorClose({_door.Name})";
    public DoorCloseCommand(Door door) => _door = door;
    public void Execute() => _door.Close();
    public void Undo() => _door.Open();
}

class IncreaseTempCommand : ICommand
{
    private Thermostat _thermo;
    private double _delta;
    public string Name => $"IncreaseTemp({ _delta })";
    public IncreaseTempCommand(Thermostat t, double delta) { _thermo = t; _delta = delta; }
    public void Execute() => _thermo.Increase(_delta);
    public void Undo() => _thermo.Decrease(_delta);
}

class DecreaseTempCommand : ICommand
{
    private Thermostat _thermo;
    private double _delta;
    public string Name => $"DecreaseTemp({ _delta })";
    public DecreaseTempCommand(Thermostat t, double delta) { _thermo = t; _delta = delta; }
    public void Execute() => _thermo.Decrease(_delta);
    public void Undo() => _thermo.Increase(_delta);
}

class TV
{
    public bool IsOn { get; private set; } = false;
    public void TurnOn() { IsOn = true; Console.WriteLine("[TV] TV turned ON."); }
    public void TurnOff() { IsOn = false; Console.WriteLine("[TV] TV turned OFF."); }
}
class TVOnCommand : ICommand
{
    private TV _tv;
    public string Name => "TVOn";
    public TVOnCommand(TV tv) => _tv = tv;
    public void Execute() => _tv.TurnOn();
    public void Undo() => _tv.TurnOff();
}
class TVOffCommand : ICommand
{
    private TV _tv;
    public string Name => "TVOff";
    public TVOffCommand(TV tv) => _tv = tv;
    public void Execute() => _tv.TurnOff();
    public void Undo() => _tv.TurnOn();
}

class Invoker
{
    private readonly Stack<ICommand> _history = new Stack<ICommand>();
    private readonly int _maxHistory;
    public Invoker(int maxHistory = 20) { _maxHistory = Math.Max(1, maxHistory); }

    public void ExecuteCommand(ICommand cmd)
    {
        try
        {
            cmd.Execute();
            _history.Push(cmd);
            while (_history.Count > _maxHistory) _history.ToList().RemoveAt(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Invoker] Error executing command {cmd.Name}: {ex.Message}");
        }
    }

    public void UndoLast()
    {
        if (_history.Count == 0)
        {
            Console.WriteLine("[Invoker] No commands to undo.");
            return;
        }
        var cmd = _history.Pop();
        try
        {
            cmd.Undo();
            Console.WriteLine($"[Invoker] Undid: {cmd.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Invoker] Error undoing command {cmd.Name}: {ex.Message}");
        }
    }

    public void UndoMultiple(int n)
    {
        if (n <= 0) return;
        for (int i = 0; i < n; i++)
        {
            if (_history.Count == 0)
            {
                Console.WriteLine($"[Invoker] Nothing left to undo after {i} undos.");
                break;
            }
            UndoLast();
        }
    }

    public void PrintHistory()
    {
        Console.WriteLine("[Invoker] Command history (most recent first):");
        foreach (var cmd in _history)
        {
            Console.WriteLine(" - " + cmd.Name);
        }
    }
}

abstract class Beverage
{
    public void PrepareRecipe()
    {
        BoilWater();
        BrewOrSteep();
        PourInCup();
        if (CustomerWantsCondiments())
        {
            AddCondiments();
        }
        Finish();
    }

    protected void BoilWater() => Console.WriteLine("Boiling water...");
    protected void PourInCup() => Console.WriteLine("Pouring into cup...");
    protected void Finish() => Console.WriteLine("Your drink is ready!\n");

    protected abstract void BrewOrSteep();
    protected abstract void AddCondiments();

    protected virtual bool CustomerWantsCondiments()
    {
        return true;
    }
}

class Tea : Beverage
{
    protected override void BrewOrSteep() => Console.WriteLine("Steeping the tea...");
    protected override void AddCondiments() => Console.WriteLine("Adding lemon...");

}

class Coffee : Beverage
{
    private readonly Func<string> _userInputProvider;
    public Coffee(Func<string> userInputProvider = null)
    {
        _userInputProvider = userInputProvider ?? (() => Console.ReadLine());
    }
    protected override void BrewOrSteep() => Console.WriteLine("Brewing the coffee...");
    protected override void AddCondiments() => Console.WriteLine("Adding sugar and milk...");

    protected override bool CustomerWantsCondiments()
    {
        Console.Write("Would you like condiments in your coffee? (yes/no): ");
        string input = _userInputProvider().Trim().ToLower();
        int attempts = 0;
        while (input != "yes" && input != "no" && attempts < 2)
        {
            Console.Write("Please answer 'yes' or 'no': ");
            input = _userInputProvider().Trim().ToLower();
            attempts++;
        }
        if (input == "yes") return true;
        if (input == "no") return false;
        Console.WriteLine("[Coffee] Invalid response; assuming 'no'.");
        return false;
    }
}

class HotChocolate : Beverage
{
    protected override void BrewOrSteep() => Console.WriteLine("Mixing cocoa and hot milk...");
    protected override void AddCondiments() => Console.WriteLine("Adding whipped cream and marshmallows...");

    protected override bool CustomerWantsCondiments()
    {
        Console.Write("Add toppings to hot chocolate? (yes/no): ");
        var input = Console.ReadLine()?.Trim().ToLower() ?? "no";
        return input == "yes";
    }
}
interface IMediator
{
    void Register(User user);
    void Unregister(User user);
    void SendMessage(string message, User from, bool isPrivate = false, User to = null);
}

class ChatRoom : IMediator
{
    private readonly HashSet<User> _users = new HashSet<User>();

    public void Register(User user)
    {
        if (_users.Add(user))
        {
            user.SetMediator(this);
            BroadcastSystemMessage($"{user.Name} has joined the chat.", user);
            Console.WriteLine($"[ChatRoom] {user.Name} registered.");
        }
    }

    public void Unregister(User user)
    {
        if (_users.Remove(user))
        {
            BroadcastSystemMessage($"{user.Name} has left the chat.", user);
            user.SetMediator(null);
            Console.WriteLine($"[ChatRoom] {user.Name} unregistered.");
        }
    }

    public void SendMessage(string message, User from, bool isPrivate = false, User to = null)
    {
        if (!_users.Contains(from))
        {
            Console.WriteLine("[ChatRoom] Error: Sender is not part of this chat.");
            return;
        }

        if (isPrivate)
        {
            if (to == null || !_users.Contains(to))
            {
                Console.WriteLine("[ChatRoom] Error: Private message recipient not found in chat.");
                return;
            }
            to.Receive($"(Private) {from.Name}: {message}");
            Console.WriteLine($"[ChatRoom] Private message sent from {from.Name} to {to.Name}");
            return;
        }

        foreach (var user in _users)
        {
            if (user != from)
            {
                user.Receive($"{from.Name}: {message}");
            }
        }
        Console.WriteLine($"[ChatRoom] {from.Name} broadcasted message.");
    }

    private void BroadcastSystemMessage(string sysMsg, User except = null)
    {
        foreach (var user in _users)
        {
            if (user != except)
                user.Receive($"[System]: {sysMsg}");
        }
    }
}

class User
{
    public string Name { get; }
    private IMediator _mediator;
    public User(string name) => Name = name;
    public void SetMediator(IMediator mediator) => _mediator = mediator;

    public void Send(string message)
    {
        if (_mediator == null) { Console.WriteLine("[User] Error: You are not connected to a chat."); return; }
        _mediator.SendMessage(message, this);
    }

    public void SendPrivate(string message, User to)
    {
        if (_mediator == null) { Console.WriteLine("[User] Error: You are not connected to a chat."); return; }
        _mediator.SendMessage(message, this, isPrivate: true, to: to);
    }

    public void Receive(string message)
    {
        Console.WriteLine($"[{Name} receives] {message}");
    }
}


class Program
{
    static void Main()
    {
        Console.WriteLine("=== DEMONSTRATION: Command Pattern ===");
        var livingLight = new Light("Living Room");
        var kitchenLight = new Light("Kitchen");
        var frontDoor = new Door("Front Door");
        var bedroomThermo = new Thermostat(21.5);
        var tv = new TV();

        var invoker = new Invoker(maxHistory: 10);

        invoker.ExecuteCommand(new LightOnCommand(livingLight));
        invoker.ExecuteCommand(new LightOnCommand(kitchenLight));
        invoker.ExecuteCommand(new DoorOpenCommand(frontDoor));
        invoker.ExecuteCommand(new IncreaseTempCommand(bedroomThermo, 1.0));
        invoker.ExecuteCommand(new TVOnCommand(tv));

        Console.WriteLine();
        invoker.PrintHistory();
        Console.WriteLine();

        Console.WriteLine("Undo last 2 commands:");
        invoker.UndoMultiple(2);
        Console.WriteLine();

        Console.WriteLine("Undo many commands (more than present):");
        invoker.UndoMultiple(10);
        Console.WriteLine();

        invoker.UndoLast();
        Console.WriteLine();

        Console.WriteLine("=== DEMONSTRATION: Template Method ===");
        Beverage tea = new Tea();
        Console.WriteLine("Preparing tea:");
        tea.PrepareRecipe();

        Beverage coffee = new Coffee(() =>
        {
            Console.WriteLine("(Simulated input) yes");
            return "yes";
        });
        Console.WriteLine("Preparing coffee:");
        coffee.PrepareRecipe();

        Beverage choc = new HotChocolate();
        Console.WriteLine("Preparing hot chocolate:");
        choc.PrepareRecipe();

        Console.WriteLine("=== DEMONSTRATION: Mediator (ChatRoom) ===");
        var chat = new ChatRoom();
        var alice = new User("Alice");
        var bob = new User("Bob");
        var charlie = new User("Charlie");

        chat.Register(alice);
        chat.Register(bob);
        chat.Register(charlie);

        alice.Send("Hi everyone!");
        bob.Send("Hello Alice!");
        charlie.SendPrivate("Hey Bob, are you free later?", bob);

        var stranger = new User("Stranger");
        stranger.Send("Am I in the chat?");

        chat.Unregister(charlie);
        bob.Send("Looks like Charlie left.");

        Console.WriteLine("\n=== DEMONSTRATION COMPLETE ===");
    }
}

