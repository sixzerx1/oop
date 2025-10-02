using System;
using System.Collections.Generic;

namespace EmployeeApp
{
    
    abstract class Employee
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Position { get; set; }

        public Employee(string name, int id, string position)
        {
            Name = name;
            Id = id;
            Position = position;
        }

       
        public abstract decimal CalculateSalary();

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}, Имя: {Name}, Должность: {Position}, Зарплата: {CalculateSalary()}");
        }
    }

  
    class Worker : Employee
    {
        public decimal HourlyRate { get; set; }
        public int HoursWorked { get; set; }

        public Worker(string name, int id, string position, decimal hourlyRate, int hoursWorked)
            : base(name, id, position)
        {
            HourlyRate = hourlyRate;
            HoursWorked = hoursWorked;
        }

        public override decimal CalculateSalary()
        {
            return HourlyRate * HoursWorked;
        }
    }

    
    class Manager : Employee
    {
        public decimal FixedSalary { get; set; }
        public decimal Bonus { get; set; }

        public Manager(string name, int id, string position, decimal fixedSalary, decimal bonus)
            : base(name, id, position)
        {
            FixedSalary = fixedSalary;
            Bonus = bonus;
        }

        public override decimal CalculateSalary()
        {
            return FixedSalary + Bonus;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Employee> employees = new List<Employee>();

            
            employees.Add(new Worker("Мерей", 1, "Рабочий", 500, 160));   
            employees.Add(new Worker("Алтынай", 2, "Рабочий", 600, 170));
            employees.Add(new Manager("Али", 3, "Менеджер", 50000, 10000));
            employees.Add(new Manager("Амир", 4, "Менеджер", 60000, 15000));

            
            Console.WriteLine("Список сотрудников и их зарплаты:\n");
            foreach (var emp in employees)
            {
                emp.DisplayInfo();
            }
        }
    }
}