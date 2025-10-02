public enum CustomerType
{
    Regular,
    Silver,
    Gold
}

public interface IDiscountStrategy
{
    double CalculateDiscount(double amount);
}

public class RegularCustomerDiscount : IDiscountStrategy
{
    public double CalculateDiscount(double amount)
    {
        return amount; // 0% скидка
    }
}

public class SilverCustomerDiscount : IDiscountStrategy
{
    public double CalculateDiscount(double amount)
    {
        return amount * 0.9; // 10% скидка
    }
}

public class GoldCustomerDiscount : IDiscountStrategy
{
    public double CalculateDiscount(double amount)
    {
        return amount * 0.8; // 20% скидка
    }
}

public class DiscountCalculator
{
    private readonly Dictionary<CustomerType, IDiscountStrategy> _discountStrategies;

    public DiscountCalculator()
    {
        _discountStrategies = new Dictionary<CustomerType, IDiscountStrategy>
        {
            [CustomerType.Regular] = new RegularCustomerDiscount(),
            [CustomerType.Silver] = new SilverCustomerDiscount(),
            [CustomerType.Gold] = new GoldCustomerDiscount()
        };
    }

    public void AddDiscountStrategy(CustomerType customerType, IDiscountStrategy strategy)
    {
        _discountStrategies[customerType] = strategy;
    }

    public double CalculateDiscount(CustomerType customerType, double amount)
    {
        if (_discountStrategies.TryGetValue(customerType, out var strategy))
        {
            return strategy.CalculateDiscount(amount);
        }
        throw new ArgumentException("Неизвестный тип клиента");
    }
}

// ДОБАВЛЕНИЕ НОВОГО ТИПА КЛИЕНТА БЕЗ ИЗМЕНЕНИЯ КОДА:
public class PlatinumCustomerDiscount : IDiscountStrategy
{
    public double CalculateDiscount(double amount)
    {
        return amount * 0.7; // 30% скидка
    }
}