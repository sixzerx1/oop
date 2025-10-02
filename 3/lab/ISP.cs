public interface IWorkable
{
    void Work();
}

public interface IEatable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}

public class HumanWorker : IWorkable, IEatable, ISleepable
{
    public void Work()
    {
        Console.WriteLine("Человек работает");
    }

    public void Eat()
    {
        Console.WriteLine("Человек ест");
    }

    public void Sleep()
    {
        Console.WriteLine("Человек спит");
    }
}

public class RobotWorker : IWorkable
{
    public void Work()
    {
        Console.WriteLine("Робот работает");
    }
}