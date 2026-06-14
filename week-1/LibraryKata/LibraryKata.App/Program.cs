using LibraryKata.Domain;

namespace LibraryKata.App; //Logical container for related code files

public class Program
{
    //main method
    public static void Main()
    {
        DataTypesAndOperatos();
        ControlFlow();
        Loops();
        ArraysWork();
        ClassesExample();
        OopDemo();
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


}
