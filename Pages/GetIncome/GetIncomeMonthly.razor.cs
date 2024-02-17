namespace BikeSparesInventorySystem.Pages.GetIncome;

public partial class GetIncomeMonthly
{
    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private bool ReadOnly = false;
    private readonly bool CanCancelEdit = true;
    private readonly bool BlockSwitch = true;
    private Sales SelectedItem;
    private Sales ElementBeforeEdit;
    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    private readonly TableEditTrigger EditTrigger = TableEditTrigger.EditButton;
    protected int count;
    private string SearchString;
    private IEnumerable<MonthlySales> _MonthlySales;
    private IEnumerable<Sales> _Sales;
    private Contributions _GetActiveContributions;

    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }
    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    protected sealed override void OnInitialized()
    {
        SetAppBarTitle.Invoke("Manage Sales");
        _MonthlySales = MonthlySalesRepository.GetAll();
        _GetActiveContributions = ContributionRepository.Get(s => s.Status, Status.Active);
    }

    private bool FilterFunc(MonthlySales sales)
    {
        return string.IsNullOrWhiteSpace(SearchString)
        || sales.Id.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.Month.ToString("MM/dd/yyyy").Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.Purchases.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.GrossSale.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.Profit.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase);
    }

    private void FilterByMonth(string a)
    {
        ICollection<MonthlySales> _sales = MonthlySalesRepository.GetAll();
        if (string.IsNullOrEmpty(a))
        {
            _MonthlySales = _sales;
            return;
        }
        string[] date = a.Split('-');
        //_MonthlySales = _sales.Where(d => d.Month.Year == int.Parse(date[0]) && d.Month.Month == int.Parse(date[1])).ToList();
    }

    private async Task SyncSales()
    {
        _Sales = SalesRepository.GetAll().ToList();
        var getData = 
            _Sales.GroupBy(d => d.DailyDate.Date)
            .Select(group => new
            {
                Id = group.Select(x => x.Id).FirstOrDefault(),
                Date = group.Key,
                GrossSale = group.Sum(s => s.GrossSale),
                MonthlySale = group.Sum(s => s.DailyNetIncome),
                Expense = group.Sum(s => s.Expenses),
                _Purchase = group.Sum(s => s.Purchases),
                Tithes = group.Sum(s => s.Tithes),
                Car = group.Sum(s => s.Car),
                Charity = group.Sum(s => s.Charity),
                OtherContribution = group.Sum(s => s.OtherContribution),
                Profit = group.Sum(s => s.Profit)
            })
            .OrderBy(o => o.Date);

        foreach(var sale in getData)
        {
            if(!_MonthlySales.Any(a => a.Month == sale.Date.Month && a.Year == sale.Date.Year) 
                && !_MonthlySales.Any(a => a.Id == sale.Id))
            {
                MonthlySales newMonthlySales = new()
                {
                    GrossSale = sale.GrossSale,
                    MonthlyNetIncome = sale.MonthlySale,
                    Tithes = sale.Tithes,
                    Car = sale.Car,
                    Charity = sale.Charity,
                    OtherContribution = sale.OtherContribution,
                    Profit = sale.Profit,
                    Expenses = sale.Expense,
                    Purchases = sale._Purchase,
                    Month = sale.Date.Month,
                    Year = sale.Date.Year
                };

                MonthlySalesRepository.Add(newMonthlySales);
                await MonthlySalesRepository.FlushAsync();
                Snackbar.Add("Sales is updated!", Severity.Info);
            }
            else if(_MonthlySales.Any(a => a.Month == sale.Date.Month && a.Year == sale.Date.Year)
                && !_MonthlySales.Any(a => a.Id == sale.Id))
            {
                var existing = MonthlySalesRepository.Get(g => g.Month, sale.Date.Month);
                if (existing != null && existing.Year == sale.Date.Year)
                {
                    existing.GrossSale = sale.GrossSale;
                    existing.Tithes = sale.Tithes;
                    existing.Car = sale.Car;
                    existing.Charity = sale.Charity;
                    existing.OtherContribution = sale.OtherContribution;
                    existing.Profit = sale.Profit;
                    existing.Expenses = sale.Expense;
                    existing.Purchases = sale._Purchase;
                    existing.Month = sale.Date.Month;
                    existing.Year = sale.Date.Year;
                }
                else
                {
                    Snackbar.Add("Cannot get monthly income!");
                }

                await MonthlySalesRepository.FlushAsync();
                Snackbar.Add("Sync complete", Severity.Info);
            }
            else
            {
                Snackbar.Add("Sales is up to date!", Severity.Warning);
            }
        }

    }

}
