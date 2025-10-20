using System;
using System.Collections.Generic;
using System.Linq;

namespace BehavioralPatternsModule07
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Name { get; }
    }

    public static class Logger
    {
        private static readonly List<string> _log = new();
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
            Name = name;
            _temp = initialTemp;
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
        public void Execute() => Logger.Info("NoCommand: slot not assigned.");
        public void Undo() => Logger.Info("NoCommand: nothing to undo.");
    }

    public class RemoteControl
    {
        private readonly ICommand[] _onCommands;
        private readonly ICommand[] _offCommands;
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();
        private readonly int _maxHistory;
        public RemoteControl(int slots = 7, int maxHistory = 50)
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
                _redoStack.Clear();
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
                _redoStack.Clear();
            }
            catch (Exception ex)
            {
                Logger.Info($"Error executing {cmd?.Name}: {ex.Message}");
            }
        }

        public void PressUndoButton()
        {
            if (_undoStack.Count == 0)
            {
                Logger.Info("Undo: history empty.");
                return;
            }
            var cmd = _undoStack.Pop();
            try
            {
                cmd.Undo();
                _redoStack.Push(cmd);
                Logger.Info($"Undo executed for {cmd.Name}");
            }
            catch (Exception ex)
            {
                Logger.Info($"Error undoing {cmd.Name}: {ex.Message}");
            }
        }

        public void PressRedoButton()
        {
            if (_redoStack.Count == 0)
            {
                Logger.Info("Redo: nothing to redo.");
                return;
            }
            var cmd = _redoStack.Pop();
            try
            {
                cmd.Execute();
                _undoStack.Push(cmd);
                Logger.Info($"Redo executed for {cmd.Name}");
            }
            catch (Exception ex)
            {
                Logger.Info($"Error redoing {cmd.Name}: {ex.Message}");
            }
        }

        public void PressUndoMultiple(int n)
        {
            if (n <= 0) return;
            for (int i = 0; i < n; i++)
            {
                if (_undoStack.Count == 0) { Logger.Info($"UndoMultiple: finished after {i} undos."); break; }
                PressUndoButton();
            }
        }

        private void PushHistory(ICommand cmd)
        {
            _undoStack.Push(cmd);
            while (_undoStack.Count > _maxHistory) _undoStack.ToList().RemoveAt(0);
        }

        private bool IsValidSlot(int slot)
        {
            if (slot < 0 || slot >= _onCommands.Length)
            {
                Logger.Info($"Slot {slot} is invalid (0..{_onCommands.Length - 1}).");
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

    public abstract class ReportGenerator
    {
        public void GenerateReport()
        {
            LoadData();
            FormatData();
            AddHeader();
            AddFooter();

            if (CustomerWantsSave())
                SaveToFile();
            else
                Console.WriteLine("Report not saved (maybe sent by email).");
        }

        protected abstract void LoadData();
        protected abstract void FormatData();
        protected abstract void AddHeader();
        protected abstract void AddFooter();

        protected virtual bool CustomerWantsSave() => true;
        protected virtual void SaveToFile() => Console.WriteLine("Saving report to file...");
    }

    public class PdfReport : ReportGenerator
    {
        protected override void LoadData() => Console.WriteLine("Loading data for PDF...");
        protected override void FormatData() => Console.WriteLine("Formatting PDF content...");
        protected override void AddHeader() => Console.WriteLine("Adding PDF header...");
        protected override void AddFooter() => Console.WriteLine("Adding PDF footer...");
    }

    public class ExcelReport : ReportGenerator
    {
        protected override void LoadData() => Console.WriteLine("Loading data for Excel...");
        protected override void FormatData() => Console.WriteLine("Formatting Excel tables...");
        protected override void AddHeader() => Console.WriteLine("Adding Excel header...");
        protected override void AddFooter() => Console.WriteLine("Adding Excel footer...");
        protected override void SaveToFile() => Console.WriteLine("Saving to .xlsx file...");
    }

    public class HtmlReport : ReportGenerator
    {
        protected override void LoadData() => Console.WriteLine("Loading data for HTML...");
        protected override void FormatData() => Console.WriteLine("Formatting HTML...");
        protected override void AddHeader() => Console.WriteLine("Adding HTML header...");
        protected override void AddFooter() => Console.WriteLine("Adding HTML footer...");
        protected override bool CustomerWantsSave() => false; 
    }

    public interface IMediator
    {
        void Register(User user);
        void Unregister(User user);
        void SendMessage(string message, User from, string channel, bool isPrivate = false, User to = null);
        void CreateChannel(string channel);
    }

    public class ChatMediator : IMediator
    {
        private readonly Dictionary<string, HashSet<User>> _channels = new();

        public void CreateChannel(string channel)
        {
            if (!_channels.ContainsKey(channel)) _channels[channel] = new HashSet<User>();
        }

        public void Register(User user)
        {
            if (user == null) return;
            CreateChannel(user.Channel);
            _channels[user.Channel].Add(user);
            user.SetMediator(this);
            BroadcastSystemMessage(user.Channel, $"{user.Name} joined channel {user.Channel}", except: user);
            Logger.Info($"ChatMediator: {user.Name} registered in {user.Channel}");
        }

        public void Unregister(User user)
        {
            if (user == null) return;
            if (_channels.ContainsKey(user.Channel) && _channels[user.Channel].Remove(user))
            {
                BroadcastSystemMessage(user.Channel, $"{user.Name} left channel {user.Channel}", except: user);
                user.SetMediator(null);
                Logger.Info($"ChatMediator: {user.Name} unregistered from {user.Channel}");
            }
        }

        public void SendMessage(string message, User from, string channel, bool isPrivate = false, User to = null)
        {
            if (from == null) return;
            if (!_channels.ContainsKey(channel))
            {
                Logger.Info($"ChatMediator: channel '{channel}' does not exist.");
                return;
            }
            if (!_channels[channel].Contains(from))
            {
                Logger.Info($"ChatMediator: {from.Name} is not part of channel '{channel}'.");
                return;
            }

            if (isPrivate)
            {
                if (to == null || !_channels[channel].Contains(to))
                {
                    Logger.Info($"ChatMediator: private recipient not found in '{channel}'.");
                    return;
                }
                to.Receive($"(Private) {from.Name}: {message}");
                Logger.Info($"ChatMediator: private from {from.Name} to {to.Name} in {channel}");
                return;
            }

            foreach (var u in _channels[channel])
            {
                if (u != from) u.Receive($"{from.Name}: {message}");
            }
            Logger.Info($"ChatMediator: broadcast from {from.Name} in {channel}");
        }

        private void BroadcastSystemMessage(string channel, string msg, User except = null)
        {
            if (!_channels.ContainsKey(channel)) return;
            foreach (var u in _channels[channel])
                if (u != except) u.Receive($"[System]: {msg}");
        }
    }

    public class User
    {
        public string Name { get; }
        public string Channel { get; private set; }
        private IMediator _mediator;
        public User(string name, string channel)
        {
            Name = name;
            Channel = channel;
        }
        public void SetMediator(IMediator mediator) => _mediator = mediator;
        public void Send(string message) => _mediator?.SendMessage(message, this, Channel);
        public void SendPrivate(string message, User to) => _mediator?.SendMessage(message, this, Channel, isPrivate: true, to: to);
        public void Receive(string message) => Console.WriteLine($"[{Name} receives] {message}");
        public void ChangeChannel(string newChannel)
        {
            if (_mediator == null) { Logger.Info($"{Name} not connected to mediator."); return; }
            _mediator.Unregister(this);
            Channel = newChannel;
            _mediator.Register(this);
        }
    }

    public static class Demo
    {
        public static void RunCommandDemo()
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

            var remote = new RemoteControl(slots: 4, maxHistory: 20);
            remote.SetCommand(0, livingOn, livingOff);
            remote.SetCommand(1, kitchenOn, kitchenOff);
            remote.SetCommand(2, tvOn, tvOff);

            var leaveHomeMacro = new MacroCommand(new ICommand[] { kitchenOff, livingOff, tvOff, acSet24, acEcoOn }, "LeaveHome");
            remote.SetCommand(3, leaveHomeMacro, new NoCommand());

            remote.PrintAssignedCommands();

            Console.WriteLine("\n-- Executing commands --");
            remote.PressOnButton(0);
            remote.PressOnButton(1);
            remote.PressOnButton(2);
            remote.PressOnButton(3);

            Console.WriteLine("\n-- Undo two last commands --");
            remote.PressUndoMultiple(2);

            Console.WriteLine("\n-- Attempt invalid slot and empty button --");
            remote.PressOnButton(9);
            remote.PressOffButton(3);

            Console.WriteLine();
        }

        public static void RunTemplateDemo()
        {
            Console.WriteLine("=== Template Method Demo (Report Generator) ===\n");
            ReportGenerator pdf = new PdfReport();
            ReportGenerator excel = new ExcelReport();
            ReportGenerator html = new HtmlReport();

            Console.WriteLine("- Generating PDF report:");
            pdf.GenerateReport();
            Console.WriteLine();

            Console.WriteLine("- Generating Excel report:");
            excel.GenerateReport();
            Console.WriteLine();

            Console.WriteLine("- Generating HTML report:");
            html.GenerateReport();
            Console.WriteLine();
        }

        public static void RunMediatorDemo()
        {
            Console.WriteLine("=== Mediator (Chat) Demo ===\n");
            var mediator = new ChatMediator();
            var alice = new User("Alice", "general");
            var bob = new User("Bob", "general");
            var charlie = new User("Charlie", "music");

            mediator.Register(alice);
            mediator.Register(bob);
            mediator.Register(charlie);

            alice.Send("Hi everyone!");
            bob.Send("Hello Alice!");
            charlie.Send("Who likes music?");

            Console.WriteLine("\n-- Private message demo --");
            charlie.SendPrivate("Hey Bob, private question.", bob);

            Console.WriteLine("\n-- Change channel demo --");
            bob.ChangeChannel("music");
            bob.Send("Now I'm in music channel.");

            Console.WriteLine("\n-- Channel not exists demo --");
            var dave = new User("Dave", "sports");
            dave.Send("Am I in the chat?");

            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Demo.RunCommandDemo();
            Demo.RunTemplateDemo();
            Demo.RunMediatorDemo();

            Console.WriteLine("=== Demo complete ===\n");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}