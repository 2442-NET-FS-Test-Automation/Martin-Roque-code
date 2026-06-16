namespace LibraryKata.Domain;

public interface ILibraryRepository
{
    void Add(LibraryItem item);

    LibraryItem GetById(int id);

    List<LibraryItem> GetAll();

    bool Remove(int id);
}