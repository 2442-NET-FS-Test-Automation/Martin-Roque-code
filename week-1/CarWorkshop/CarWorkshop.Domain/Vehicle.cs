namespace CarWorkshop.Domain;

public abstract class Vehicle : IVehicleService
{
    private static int _nextId = 1;

    public int Id { get; }
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public int? Mileage { get; }

    public Vehicle()
    {
        Id = _nextId++;
    }

    public Vehicle(string brand, string model, int millage)
        : this()
    {
        Brand = brand;
        Model = model;
        Mileage = millage;
    }

    public string MileageRemainingForService(int millage)
    {
        return $"Mileage remaining for next service: {6000 - (millage % 6000)}";
    }

    public string MileageRemainingForService()
    {
        int mileage = Mileage ?? 0;
        return MileageRemainingForService(mileage);
    }

    protected string GetBaseSummary()
    {
        return $"Id: {Id} | {Brand} {Model} | Mileage: {Mileage}";
    }

    public virtual string EquipmentToWashIt()
    {
        return $"Use the presure washer";
    }
    public abstract string PerformService();
    public abstract string GetSummary();
}