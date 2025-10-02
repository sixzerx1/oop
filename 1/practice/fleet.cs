public class Fleet
{
    private List<Garage> garages = new List<Garage>();

    public void AddGarage(Garage garage)
    {
        garages.Add(garage);
        Console.WriteLine($"Добавлен {garage}");
    }

    public void RemoveGarage(Garage garage)
    {
        garages.Remove(garage);
        Console.WriteLine($"Удалён {garage}");
    }

    public Vehicle? FindVehicle(string brand, string model)
    {
        foreach (var garage in garages)
        {
            foreach (var v in garage.GetVehicles())
            {
                if (v.Brand == brand && v.Model == model)
                {
                    return v;
                }
            }
        }
        return null;
    }

    public void ShowAll()
    {
        foreach (var garage in garages)
        {
            Console.WriteLine(garage);
            foreach (var v in garage.GetVehicles())
            {
                Console.WriteLine("   - " + v);
            }
        }
    }
}