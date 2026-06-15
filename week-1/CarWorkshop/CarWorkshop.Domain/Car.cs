namespace CarWorkshop.Domain;

public class Car : Vehicle
{
    public int NumberOfDoors { get; }

    public Car(string brand, string model, int millage, int numberOfDoors) : base(brand, model, millage)
    {
        NumberOfDoors = numberOfDoors;
    }
    public override string EquipmentToWashIt()
    {
        return "Use the Car wash tunnel";
    }
    public override string PerformService()
    {
        return $"Servicing the {Brand} {Model} Car.";
    }

    public override string GetSummary()
    {
        return $"{GetBaseSummary()} | Doors: {NumberOfDoors}";
    }
}