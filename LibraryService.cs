using System.Collections.Generic;
using System.Linq;

namespace Library;

public class LibraryService
{
    public List<Book> Books { get; private set; } = new();
    public List<User> Users { get; private set; } = new();
    public Dictionary<User, List<Book>> BorrowedBooks { get; private set; } = new();

    public void AddBook(string title, string author, string isbn)
    {
        int id = Books.Any() ? Books.Max(b => b.Id) + 1 : 1;
        Books.Add(new Book { Id = id, Title = title, Author = author, ISBN = isbn });
    }

    public void EditBook(int id, string title, string author, string isbn)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);
        if (book == null) return;
        if (!string.IsNullOrEmpty(title)) book.Title = title;
        if (!string.IsNullOrEmpty(author)) book.Author = author;
        if (!string.IsNullOrEmpty(isbn)) book.ISBN = isbn;
    }

    public void DeleteBook(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);
        if (book != null) Books.Remove(book);
    }

    public void AddUser(string name, string email)
    {
        int id = Users.Any() ? Users.Max(u => u.Id) + 1 : 1;
        Users.Add(new User { Id = id, Name = name, Email = email });
    }

    public void EditUser(int id, string name, string email)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return;
        if (!string.IsNullOrEmpty(name)) user.Name = name;
        if (!string.IsNullOrEmpty(email)) user.Email = email;
    }

    public void DeleteUser(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user != null) { BorrowedBooks.Remove(user); Users.Remove(user); }
    }

    public bool BorrowBook(int bookId, int userId)
    {
        var book = Books.FirstOrDefault(b => b.Id == bookId);
        var user = Users.FirstOrDefault(u => u.Id == userId);
        if (book == null || user == null) return false;

        if (!BorrowedBooks.ContainsKey(user))
            BorrowedBooks[user] = new List<Book>();

        BorrowedBooks[user].Add(book);
        Books.Remove(book);
        return true;
    }

    public bool ReturnBook(int userId, int bookIndex)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId);
        if (user == null || !BorrowedBooks.ContainsKey(user)) return false;

        var borrowed = BorrowedBooks[user];
        if (bookIndex < 0 || bookIndex >= borrowed.Count) return false;

        var book = borrowed[bookIndex];
        borrowed.RemoveAt(bookIndex);
        Books.Add(book);
        return true;
    }

    // Default constructor — skips CSV loading, used by unit tests
    public LibraryService() { }

    // Constructor used by Blazor app — loads CSV data on startup
    public LibraryService(bool loadData)
    {
        if (!loadData) return;

        try
        {
            foreach (var line in File.ReadLines("Data/Books.csv"))
            {
                var fields = line.Split(',');
                if (fields.Length >= 4)
                {
                    Books.Add(new Book
                    {
                        Id = int.Parse(fields[0].Trim()),
                        Title = fields[1].Trim(),
                        Author = fields[2].Trim(),
                        ISBN = fields[3].Trim()
                    });
                }
            }

            foreach (var line in File.ReadLines("Data/Users.csv"))
            {
                var fields = line.Split(',');
                if (fields.Length >= 3)
                {
                    Users.Add(new User
                    {
                        Id = int.Parse(fields[0].Trim()),
                        Name = fields[1].Trim(),
                        Email = fields[2].Trim()
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not load seed data: {ex.Message}");
        }
    }

}





