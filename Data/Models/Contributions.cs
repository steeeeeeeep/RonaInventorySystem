

namespace BikeSparesInventorySystem.Data.Models;

public class Contributions : IModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double ? Tithes { get; set; }
    public double ? CarExpense { get; set; }
    public double ? Charity { get; set; }
    public double ? Others { get; set; }
    public Status Status { get; set; }

    public object Clone()
    {
        return new Contributions
        {
            Id = Id,
            Tithes = Tithes,
            CarExpense = CarExpense,
            Charity = Charity,
            Others = Others,
            Status = Status
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
