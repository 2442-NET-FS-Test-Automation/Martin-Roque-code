using Serilog;

namespace LibraryKata.Domain;

public class InMemoryLibraryRepository : ILibraryRepository
{
    private readonly Dictionary<int, LibraryItem> _items = new();

    public void Add(LibraryItem item)
    {
        //_items.Add(item);
        //New dictionary add code

        _items.Add(item.Id, item);
        //_items[item.Id] = item; -alternative dictionary adding syntax

        Log.Information("Added {Title} - (id : {id})", item.Title, item.Id);
    }

    public List<LibraryItem> GetAll()
    {
        //return _items.ToList();

        // Avoiding refactoring
        return _items.Values.ToList();
    }

    public LibraryItem GetById(int id)
    {
        /*foreach (LibraryItem item in _items)
        {
            if (item.Id == id)
            {
                return item;
            }
        }*/

        if (_items.TryGetValue(id, out LibraryItem? item)) // using out parameter for second return
        {
            return item;
        }

        Log.Warning("Lookup failed for id {id}", id);
        throw new ItemNotFoundException(id);
    }

    public bool Remove(int id)
    {
        // foreach (LibraryItem item in _items)
        // {
        //     if (item.Id == id)
        //     {
        //         _items.Remove(item);
        //         Log.Information("Removed item with id {Id}", id);
        //         return true;
        //     }

        //     Log.Information("Failed to remove item with id {id} - not found", id);
        //     return true;
        // }

        if (_items.Remove(id))
        {
            Log.Information("Removed item with id {Id}", id);
            return true;
        }

        Log.Information("Removal failed for the item with id {id}", id);
        return false;
    }
}