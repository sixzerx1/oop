public class Garage
{
    public string Name { get; set; }
    private List<Vehicle> vehicles = new List<Vehicle>();

    public Garage(string name)
    {
        Name = name;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        vehicles.Add(vehicle);
        Console.WriteLine($"В {Name} добавлено: {vehicle}");
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        vehicles.Remove(vehicle);
        Console.WriteLine($"Из {Name} удалено: {vehicle}");
    }

    public List<Vehicle> GetVehicles()
    {
        return vehicles;
    }

    public override string ToString()
    {
        return $"Гараж: {Name}, Транспортных средств: {vehicles.Count}";
    }
}