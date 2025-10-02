public class Item
{
    public string Name { get; set; }
    public double Price { get; set; }
}

public class Invoice
{
    public int Id { get; set; }
    public List<Item> Items { get; set; }
    public double TaxRate { get; set; }
}

public class InvoiceCalculator
{
    public double CalculateTotal(Invoice invoice)
    {
        double subTotal = 0;
        foreach (var item in invoice.Items)
        {
            subTotal += item.Price;
        }
        return subTotal + (subTotal * invoice.TaxRate);
    }
}

public class InvoiceRepository
{
    public void SaveToDatabase(Invoice invoice)
    {
        // Логика для сохранения счета-фактуры в базу данных
        Console.WriteLine($"Invoice {invoice.Id} saved to database");
    }
}