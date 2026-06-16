using Serilog;

namespace LibraryKata.Domain;

public class InMemoryLibraryRepository : ILibraryRepository
{
    private readonly List<LibraryItem> _items = new();

    public void Add(LibraryItem item)
    {
        _items.Add(item);
        Log.Information("Added {Title} - (id : {id})", item.Title, item.Id);
    }

    public List<LibraryItem> GetAll()
    {
        return _items.ToList();
    }

    public LibraryItem GetById(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        Log.Warning("Lookup failed for id {id}", id);
        throw new ItemNotFoundException(id);
    }

    public bool Remove(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if (item.Id == id)
            {
                _items.Remove(item);
                Log.Information("Removed item with id");
                return true;
            }

            Log.Information("Failed to remove item with id {id} - not found", id);
            return true;
        }
        Log.Information("Removal faildes for id {id}", id);
        return false;
    }
}