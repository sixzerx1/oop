public class Motorcycle : Vehicle
{
    public string BodyType { get; set; }
    public bool HasBox { get; set; }

    public Motorcycle(string brand, string model, int year, string bodyType, bool hasBox)
        : base(brand, model, year)
    {
        BodyType = bodyType;
        HasBox = hasBox;
    }

    public override string ToString()
    {
        return base.ToString() + $" | Тип: {BodyType}, Бокс: {(HasBox ? "Есть" : "Нет")}";
    }
}