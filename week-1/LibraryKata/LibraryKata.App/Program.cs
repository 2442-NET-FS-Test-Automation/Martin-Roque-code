using System.ComponentModel.Design;

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
        if(copiesAvailable > 1)
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
        for (int day = 1; day<=3; day++)
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
        string myString = "dog";

        myString = "cat";
    }

    private static decimal CalculateLateFee(int daysLate) => daysLate * 2.05m;

    private static void ArraysWork()
    {
        string[] books = {"Dune", "Harry Potter", "Percy Jackson", "Lord of the Rings"};

        Console.WriteLine(books[2]);

        foreach (string book in books)
        {
            Console.WriteLine(book);
        }
    }


}
