using System.ComponentModel.Design;

namespace LibraryKata.App; //Logical container for related code files

public class Program
{
    //main method
    public static void Main()
    {
        DataTypesAndOperatos();
        ControlFlow();
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
        bool isMember = true;

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
            
    }
}
