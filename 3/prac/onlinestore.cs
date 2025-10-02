using System;
using System.Collections.Generic;
using System.Linq;

// ИНТЕРФЕЙСЫ
public interface IPayment
{
    bool ProcessPayment(double amount);
}

public interface IDelivery
{
    void DeliverOrder(Order order);
    double CalculateDeliveryCost(Order order);
}

public interface INotification
{
    void SendNotification(string message);
}

public interface IDiscount
{
    double ApplyDiscount(Order order, double currentTotal);
}

// ПЛАТЕЖИ
public class CreditCardPayment : IPayment
{
    private readonly string _cardNumber;
    public CreditCardPayment(string cardNumber) => _cardNumber = cardNumber;
    public bool ProcessPayment(double amount)
    {
        Console.WriteLine($"Оплата картой {_cardNumber.Substring(0, 4)}... Сумма: ${amount}");
        return true;
    }
}

public class PayPalPayment : IPayment
{
    private readonly string _email;
    private object email;

    public PayPalPayment(string email) => _email = email;
    public bool ProcessPayment(double amount)
    {
        Console.WriteLine($"📧 PayPal оплата для {email}... Сумма: ${amount}");
        return true;
    }
}

// ДОСТАВКА
public class CourierDelivery : IDelivery
{
    public void DeliverOrder(Order order) =>
        Console.WriteLine($"Курьерская доставка заказа {order.OrderId}");
    public double CalculateDeliveryCost(Order order) => 10.0;
}

public class PickUpPointDelivery : IDelivery
{
    public void DeliverOrder(Order order) =>
        Console.WriteLine($"📦 Самовывоз заказа {order.OrderId}");
    public double CalculateDeliveryCost(Order order) => 0;
}

// УВЕДОМЛЕНИЯ
public class EmailNotification : INotification
{
    private readonly string _email;
    public EmailNotification(string email) => _email = email;
    public void SendNotification(string message) =>
        Console.WriteLine($"Email для {_email}: {message}");
}

public class SmsNotification : INotification
{
    private readonly string _phone;
    public SmsNotification(string phone) => _phone = phone;
    public void SendNotification(string message) =>
        Console.WriteLine($"SMS для {_phone}: {message}");
}

// СКИДКИ
public class PercentageDiscount : IDiscount
{
    private readonly double _percent;
    public PercentageDiscount(double percent) => _percent = percent;
    public double ApplyDiscount(Order order, double currentTotal)
    {
        var discount = currentTotal * (_percent / 100);
        Console.WriteLine($"Скидка {_percent}%: -${discount}");
        return currentTotal - discount;
    }
}

// ТОВАРЫ И ЗАКАЗЫ
public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public Product(string id, string name, double price)
    {
        Id = id; Name = name; Price = price;
    }
}

public class Order
{
    public string OrderId { get; }
    public List<Product> Products { get; }
    public IPayment Payment { get; set; }
    public IDelivery Delivery { get; set; }
    public List<INotification> Notifications { get; }

    public Order(string orderId)
    {
        OrderId = orderId;
        Products = new List<Product>();
        Notifications = new List<INotification>();
    }

    public void AddProduct(Product product) => Products.Add(product);
    public void AddNotification(INotification notification) => Notifications.Add(notification);

    public void ProcessOrder()
    {
        var total = Products.Sum(p => p.Price);
        Console.WriteLine($"\n=== ОБРАБОТКА ЗАКАЗА {OrderId} ===");
        Console.WriteLine($"Товаров: {Products.Count}");
        Console.WriteLine($"Общая сумма: ${total}");

        Payment?.ProcessPayment(total);
        Delivery?.DeliverOrder(this);

        foreach (var notification in Notifications)
            notification.SendNotification($"Заказ {OrderId} обработан!");
    }
}

// Ран
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("ЗАПУСК СИСТЕМЫ МАГАЗИНА\n");

        // Создаем товары
        var iPhone = new Product("1", "iPhone 15", 999.99);
        var airpods = new Product("2", "AirPods Pro", 249.99);

        // Создаем заказ
        var order = new Order("ORDER-67");
        order.AddProduct(iPhone);
        order.AddProduct(airpods);

        // Настраиваем платеж и доставку
        order.Payment = new CreditCardPayment("4400456894236");
        order.Delivery = new CourierDelivery();
        order.AddNotification(new EmailNotification("client@gmail.com"));
        order.AddNotification(new SmsNotification("+77777777777"));

        // Обрабатываем заказ
        order.ProcessOrder();

        Console.WriteLine("\nГОТОВО! SOLID принципы работают!");
    }
}