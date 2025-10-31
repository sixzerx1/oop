using System;

public interface IBeverage
{
    double Cost();
    string GetDescription();
}

public abstract class Beverage : IBeverage
{
    public abstract string GetDescription();
    public abstract double Cost();
}
public class Espresso : Beverage
{
    public override string GetDescription() => "Эспрессо";
    public override double Cost() => 300;
}

public class Tea : Beverage
{
    public override string GetDescription() => "Чай";
    public override double Cost() => 250;
}

public class Latte : Beverage
{
    public override string GetDescription() => "Латте";
    public override double Cost() => 400;
}

public class Mocha : Beverage
{
    public override string GetDescription() => "Мокко";
    public override double Cost() => 450;
}

public abstract class BeverageDecorator : Beverage
{
    protected Beverage _beverage;

    public BeverageDecorator(Beverage beverage)
    {
        _beverage = beverage;
    }

    public override string GetDescription()
    {
        return _beverage.GetDescription();
    }
    public override double Cost()
    {
        return _beverage.Cost();
    }
}

public class Milk : BeverageDecorator
{
    public Milk(Beverage beverage) : base(beverage) { }

    public override string GetDescription()
    {
        return _beverage.GetDescription() + ", с молоком";
    }

    public override double Cost()
    {
        return _beverage.Cost() + 50;
    }
}

public class Sugar : BeverageDecorator
{
    public Sugar(Beverage beverage) : base(beverage) { }

    public override string GetDescription()
    {
        return _beverage.GetDescription() + ", с сахаром";
    }

    public override double Cost()
    {
        return _beverage.Cost() + 20;
    }
}

public class WhippedCream : BeverageDecorator
{
    public WhippedCream(Beverage beverage) : base(beverage) { }

    public override string GetDescription()
    {
        return _beverage.GetDescription() + ", с взбитыми сливками";
    }

    public override double Cost()
    {
        return _beverage.Cost() + 70;
    }
}

public class Caramel : BeverageDecorator
{
    public Caramel(Beverage beverage) : base(beverage) { }

    public override string GetDescription()
    {
        return _beverage.GetDescription() + ", с карамелью";
    }

    public override double Cost()
    {
        return _beverage.Cost() + 80;
    }
}



internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("КАФЕ: Система заказов\n");

        Beverage order1 = new Sugar(new Milk(new Espresso()));
        Console.WriteLine($"{order1.GetDescription()} — {order1.Cost()} тг");

        Beverage order2 = new WhippedCream(new Caramel(new Latte()));
        Console.WriteLine($"{order2.GetDescription()} — {order2.Cost()} тг");

        Beverage order3 = new Sugar(new Tea());
        Console.WriteLine($"{order3.GetDescription()} — {order3.Cost()} тг");

    }
}


