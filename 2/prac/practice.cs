using System;

class Program
{
    static void Main(string[] args)
    {
        UserManager manager = new UserManager();

        manager.AddUser(new User("Алия", "aliya@mail.com", "Admin"));
        manager.AddUser(new User("Дана", "dana@mail.com", "User"));

        Console.WriteLine("Пользователи:");
        manager.PrintUsers();

        manager.UpdateUser("dana@mail.com", "Дана Серикова", "Admin");

        Console.WriteLine("\nПосле обновления:");
        manager.PrintUsers();

        manager.RemoveUser("aliya@mail.com");

        Console.WriteLine("\nПосле удаления:");
        manager.PrintUsers();
    }
}