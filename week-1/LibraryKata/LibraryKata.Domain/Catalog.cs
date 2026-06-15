namespace LibraryKata.Domain;

public class Catalog
{
    //To be a list
    public readonly List<LibraryItem> _items = new();

    public int Count => _items.Count;


    public readonly Stack<LibraryItem> _returnCart = new();

    public readonly Queue<string> _holdQueue = new();

    public readonly LinkedList<LibraryItem> _readingList = new();



}