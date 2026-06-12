namespace Library.Domain;

public class Book
{
    //Things about a book 
    public string? Title { get; private set; }
    public string? Author { get; private set; }
    public int? CopiesAvailable { get; private set; }

    private static int _nextId = 1;

    public int Id { get; }

    //Methods
    //Constructor - many as you need
    public Book(string title, string author, int copiesAvailable)
    {
        Title = title;
        Author = author;
        CopiesAvailable = copiesAvailable;
        Id = _nextId++;
    }

    public Book()
    {

    }

    public bool Checkout()
    {
        if (CopiesAvailable == 0)
            return false;

        CopiesAvailable--;
        return true;
    }

    public void Return() => CopiesAvailable++;

    //override
    public override string ToString()
    {
        //return base.ToString();
        return $"{Title} by {Author}: {CopiesAvailable} available for checkout";
    }
}