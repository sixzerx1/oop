using System;

public interface ITransport

{
    void Move();
    void FuelUp();
}

public class Car : ITransport
{
    public void Move() => Console.WriteLine("Машина едет.");
    public void FuelUp() => Console.WriteLine("Машина заправляется бензином.");
}

public class Motorcycle : ITransport
{
    public void Move() => Console.WriteLine("Мотоцикл едет.");
    public void FuelUp() => Console.WriteLine("Мотоцикл заправляется бензином.");
}

public class Plane : ITransport
{
    public void Move() => Console.WriteLine("Самолет летит.");
    public void FuelUp() => Console.WriteLine("Самолет заправляется керосином.");
}

public class Bicycle : ITransport
{
    public void Move() => Console.WriteLine("Велосипед едет.");
    public void FuelUp() => Console.WriteLine("Велосипеду топливо не нужно.");
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Выберите транспорт: car, motorcycle, plane, bicycle");
        string choice = Console.ReadLine();

        ITransport t = choice switch
        {
            "car" => new Car(),
            "motorcycle" => new Motorcycle(),
            "plane" => new Plane(),
            "bicycle" => new Bicycle(),
            _ => null
        };

        if (t == null)
        {
            Console.WriteLine("Неизвестный транспорт.");
            return;
        }

        t.Move();
        t.FuelUp();
    }
}