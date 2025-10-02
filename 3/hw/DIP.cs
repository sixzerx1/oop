public interface INotificationSender
{
    void Send(string message);
}

// Конкретные реализации
public class EmailSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine("Email sent: " + message);
    }
}

public class SmsSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine("SMS sent: " + message);
    }
}

public class PushNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine("Push notification sent: " + message);
    }
}

// Сервис уведомлений зависит от абстракций
public class NotificationService
{
    private readonly List<INotificationSender> _senders;

    public NotificationService(params INotificationSender[] senders)
    {
        _senders = new List<INotificationSender>(senders);
    }

    public void AddSender(INotificationSender sender)
    {
        _senders.Add(sender);
    }

    public void SendNotification(string message)
    {
        foreach (var sender in _senders)
        {
            sender.Send(message);
        }
    }
}