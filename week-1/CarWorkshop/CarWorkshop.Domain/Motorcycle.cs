namespace CarWorkshop.Domain;

public class Motorcycle : Vehicle
{
    public bool HasSidecar { get; }

    public Motorcycle(string brand, string model, int millage, bool hasSidecar) : base(brand, model, millage)
    {
        HasSidecar = hasSidecar;
    }

    public override string PerformService()
    {
        return $"Servicing the {Brand} {Model} Motorcycle.";
    }

    public override string GetSummary()
    {
        return $"{GetBaseSummary()} | Sidecar: {HasSidecar}";
    }
}