using LibraryKata.Domain;
using Serilog;

namespace LibraryKata.App; //Logical container for related code files

public class Program
{
    //main method
    public static void Main()
    {
        //Configure Serilog (singleton)
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information() // Verbose > Debug > Info > Warning > Error > Fatal
        .WriteTo.Console() //Sink: where do my logs go?
        .CreateLogger(); // create logger based on config adove  

        DataTypesAndOperatos();
        ControlFlow();
        Loops();
        ArraysWork();
        ClassesExample();
        OopDemo();
        CollectionsDemo();
        ExceptionsDemo();

        Log.CloseAndFlush();
    }

    private static void DataTypesAndOperatos()
    {
        Console.WriteLine("== Data Types and Operators ==");

        int copies = 3;
        double lateFee = 3.25;
        bool isMember = true;
        char shelf = 'A';
        string title = "Clean code";

        //Operators
        string user = "Jon";
        int total = copies * 2; //arithmetic operator
        bool isEnough = total > 4; //comparison operator

        bool exactlySix = total == 6; //equality operator

        bool lendable = isMember && isEnough; //logical operator

        Console.WriteLine(title + " has been checked by " + user);


        //String interpolation
        Console.WriteLine($"{title} on shelf: {shelf}: {copies} copies, fee: {lateFee}");

        total += 1; //assignment operator
    }

    private static void ControlFlow()
    {
        Console.WriteLine("\n== Control Flow ==");

        int copiesAvailable = 0;
        //bool isMember = true;

        // if - else if - else
        if (copiesAvailable > 1)
            Console.WriteLine("Many available for checkout!");
        else if (copiesAvailable == 1)
            Console.WriteLine("Last copy!");
        else
        {
            Console.WriteLine("Out of stock!");
            Console.WriteLine("Check again later!");
        }

        //Switch
        string genre = "Mystery";

        switch (genre)
        {
            case "Mystery":
                Console.WriteLine("Check Section A!");
                break;
            case "Science-Fiction":
                Console.WriteLine("Check Section F!");
                break;
            default:
                Console.WriteLine("Uh oh");
                break;
        }

        // New in .NET 8, Switch Expressions
        string section = genre switch
        {
            //Expression body
            "Mystery" => "Section A",
            "Science-Fiction" => "Section F",
            _ => "Uh oh" //default case
        };
        Console.WriteLine(section);

    }

    private static void Loops()
    {
        for (int day = 1; day <= 3; day++)
        {
            Console.WriteLine($"Reminder day {day}: fee so far {CalculateLateFee(day)}");
        }

        int onShelf = 3;
        while (onShelf > 0)
        {
            Console.WriteLine($"{onShelf} copies on the shelf!");
            onShelf--;
        }

        Console.WriteLine("No copies on shelf!");

        //Unmutable string
        /*string myString = "dog";

        myString = "cat";
        */
    }

    private static decimal CalculateLateFee(int daysLate) => daysLate * 2.05m;

    private static void ArraysWork()
    {
        string[] books = { "Dune", "Harry Potter", "Percy Jackson", "Lord of the Rings" };

        Console.WriteLine(books[2]);

        foreach (string book in books)
        {
            Console.WriteLine(book);
        }
    }

    private static void ClassesExample()
    {
        Console.WriteLine("Using our domain Book class");

        Book dune = new Book("Dune", "Frank Herbert", 3);
        Book littlePrince = new Book("The Little Prince", "Antoine de Saint-Exupéry", 0);

        //printing book info
        //This to lines do the same thing
        Console.WriteLine(dune);
        Console.WriteLine(littlePrince.ToString());

        Console.WriteLine($"Checking out Dune: {dune.Checkout()}");
        Console.WriteLine($"Checking out Little Prince: {littlePrince.Checkout()}");
    }

    public static void OopDemo()
    {

        Console.WriteLine("\n\n == OOP Demo stuff == ");

        // Leveraging polymorphism - Books, ReferenceBooks, Magazines - all are LibraryItems.
        LibraryItem[] catalog =
        {
            new Book("Dune", "Frank Herbert", 2),
            new ReferenceBook("C# Language Standards", "Microsoft", "Technology"),
            new Magazine("Sports Illustrated", "Francisco", 5, "Conde Naste")
        };

        foreach (LibraryItem item in catalog)
        {
            Console.WriteLine(item.Describe());
        }

        // We can even use interfaces as reference types
        foreach (LibraryItem item in catalog)
        {
            if (item is ILendable lendable)
            {
                Console.WriteLine($"{item.Title}: checkout -> {lendable.Checkout()}");
            }
            else
            {
                Console.WriteLine($"{item.Title} is Reference only.");
            }
        }

        // override vs new behavior
        Magazine wired = new Magazine("Wired", "Luis", 3, "Conde Nast");
        LibraryItem baseMag = wired;

        Console.WriteLine("== Override vs new on the same object, different ref type");
        Console.WriteLine($"Magazine reference -> {wired.ShelfLabel()}");
        Console.WriteLine($"LibraryItem reference -> {baseMag.ShelfLabel()}");

    }

    private static void CollectionsDemo()
    {
        Console.WriteLine("==== Collections Demo Stuff ====");

        Catalog catalog = new();

        Book dune = new Book("Dune", "Frank Herbert", 3);

        catalog._items.Add(dune);

        catalog._items.Add(new ReferenceBook("C# Language Specs", "Microsoft", "Technology"));
        catalog._items.Add(new Magazine("Nat Geo", "Charlie", 4, "Conde Naste"));

        Console.WriteLine($"Catalog holds {catalog._items.Count}; first is {catalog._items[0].Title}");

        // The other containers, each reached through intent-named methods instead of raw fields:
        // STACK (LIFO) - return cart: the last book dropped is the first re-shelved.
        catalog.DropInReturnCart(catalog[0]);
        catalog.DropInReturnCart(catalog[2]);
        Console.WriteLine($"Return cart has {catalog.CartCount}; reshelving \"{catalog.Reshelve().Title}\" first");

        // QUEUE (FIFO) - holds line: the first member to ask is the first served.
        catalog.PlaceHold("Ada");
        catalog.PlaceHold("Grace");
        Console.WriteLine($"{catalog.HoldsWaiting} holds waiting; serving {catalog.ServeNextHold()} first");

        // LINKEDLIST - a reading list we reorder; AddNextUp jumps to the front.
        catalog.AddToReadingList(catalog[0]);
        catalog.AddNextUp(catalog[1]);
        Console.WriteLine("Reading list order:");
        foreach (LibraryItem item in catalog.ReadingList)
        {
            Console.WriteLine($"  - {item.Title}");
        }

        ItemKind kind = ItemKind.Magazine;

        ShelfLocation whereIs = new ShelfLocation(3, 12);

        Console.WriteLine($"{kind} sits at {whereIs}");

        //Book duneCopy = dune; //copies the reference

        //ShelfLocation location = whereIs; //copies the data/fields

        //Generics
        Shelf<LibraryItem> shelf = new Shelf<LibraryItem>(2);
        Shelf<int> intShelf = new Shelf<int>(200);

        shelf.TryAdd(catalog._items[0]);
        shelf.TryAdd(catalog._items[1]);

        Console.WriteLine($"Trying to add a third item in our shelf: {shelf.TryAdd(catalog._items[2])}");

    }

    public static void ExceptionsDemo()
    {
        Console.WriteLine("\n == Exceptions, patterns, logging ==");

        ILibraryRepository repo = new InMemoryLibraryRepository();

        IUnitOfWork libraryWork = new LibraryUnitOfWork(repo);

        LibraryItem dune = LibraryItemFactory.Create(ItemKind.Book, "Dune", "Frank Herber", copies: 3);

        repo.Add(dune);

        repo.Add(LibraryItemFactory.Create(ItemKind.Magazine, "Wired", "Axel", copies: 2));

        //Pretend commiting changes
        libraryWork.Stage("added 2 items");
        libraryWork.Commit();

        try
        {
            LibraryItem missing = repo.GetById(345);
            Console.WriteLine(missing.Describe());
        }
        catch (ItemNotFoundException ex)
        {
            Log.Error("Lookup failed for id {Id}: {Message}", ex.Id, ex.Message);
        }
        catch (LibraryException ex)
        {
            Log.Error("Library error: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error("Non Library error: {Message}", ex.Message);
        }
        finally
        {
            Console.WriteLine("Hit out finally block - lookup attempt done");
        }

        Book noCopies = new Book("Count of Montecristo", "Alejandro Dumas", 0);

        try
        {
            Borrow(noCopies);
        }
        catch (ItemNotAvailableException ex)
        {
            Log.Warning("Borrow refused: {Message}", ex.Message);
        }
    }

    public static void Borrow(Book book)
    {
        if (!book.Checkout())
        {
            throw new ItemNotAvailableException(book.Title);
        }
    }

}