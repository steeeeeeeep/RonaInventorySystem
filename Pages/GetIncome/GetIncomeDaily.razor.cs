namespace BikeSparesInventorySystem.Pages.GetIncome;

public partial class GetIncomeDaily
{
    public const string Route = "/dailyIncome";

    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private readonly bool ReadOnly = false;
    private readonly bool BlockSwitch = true;
    private Sales SelectedItem;
    private Sales ElementBeforeEdit;
    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    protected Guid categoryId;
    protected int count;
    private string SearchString;
    private IEnumerable<Sales> _Sales;
    private IEnumerable<Purchases> _Purchases;
    private IEnumerable<Miners> _Miners;
    private IEnumerable<Expenses> _Expenses;
    private IEnumerable<Contributions> _Contributions;
    private Contributions _GetActiveContributions;

    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }

    public Guid PurchaseId { get; set; }
    public Guid ExpenseId { get; set; }

    protected sealed override void OnInitialized()
    {
        SetAppBarTitle.Invoke("Manage Sales");
        _Sales = SalesRepository.GetAll().OrderBy(s => s.DailyDate);
        _Purchases = PurchaseRepository.GetAll();
        _Expenses = ExpensesRepository.GetAll();
        _GetActiveContributions = ContributionRepository.Get(s => s.Status, Status.Active);
    }

    private void BackupItem(object element)
    {
        ElementBeforeEdit = ((Sales)element).Clone() as Sales;
    }

    public async Task SaveInSales()
    {

        _Miners = MinersRepository.GetAll();
        _Contributions = ContributionRepository.GetAll();
        var ActiveContribution = _Contributions.Where(s => s.Status == Status.Active).FirstOrDefault();

        var grossSalesPerDay = _Miners
            .Where(x => x.Status == "Delivered")
            .GroupBy(x => x.CreatedAt.Value.Date)  // Group by date without considering time
            .Select(group => new
            {
                Date = group.Key,
                GrossSale = group.Sum(x => x.Price),
                Id = group.Select(x => x.Id).FirstOrDefault(),
            })
            .OrderBy(entry => entry.Date);

        foreach (var saleData in grossSalesPerDay)
        {
            if (!_Sales.Any(x => x.DailyDate == saleData.Date) && !_Sales.Any(x => x.Id == saleData.Id))
            {
                Sales sales = new()
                {
                    GrossSale = saleData.GrossSale,
                    DailyDate = saleData.Date,
            };

                SalesRepository.Add(sales);
                await SalesRepository.FlushAsync();
                Snackbar.Add("Sales is updated!", Severity.Info);
            }
            else if (_Sales.Any(x => x.DailyDate == saleData.Date) && !_Sales.Any(x => x.Id == saleData.Id))
            {
                var existing = SalesRepository.Get(x => x.DailyDate, saleData.Date);
                if (existing != null && ActiveContribution != null)
                {
                    existing.GrossSale = saleData.GrossSale;
                }

                await SalesRepository.FlushAsync();
                Snackbar.Add("Sales is up to date!", Severity.Info);
            }
            else
            {
                Snackbar.Add("Sales is up to date!", Severity.Warning);
            }
        }
    }

    protected async Task Update()
    {
        var grossSale = SalesRepository.Get(x => x.Id, SelectedItem.Id)
                        .GrossSale;
        var purchase = _Purchases.Where(x => x.Id == PurchaseId)
                    .Select(x => x.Amount).FirstOrDefault();

        var expenses = _Expenses.Where(x => x.Id == ExpenseId)
                    .Select(x => x.DirectCostId).FirstOrDefault();

        var contributions = ContributionRepository.Get(s => s.Status, Status.Active);

        var netSale = grossSale - purchase - expenses;
        var Tithes = ((decimal)contributions.Tithes / 100) * netSale;
        var Charity = ((decimal)contributions.Charity / 100) * netSale;
        var Car = ((decimal)contributions.CarExpense / 100) * netSale;
        var OtherContribution = ((decimal)contributions.Others / 100) * netSale;

        var existingSales = SalesRepository.Get(x => x.Id, SelectedItem.Id);

        if (existingSales != null)
        {
            existingSales.Purchases = purchase;
            existingSales.Expenses = expenses;
            existingSales.Tithes = Tithes;
            existingSales.Charity = Charity;
            existingSales.Car = Car;
            existingSales.OtherContribution = OtherContribution;
            existingSales.DailyNetIncome = netSale;
            existingSales.Profit = netSale - Tithes - Charity - Car - OtherContribution;
        }
        await SalesRepository.FlushAsync();
    }

    private bool FilterFunc(Sales sales)
    {
        return string.IsNullOrWhiteSpace(SearchString)
        || sales.Id.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.DailyDate.ToString("MM/dd/yyyy").Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.PurchaseDate.ToString("MM/dd/yyyy").Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.DailyNetIncome.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.Purchases.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.GrossSale.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
        || sales.Profit.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase);
    }

    private void FilterByMonth(string a)
    {
        ICollection<Sales> _sales = SalesRepository.GetAll();
        if (string.IsNullOrEmpty(a))
        {
            _Sales = _sales;
            return;
        }
        string[] date = a.Split('-');
        _Sales = _sales.Where(d => d.DailyDate.Year == int.Parse(date[0]) && d.DailyDate.Month == int.Parse(date[1])).ToList();
    }
}
