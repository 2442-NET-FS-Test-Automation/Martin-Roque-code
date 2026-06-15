using CarWorkshop.Domain;

namespace CarWorkshop.App;

class Program
{
    static void Main()
    {
        List<Vehicle> inventory =
        [
            new Car("Toyota", "Camry", 12000, 4),
            new Motorcycle("Honda", "CBR500R", 4500, false)
        ];

        ShowMenu(inventory);
    }
    static void ListInventory(List<Vehicle> inventory)
    {
        foreach (Vehicle vehicle in inventory)
            Console.WriteLine(vehicle.GetSummary());
    }
    static void ServiceAllVehicles(List<Vehicle> inventory)
    {
        foreach (Vehicle vehicle in inventory)
            Console.WriteLine(vehicle.PerformService());
    }

    static Vehicle? GetVehicleById(List<Vehicle> inventory, int id)
    {
        foreach (Vehicle vehicle in inventory)
        {
            if (vehicle.Id == id)
            {
                return vehicle;
            }
        }

        return null;
    }

    static void ShowServiceMilesForVehicle(List<Vehicle> inventory)
    {
        ListInventory(inventory);
        Console.Write("Enter vehicle Id: ");
        int id = int.Parse(Console.ReadLine());

        inventory.Count();

        if (id > inventory.Count() + 1)
        {
            Console.WriteLine("Invalid Id.");
            return;
        }

        Vehicle? selectedVehicle = GetVehicleById(inventory, id);

        if (selectedVehicle is null)
        {
            Console.WriteLine("Vehicle not found.");
            return;
        }

        Console.WriteLine($"{selectedVehicle.Brand} {selectedVehicle.Model}: {selectedVehicle.MileageRemainingForService()}");
    }

    static void ShowWashEquipmentForVehicle(List<Vehicle> inventory)
    {
        ListInventory(inventory);
        Console.Write("Enter vehicle Id: ");
        int id = int.Parse(Console.ReadLine());

        inventory.Count();

        if (id > inventory.Count() + 1)
        {
            Console.WriteLine("Invalid Id.");
            return;
        }

        Vehicle? selectedVehicle = GetVehicleById(inventory, id);

        if (selectedVehicle is null)
        {
            Console.WriteLine("Vehicle not found.");
            return;
        }

        Console.WriteLine($"Wash instructions for {selectedVehicle.Brand} {selectedVehicle.Model}: {selectedVehicle.EquipmentToWashIt()}");
    }

    static string AddNewVehicle(List<Vehicle> inventory)
    {
        Console.Write("Add (1) Car or (2) Motorcycle: ");
        string? vehicleType = Console.ReadLine();
        if (vehicleType != "2" && vehicleType != "1")
            return "Invalid option.";

        Console.Write("Brand: ");
        string brand = Console.ReadLine() ?? "Unknown";

        Console.Write("Model: ");
        string model = Console.ReadLine() ?? "Unknown";

        Console.Write("Mileage: ");
        int mileage = int.TryParse(Console.ReadLine(), out int parsedMileage) ? parsedMileage : 0;

        switch (vehicleType)
        {
            case "1":
                Console.Write("Number of doors: ");
                int numberOfDoors = int.Parse(Console.ReadLine());
                inventory.Add(new Car(brand, model, mileage, numberOfDoors));
                return "Car added.";

            case "2":
                Console.Write("Has sidecar (y/n): ");
                string? hasSidecarInput = Console.ReadLine();
                bool hasSidecar = string.Equals(hasSidecarInput, "y");
                inventory.Add(new Motorcycle(brand, model, mileage, hasSidecar));
                return "Motorcycle added.";

            default:
                return "Invalid vehicle type.";
        }
    }

    static void ShowMenu(List<Vehicle> inventory)
    {
        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("\n=== Garage Menu ===");
            Console.WriteLine("1. List inventory");
            Console.WriteLine("2. Service all vehicles");
            Console.WriteLine("3. Add new vehicle");
            Console.WriteLine("4. Show miles remaining for service by id");
            Console.WriteLine("5. Show wash equipment by id");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListInventory(inventory);
                    ClearConsole();
                    break;

                case "2":
                    ServiceAllVehicles(inventory);
                    ClearConsole();
                    break;

                case "3":
                    AddNewVehicle(inventory);
                    ClearConsole();
                    break;

                case "4":
                    ShowServiceMilesForVehicle(inventory);
                    ClearConsole();
                    break;

                case "5":
                    ShowWashEquipmentForVehicle(inventory);
                    ClearConsole();
                    break;

                case "6":
                    isRunning = false;
                    Console.WriteLine("Goodbye!");
                    break;

                default:
                    Console.WriteLine("Invalid option. Try again.");
                    ClearConsole();
                    break;
            }
        }
    }
    static void ClearConsole()
    {
        Console.WriteLine("\nPress enter to continue");
        Console.ReadLine();
        Console.Clear();
    }

}
