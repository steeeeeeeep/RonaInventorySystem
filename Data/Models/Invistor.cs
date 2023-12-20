namespace BikeSparesInventorySystem.Data.Models;

public class Invistor : IModel, ICloneable
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public decimal Percent {  get; set; }

    public object Clone()
    {
        return new Invistor
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Description = Description,
            Amount = Amount,
            Percent = Percent
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
