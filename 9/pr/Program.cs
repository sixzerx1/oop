using System;
using System.Collections.Generic;

class RoomBookingSystem
{
    public bool IsRoomAvailable(int roomNumber, DateTime date)
    {
        Console.WriteLine($"Checking if room {roomNumber} is available on {date.ToShortDateString()}...");
        return true;
    }

    public void BookRoom(int roomNumber, string customerName, DateTime date)
    {
        Console.WriteLine($"Room {roomNumber} booked for {customerName} on {date.ToShortDateString()}");
    }
}

class RestaurantSystem
{
    public void ReserveTable(int tableNumber, string[] dishes, DateTime date)
    {
        Console.WriteLine($"Table {tableNumber} reserved on {date.ToShortDateString()}");
        Console.WriteLine("Dishes: " + string.Join(", ", dishes));
    }
}

class HotelFacade
{
    private RoomBookingSystem roomSystem;
    private RestaurantSystem restaurantSystem;

    public HotelFacade(RoomBookingSystem room, RestaurantSystem restaurant)
    {
        roomSystem = room;
        restaurantSystem = restaurant;
    }

    public void BookFullService(int roomNumber, string customerName, DateTime date, int tableNumber, string[] dishes)
    {
        if (roomSystem.IsRoomAvailable(roomNumber, date))
        {
            roomSystem.BookRoom(roomNumber, customerName, date);
            restaurantSystem.ReserveTable(tableNumber, dishes, date);
            Console.WriteLine("Full service booked successfully!");
        }
        else
        {
            Console.WriteLine("Not available.");
        }
    }
}



abstract class OrgComponent
{
    protected string name;
    public OrgComponent(string name) { this.name = name; }
    public abstract void Show(int indent);
}

class Employee : OrgComponent
{
    public Employee(string name) : base(name) { }
    public override void Show(int indent)
    {
        Console.WriteLine(new string('-', indent) + name);
    }
}

class Department : OrgComponent
{
    private List<OrgComponent> members = new List<OrgComponent>();
    public Department(string name) : base(name) { }
    public void Add(OrgComponent c) { members.Add(c); }
    public override void Show(int indent)
    {
        Console.WriteLine(new string('-', indent) + name);
        foreach (var m in members) m.Show(indent + 2);
    }
}

class Program
{
    static void Main()
    {
        
        var hotel = new HotelFacade(new RoomBookingSystem(), new RestaurantSystem());
        hotel.BookFullService(101, "Emir", DateTime.Now, 5, new string[] { "Burger", "Tomato Juice" });

        Console.WriteLine("\n--- Organization Structure ---");

        
        var head = new Department("Head Office");
        head.Add(new Employee("Accountant"));
        head.Add(new Employee("Manager"));

        var branch = new Department("Branch Office");
        branch.Add(new Employee("Cleaner"));
        branch.Add(new Employee("Seller"));

        head.Add(branch);
        head.Show(1);
    }
}
