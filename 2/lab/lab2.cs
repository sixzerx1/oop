using System;

namespace Lab2AppNamespace
{
    public class OrderService
    {
        public void CreateOrder(string productName, int quantity, double price)
        {
            PrintOrder(productName, quantity, price, "created");
        }

        public void UpdateOrder(string productName, int quantity, double price)
        {
            PrintOrder(productName, quantity, price, "updated");
        }

        private void PrintOrder(string productName, int quantity, double price, string action)
        {
            double totalPrice = quantity * price;
            Console.WriteLine($"Order for {productName} {action}. Total: {totalPrice}");
        }
    }

    public class Vehicle
    {
        public void Start()
        {
            Console.WriteLine($"{GetType().Name} is starting");
        }

        public void Stop()
        {
            Console.WriteLine($"{GetType().Name} is stopping");
        }
    }

    public class Car : Vehicle { }
    public class Truck : Vehicle { }

    public class Calculator
    {
        public void Add(int a, int b)
        {
            Console.WriteLine(a + b);
        }

        public void Subtract(int a, int b)
        {
            Console.WriteLine(a - b);
        }
    }

    public class Singleton
    {
        private static Singleton _instance;
        private Singleton() { }
        public static Singleton Instance => _instance ??= new Singleton();
        public void DoSomething()
        {
            Console.WriteLine("Doing something...");
        }
    }

    public class Client
    {
        public void Execute()
        {
            Singleton.Instance.DoSomething();
        }
    }

    public class MathOperations
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    class Lab2App
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== DRY: OrderService ===");
            var orderService = new OrderService();
            orderService.CreateOrder("Laptop", 2, 1200);
            orderService.UpdateOrder("Laptop", 3, 1200);

            Console.WriteLine("\n=== Vehicle: Car & Truck ===");
            var car = new Car();
            var truck = new Truck();
            car.Start();
            truck.Start();
            car.Stop();
            truck.Stop();

            Console.WriteLine("\n=== KISS: Calculator ===");
            var calc = new Calculator();
            calc.Add(5, 7);
            calc.Subtract(10, 3);

            Console.WriteLine("\n=== Singleton ===");
            var client = new Client();
            client.Execute();

            Console.WriteLine("\n=== YAGNI: MathOperations ===");
            var math = new MathOperations();
            Console.WriteLine("3 + 5 = " + math.Add(3, 5));
        }
    }
}