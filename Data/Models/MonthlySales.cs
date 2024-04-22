
namespace BikeSparesInventorySystem.Data.Models;

public class MonthlySales : IModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DailySalesId { get; set; }
    public decimal GrossSale { get; set; }
    public decimal MonthlyNetIncome { get; set; }
    public decimal Tithes { get; set; }
    public decimal Charity { get; set; }
    public decimal OtherContribution { get; set; }
    public decimal Expenses { get; set; }
    public decimal Car { get; set; }
    public decimal Profit { get; set; }
    public decimal SalesEntry { get; set; }
    public decimal Purchases { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime? CreatedAt { get; set; }

    public object Clone()
    {
        return new MonthlySales
        {
            Id = Id,
            DailySalesId = DailySalesId,
            GrossSale = GrossSale,
            MonthlyNetIncome = MonthlyNetIncome,
            Tithes = Tithes,
            Charity = Charity,
            OtherContribution = OtherContribution,
            Expenses = Expenses,
            Car = Car,
            Profit = Profit,
            SalesEntry = SalesEntry,
            Purchases = Purchases,
            Month = Month,
            Year = Year,
            CreatedAt = CreatedAt
        };
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
