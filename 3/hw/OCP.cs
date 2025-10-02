public class Employee
{
    public string Name { get; set; }
    public double BaseSalary { get; set; }
}

// Базовый интерфейс для расчета зарплаты
public interface ISalaryCalculator
{
    double CalculateSalary(Employee employee);
}

// Конкретные реализации для каждого типа сотрудников
public class PermanentEmployeeCalculator : ISalaryCalculator
{
    public double CalculateSalary(Employee employee)
    {
        return employee.BaseSalary * 1.2; // 20% bonus
    }
}

public class ContractEmployeeCalculator : ISalaryCalculator
{
    public double CalculateSalary(Employee employee)
    {
        return employee.BaseSalary * 1.1; // 10% bonus
    }
}

public class InternEmployeeCalculator : ISalaryCalculator
{
    public double CalculateSalary(Employee employee)
    {
        return employee.BaseSalary * 0.8; // 80% of base salary
    }
}

// Калькулятор зарплат, который легко расширяется
public class EmployeeSalaryCalculator
{
    private readonly Dictionary<string, ISalaryCalculator> _calculators;

    public EmployeeSalaryCalculator()
    {
        _calculators = new Dictionary<string, ISalaryCalculator>
        {
            ["Permanent"] = new PermanentEmployeeCalculator(),
            ["Contract"] = new ContractEmployeeCalculator(),
            ["Intern"] = new InternEmployeeCalculator()
        };
    }

    public void AddCalculator(string employeeType, ISalaryCalculator calculator)
    {
        _calculators[employeeType] = calculator;
    }

    public double CalculateSalary(Employee employee, string employeeType)
    {
        if (_calculators.TryGetValue(employeeType, out var calculator))
        {
            return calculator.CalculateSalary(employee);
        }
        throw new NotSupportedException($"Employee type '{employeeType}' not supported");
    }
}

// ДОБАВЛЕНИЕ НОВОГО ТИПА БЕЗ ИЗМЕНЕНИЯ СУЩЕСТВУЮЩЕГО КОДА:
public class FreelancerCalculator : ISalaryCalculator
{
    public double CalculateSalary(Employee employee)
    {
        return employee.BaseSalary * 1.15; // 15% bonus
    }
}