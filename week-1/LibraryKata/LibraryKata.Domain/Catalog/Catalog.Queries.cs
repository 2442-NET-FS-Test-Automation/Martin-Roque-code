using System.Collections;

namespace LibraryKata.Domain;

public partial class Catalog : IEnumerable<LibraryItem>
{
    public IEnumerator<LibraryItem> GetEnumerator()
    {
        foreach (LibraryItem item in _items)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    //return only lendable items

    public IEnumerable<LibraryItem> Lendable()
    {
        foreach (LibraryItem item in _items)
        {
            if (item is ILendable)
            {
                yield return item;
            }
        }
    }

    //Search function
    public List<LibraryItem> Find(Predicate<LibraryItem> match)
    {
        //match is a pointer to some method that get passed in when we call Find()
        List<LibraryItem> foundItems = new();

        foreach (LibraryItem item in _items)
        {
            if (match(item))
            {
                foundItems.Add(item);
            }
        }

        return foundItems;
    }
}