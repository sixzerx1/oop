using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Введите car или motorcycle:");
        string choice = Console.ReadLine();

        VehicleFactory factory = choice == "car"
            ? new CarFactory()
            : new MotorcycleFactory();

        IVehicle v = factory.Create();
        v.Drive();
    }
}