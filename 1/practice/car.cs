public class Car : Vehicle
{
    public int Doors { get; set; }
    public string Transmission { get; set; }

    public Car(string brand, string model, int year, int doors, string transmission)
        : base(brand, model, year)
    {
        Doors = doors;
        Transmission = transmission;
    }

    public override string ToString()
    {
        return base.ToString() + $" | Дверей: {Doors}, КПП: {Transmission}";
    }
}