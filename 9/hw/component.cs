using System;
using System.Collections.Generic;
using System.Linq;

public abstract class FileSystemComponent
{
    protected string _name;
    protected string _owner;

    public FileSystemComponent(string name, string owner)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя компонента не может быть пустым");
        
        _name = name.Trim();
        _owner = owner ?? "Система";
    }

    public string Name => _name;
    public string Owner => _owner;

    public abstract void Display(int depth);
    public abstract long GetSize();
    public abstract int GetComponentCount();

    public virtual void Add(FileSystemComponent component)
    {
        throw new InvalidOperationException("Невозможно добавить компонент к этому элементу");
    }

    public virtual void Remove(FileSystemComponent component)
    {
        throw new InvalidOperationException("Невозможно удалить компонент из этого элемента");
    }

    public virtual FileSystemComponent GetChild(int index)
    {
        throw new InvalidOperationException("Этот элемент не содержит дочерних компонентов");
    }

    public virtual bool Contains(FileSystemComponent component)
    {
        return false;
    }

    public virtual List<FileSystemComponent> Search(string name)
    {
        return new List<FileSystemComponent>();
    }
}

public class File : FileSystemComponent
{
    private long _size;
    private string _extension;

    public File(string name, string owner, long size, string extension) 
        : base(name, owner)
    {
        if (size < 0)
            throw new ArgumentException("Размер файла не может быть отрицательным");
        
        _size = size;
        _extension = extension ?? "";
    }

    public override void Display(int depth)
    {
        string indent = new string(' ', depth);
        Console.WriteLine($"{indent}📄 {_name}.{_extension}");
        Console.WriteLine($"{indent}   Владелец: {_owner}");
        Console.WriteLine($"{indent}   Размер: {FormatSize(_size)}");
    }

    public override long GetSize()
    {
        return _size;
    }

    public override int GetComponentCount()
    {
        return 1;
    }

    private string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}

public class Directory : FileSystemComponent
{
    private List<FileSystemComponent> _children;
    private DateTime _creationDate;

    public Directory(string name, string owner) : base(name, owner)
    {
        _children = new List<FileSystemComponent>();
        _creationDate = DateTime.Now;
    }

    public override void Add(FileSystemComponent component)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        if (_children.Any(c => c.Name == component.Name && c.GetType() == component.GetType()))
        {
            throw new InvalidOperationException($"Компонент с именем '{component.Name}' уже существует в папке '{_name}'");
        }

        _children.Add(component);
    }

    public override void Remove(FileSystemComponent component)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        if (!_children.Contains(component))
        {
            throw new InvalidOperationException($"Компонент '{component.Name}' не найден в папке '{_name}'");
        }

        _children.Remove(component);
    }

    public override FileSystemComponent GetChild(int index)
    {
        if (index < 0 || index >= _children.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return _children[index];
    }

    public override bool Contains(FileSystemComponent component)
    {
        return _children.Contains(component);
    }

    public override List<FileSystemComponent> Search(string name)
    {
        var results = new List<FileSystemComponent>();
        
        foreach (var component in _children)
        {
            if (component.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(component);
            }
            
            if (component is Directory directory)
            {
                results.AddRange(directory.Search(name));
            }
        }
        
        return results;
    }

    public override void Display(int depth)
    {
        string indent = new string(' ', depth);
        Console.WriteLine($"{indent}📁 {_name}/");
        Console.WriteLine($"{indent}   Владелец: {_owner}");
        Console.WriteLine($"{indent}   Создана: {_creationDate:dd.MM.yyyy HH:mm}");
        Console.WriteLine($"{indent}   Размер: {FormatSize(GetSize())}");
        Console.WriteLine($"{indent}   Элементов: {GetComponentCount()}");

        if (_children.Count > 0)
        {
            Console.WriteLine($"{indent}   Содержимое:");
            foreach (var component in _children)
            {
                component.Display(depth + 4);
            }
        }
        else
        {
            Console.WriteLine($"{indent}   [Папка пуста]");
        }
    }

    public override long GetSize()
    {
        return _children.Sum(child => child.GetSize());
    }

    public override int GetComponentCount()
    {
        return _children.Sum(child => child.GetComponentCount());
    }

    public int GetDirectChildCount()
    {
        return _children.Count;
    }

    public List<File> GetAllFiles()
    {
        var files = new List<File>();
        
        foreach (var component in _children)
        {
            if (component is File file)
            {
                files.Add(file);
            }
            else if (component is Directory directory)
            {
                files.AddRange(directory.GetAllFiles());
            }
        }
        
        return files;
    }

    public List<Directory> GetAllDirectories()
    {
        var directories = new List<Directory>();
        
        foreach (var component in _children)
        {
            if (component is Directory directory)
            {
                directories.Add(directory);
                directories.AddRange(directory.GetAllDirectories());
            }
        }
        
        return directories;
    }

    private string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== СИСТЕМА УПРАВЛЕНИЯ ФАЙЛАМИ И ПАПКАМИ С ПАТТЕРНОМ КОМПОНОВЩИК ===\n");

        try
        {
            Directory root = new Directory("Документы", "Молдахулов Эмир");

            File report1 = new File("Отчет_январь", "Молдахулов Эмир", 1024 * 150, "pdf");
            File report2 = new File("Отчет_февраль", "Молдахулов Эмир", 1024 * 180, "pdf");
            File presentation = new File("Презентация_проекта", "Молдахулов Эмир", 1024 * 1024 * 5, "pptx");
            File budget = new File("Бюджет_2024", "Молдахулов Эмир", 1024 * 250, "xlsx");

            Directory workDir = new Directory("Работа", "Молдахулов Эмир");
            Directory personalDir = new Directory("Личное", "Молдахулов Эмир");
            Directory projectsDir = new Directory("Проекты", "Молдахулов Эмир");

            File project1 = new File("Проект_Альфа", "Кожабек Али", 1024 * 1024 * 10, "zip");
            File project2 = new File("Проект_Бета", "Байжан Амир", 1024 * 1024 * 8, "zip");
            File photo1 = new File("Фото_отпуск", "Молдахулов Эмир", 1024 * 1024 * 2, "jpg");
            File resume = new File("Резюме", "Молдахулов Эмир", 1024 * 300, "docx");

            Directory currentProjects = new Directory("Текущие_проекты", "Молдахулов Эмир");
            Directory archive = new Directory("Архив", "Изатов Диас");

            File currentProject = new File("Текущий_проект", "Казимир Казимирович", 1024 * 1024 * 15, "rar");
            File oldProject = new File("Старый_проект", "Дмитрий Снег", 1024 * 1024 * 3, "rar");

            root.Add(workDir);
            root.Add(personalDir);

            workDir.Add(report1);
            workDir.Add(report2);
            workDir.Add(presentation);
            workDir.Add(budget);
            workDir.Add(projectsDir);

            projectsDir.Add(project1);
            projectsDir.Add(project2);
            projectsDir.Add(currentProjects);
            projectsDir.Add(archive);

            currentProjects.Add(currentProject);
            archive.Add(oldProject);

            personalDir.Add(photo1);
            personalDir.Add(resume);

            Console.WriteLine("=== СТРУКТУРА ФАЙЛОВОЙ СИСТЕМЫ ===");
            root.Display(1);

            Console.WriteLine("\n" + new string('=', 80));

            Console.WriteLine("=== АНАЛИТИКА ФАЙЛОВОЙ СИСТЕМЫ ===");
            Console.WriteLine($"Общий размер: {FormatSize(root.GetSize())}");
            Console.WriteLine($"Общее количество элементов: {root.GetComponentCount()}");
            Console.WriteLine($"Количество файлов: {root.GetAllFiles().Count}");
            Console.WriteLine($"Количество папок: {root.GetAllDirectories().Count}");

            Console.WriteLine("\n" + new string('=', 80));

            Console.WriteLine("=== ПОИСК ФАЙЛОВ С 'проект' В НАЗВАНИИ ===");
            var searchResults = root.Search("проект");
            foreach (var result in searchResults)
            {
                Console.WriteLine($"- {result.Name} (владелец: {result.Owner})");
            }

            Console.WriteLine("\n" + new string('=', 80));

            Console.WriteLine("=== ФАЙЛЫ РАЗНЫХ ПОЛЬЗОВАТЕЛЕЙ ===");
            
            var allFiles = root.GetAllFiles();
            var users = allFiles.GroupBy(f => f.Owner);
            
            foreach (var userGroup in users)
            {
                Console.WriteLine($"\n{userGroup.Key}:");
                foreach (var file in userGroup)
                {
                    Console.WriteLine($"  - {file.Name} ({FormatSize(file.GetSize())})");
                }
            }

            Console.WriteLine("\n" + new string('=', 80));

            Console.WriteLine("=== ДЕМОНСТРАЦИЯ ОБРАБОТКИ ОШИБОК ===");
            
            try
            {
                workDir.Add(report1);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка при добавлении дубликата: {ex.Message}");
            }

            try
            {
                File nonExistent = new File("Несуществующий", "Дмитрий Довгешко", 100, "txt");
                workDir.Remove(nonExistent);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}
