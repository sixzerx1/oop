using System;
using System.Collections.Generic;

abstract class Component
{
    protected string name;

    public Component(string name)
    {
        this.name = name;
    }

    public abstract void Show(int indent);
}

class File : Component
{
    public File(string name) : base(name) { }

    public override void Show(int indent)
    {
        Console.WriteLine(new string('-', indent) + name);
    }
}

class Folder : Component
{
    private List<Component> items = new List<Component>();

    public Folder(string name) : base(name) { }

    public void Add(Component c)
    {
        items.Add(c);
    }

    public override void Show(int indent)
    {
        Console.WriteLine(new string('-', indent) + name);
        foreach (var i in items)
            i.Show(indent + 2);
    }
}

class Program
{
    static void Main()
    {
        Folder root = new Folder("Root");
        root.Add(new File("Newfile.txt"));
        root.Add(new File("Newfile2.txt"));

        Folder sub = new Folder("SubFolder");
        sub.Add(new File("Subfile.txt"));
        root.Add(sub);

        root.Show(1);
    }
}
