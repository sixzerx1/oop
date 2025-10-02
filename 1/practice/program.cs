class Program
{
    static void Main(string[] args)
    {
        // Создаём транспорт
        Car car1 = new Car("Toyota", "Camry", 2020, 4, "Автомат");
        Car car2 = new Car("BMW", "X5", 2021, 5, "Механика");
        Motorcycle moto1 = new Motorcycle("Yamaha", "MT-07", 2019, "Спорт", true);

        Garage garage1 = new Garage("Гараж №1");
        Garage garage2 = new Garage("Гараж №2");

        garage1.AddVehicle(car1);
        garage1.AddVehicle(moto1);
        garage2.AddVehicle(car2);

        Fleet fleet = new Fleet();
        fleet.AddGarage(garage1);
        fleet.AddGarage(garage2);

        Console.WriteLine("\n--- Все гаражи и транспорт ---");
        fleet.ShowAll();

        Console.WriteLine("\n--- Поиск транспорта ---");
        var found = fleet.FindVehicle("BMW", "X5");
        Console.WriteLine(found != null ? $"Найден: {found}" : "Не найдено");
    }
}