using System;

public interface IDocument
{
    void Open();
}

public class Report : IDocument
{
    public void Open() => Console.WriteLine("Report opened");
}

public class Resume : IDocument
{
    public void Open() => Console.WriteLine("Resume opened");
}

public class Letter : IDocument
{
    public void Open() => Console.WriteLine("Letter opened");
}

public class Invoice : IDocument
{
    public void Open() => Console.WriteLine("Invoice opened");
}

public abstract class DocumentCreator
{
    public abstract IDocument CreateDocument();
}

public class ReportCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new Report();
}

public class ResumeCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new Resume();
}

public class LetterCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new Letter();
}

public class InvoiceCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new Invoice();
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Введите тип документа (report, resume, letter, invoice):");
        string choice = Console.ReadLine();

        DocumentCreator creator = choice switch
        {
            "report"  => new ReportCreator(),
            "resume"  => new ResumeCreator(),
            "letter"  => new LetterCreator(),
            "invoice" => new InvoiceCreator(),
            _         => null
        };

        if (creator == null)
        {
            Console.WriteLine("Неизвестный тип документа.");
        }
        else
        {
            IDocument doc = creator.CreateDocument();
            doc.Open();
        }
    }
}