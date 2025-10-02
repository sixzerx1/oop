using System;
using System.Collections.Generic;

namespace LibraryApp
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int CopiesAvailable { get; set; }

        public Book(string title, string author, string isbn, int copies)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            CopiesAvailable = copies;
        }

        public override string ToString()
        {
            return $"{Title} ({Author}), ISBN: {ISBN}, Доступно: {CopiesAvailable}";
        }
    }

    // Класс Читатель
    public class Reader
    {
        public string Name { get; set; }
        public int ReaderId { get; set; }
        public List<Book> BorrowedBooks { get; private set; }

        public Reader(string name, int id)
        {
            Name = name;
            ReaderId = id;
            BorrowedBooks = new List<Book>();
        }

        public override string ToString()
        {
            return $"Читатель: {Name}, ID: {ReaderId}, Взято книг: {BorrowedBooks.Count}";
        }
    }

    public class Library
    {
        private List<Book> books = new List<Book>();
        private List<Reader> readers = new List<Reader>();

        public void AddBook(Book book)
        {
            books.Add(book);
            Console.WriteLine($"Книга добавлена: {book.Title}");
        }

        public void RemoveBook(string isbn)
        {
            Book book = books.Find(b => b.ISBN == isbn);
            if (book != null)
            {
                books.Remove(book);
                Console.WriteLine($"Книга удалена: {book.Title}");
            }
            else
            {
                Console.WriteLine("Книга не найдена!");
            }
        }

        public void RegisterReader(Reader reader)
        {
            readers.Add(reader);
            Console.WriteLine($"Зарегистрирован читатель: {reader.Name}");
        }

        public void RemoveReader(int readerId)
        {
            Reader reader = readers.Find(r => r.ReaderId == readerId);
            if (reader != null)
            {
                readers.Remove(reader);
                Console.WriteLine($"Читатель удален: {reader.Name}");
            }
            else
            {
                Console.WriteLine("Читатель не найден!");
            }
        }

        public void BorrowBook(int readerId, string isbn)
        {
            Reader reader = readers.Find(r => r.ReaderId == readerId);
            Book book = books.Find(b => b.ISBN == isbn);

            if (reader == null)
            {
                Console.WriteLine("Читатель не найден!");
                return;
            }

            if (book == null)
            {
                Console.WriteLine("Книга не найдена!");
                return;
            }

            if (book.CopiesAvailable > 0)
            {
                book.CopiesAvailable--;
                reader.BorrowedBooks.Add(book);
                Console.WriteLine($"Книга '{book.Title}' выдана читателю {reader.Name}");
            }
            else
            {
                Console.WriteLine($"Книга '{book.Title}' недоступна!");
            }
        }

        public void ReturnBook(int readerId, string isbn)
        {
            Reader reader = readers.Find(r => r.ReaderId == readerId);
            if (reader == null)
            {
                Console.WriteLine("Читатель не найден!");
                return;
            }

            Book book = reader.BorrowedBooks.Find(b => b.ISBN == isbn);
            if (book != null)
            {
                reader.BorrowedBooks.Remove(book);
                book.CopiesAvailable++;
                Console.WriteLine($"Читатель {reader.Name} вернул книгу '{book.Title}'");
            }
            else
            {
                Console.WriteLine("Эта книга не числится за читателем!");
            }
        }

        public void ShowAllBooks()
        {
            Console.WriteLine("\nСписок книг в библиотеке:");
            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
        }

        public void ShowAllReaders()
        {
            Console.WriteLine("\nСписок читателей:");
            foreach (var reader in readers)
            {
                Console.WriteLine(reader);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();

            library.AddBook(new Book("Гарри Поттер", "Джоан Роулинг", "123", 3));
            library.AddBook(new Book("Властелин колец", "Р. Толкин", "150", 2));

            Reader r1 = new Reader("Досайбек Мерей", 1);
            Reader r2 = new Reader("Нурабаева Алтынай", 2);
            library.RegisterReader(r1);
            library.RegisterReader(r2);

            library.BorrowBook(1, "123");
            library.BorrowBook(2, "150");

            library.BorrowBook(1, "150");
            library.BorrowBook(2, "150"); 

            library.ReturnBook(1, "123");

            library.ShowAllBooks();
            library.ShowAllReaders();

            Console.ReadLine();
        }
    }
}