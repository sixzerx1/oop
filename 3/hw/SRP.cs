public class Order
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}

// Отдельный класс для расчета стоимости
public class PriceCalculator
{
    public double CalculateTotalPrice(Order order)
    {
        // Рассчет стоимости с учетом скидок
        return order.Quantity * order.Price * 0.9;
    }
}

// Отдельный класс для обработки платежей
public class PaymentProcessor
{
    public void ProcessPayment(string paymentDetails)
    {
        // Логика обработки платежа
        Console.WriteLine("Payment processed using: " + paymentDetails);
    }
}

// Отдельный класс для отправки уведомлений
public class NotificationService
{
    public void SendConfirmationEmail(string email)
    {
        // Логика отправки уведомления
        Console.WriteLine("Confirmation email sent to: " + email);
    }
}