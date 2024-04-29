namespace BikeSparesInventorySystem.Pages.GetIncome;

public partial class GetIncomeMonthly
{
    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private bool ReadOnly = false;
    private readonly bool BlockSwitch = true;
    protected int count;
    private string SearchString;
    private IEnumerable<MonthlySales> _MonthlySales;
    private IEnumerable<Purchases> _Purchases;
    private IEnumerable<Expenses> _Expenses;
    private IEnumerable<Contributions> _Contributions;
    private IEnumerable<Sales> _Sales;
    private Contributions _GetActiveContribution;
    private IEnumerable<Contributions> _GetActiveContributions;
    private MonthlySales SelectedMonthlySale;

    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }
    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    public HashSet<Purchases> PurchaseId { get; set; } = new HashSet<Purchases>();
    public HashSet<Expenses> ExpenseId { get; set; } = new HashSet<Expenses>();

    protected sealed override void OnInitialized()
    {
        SetAppBarTitle.Invoke("Manage Sales");
        _MonthlySales = MonthlySalesRepository.GetAll();
        _Purchases = PurchaseRepository.GetAll();
        _Expenses = ExpensesRepository.GetAll();
        _GetActiveContributions = ContributionRepository.GetAll();
        _GetActiveContribution = _GetActiveContributions.Where(c => c.Status == Status.Active).FirstOrDefault();
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
            _Sales.GroupBy(d => Tuple.Create(d.DailyDate.Month, d.DailyDate.Year ))
            .Select(group => new
            {
                Month = group.Key.Item1,
                Year = group.Key.Item2,
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
            .OrderBy(o => o.Month);

        var getExpenseList = _Expenses.GroupBy(d => Tuple.Create(d.DateOfExpense.Value.Month, d.DateOfExpense.Value.Year))
            .Select(group => new
            {
                Month = group.Key.Item1,
                Year = group.Key.Item2,
                Amount = group.Sum(s => s.Amount)
            });

        var getPurchaseList = _Purchases.GroupBy(d => Tuple.Create(d.Acquired.Value.Month, d.Acquired.Value.Year))
            .Select(group => new
            {
                Month = group.Key.Item1,
                Year = group.Key.Item2,
                Amount = group.Sum(s => s.Amount)
            });

        if (!_Sales.Any())
        {
            Snackbar.Add("Sync Daily Income first!");
            return;
        }

        if (_GetActiveContribution == null)
        {
            Snackbar.Add("Add contribution before proceeding!");
            return;
        }

        foreach (var sale in getData)
        {
            var getExpense = getExpenseList.Where(e => e.Month == sale.Month && e.Year == sale.Year).FirstOrDefault();
            var getPurchase = getPurchaseList.Where(p => p.Month == sale.Month && p.Year == sale.Year).FirstOrDefault();
            if (!_MonthlySales.Any(a => a.Month == sale.Month && a.Year == sale.Year))
            {
                if (getExpense != null || getPurchase != null)
                {
                    var monthlyIncome = sale.GrossSale - getPurchase?.Amount ?? 0 - getExpense?.Amount ?? 0;
                    MonthlySales newMonthlySales = new()
                    {
                        GrossSale = sale.GrossSale,
                        MonthlyNetIncome = monthlyIncome,
                        Tithes = sale.Tithes,
                        Car = sale.Car,
                        Charity = sale.Charity,
                        OtherContribution = sale.OtherContribution,
                        Profit = monthlyIncome - (sale.Tithes + sale.Car + sale.Charity + sale.OtherContribution),
                        Expenses = getExpense?.Amount ?? 0,
                        Purchases = getPurchase?.Amount ?? 0,
                        Month = sale.Month,
                        Year = sale.Year
                    };

                    MonthlySalesRepository.Add(newMonthlySales);
                }
                else
                {
                    MonthlySales newMonthlySales = new()
                    {
                        GrossSale = sale.GrossSale,
                        MonthlyNetIncome = sale.GrossSale,
                        Tithes = sale.Tithes,
                        Car = sale.Car,
                        Charity = sale.Charity,
                        OtherContribution = sale.OtherContribution,
                        Profit = sale.GrossSale - (sale.Tithes + sale.Car + sale.Charity + sale.OtherContribution),
                        Month = sale.Month,
                        Year = sale.Year
                    };

                    MonthlySalesRepository.Add(newMonthlySales);
                }

                await MonthlySalesRepository.FlushAsync();
                Snackbar.Add("Sales is updated!", Severity.Info);
            }
            else if (_MonthlySales.Any(a => a.Month == sale.Month && a.Year == sale.Year))
            {
                var existing = MonthlySalesRepository.Get(g => (g.Month, g.Year), (sale.Month, sale.Year));
                var expenses = getExpense?.Amount ?? existing.Expenses ;
                var purchases = getPurchase?.Amount ?? existing.Purchases;

                if (existing != null)
                { 
                    if(existing.GrossSale != sale.GrossSale)
                    {
                        existing.GrossSale = sale.GrossSale;
                    }

                    existing.MonthlyNetIncome = existing.GrossSale - (expenses + purchases);
                    existing.Tithes = existing.MonthlyNetIncome * ((decimal)_GetActiveContribution.Tithes / 100);
                    existing.Car = existing.MonthlyNetIncome * ((decimal)_GetActiveContribution.CarExpense / 100);
                    existing.Charity = existing.MonthlyNetIncome * ((decimal)_GetActiveContribution.Charity / 100);
                    existing.OtherContribution = existing.MonthlyNetIncome * ((decimal)_GetActiveContribution.Others / 100);
                    existing.Expenses = getExpense?.Amount ?? 0;
                    existing.Purchases = getPurchase?.Amount ?? 0;
                    existing.Profit = existing.MonthlyNetIncome - existing.Tithes - existing.Car - existing.Charity - existing.OtherContribution;

                }

                await MonthlySalesRepository.FlushAsync();
                Snackbar.Add("Sync complete!", Severity.Info);
            }
            else
            {
                Snackbar.Add("Sales is up to date!", Severity.Warning);
            }
        }

    }

    protected async Task UpdateMonthlySales()
    {
        decimal totalPurchases = 0.0m;
        decimal totalExpenses = 0.0m;
        if(PurchaseId.Count > 0)
        {
            foreach(var p in PurchaseId)
            {
                if(_Purchases.Any(x => x.Id == p.Id))
                {
                    var purchase = _Purchases.Where(x => x.Id == p.Id).Select(x => x.Amount).FirstOrDefault();
                    totalPurchases += purchase;
                }
            }
        }

        if(ExpenseId.Count > 0)
        {
            foreach(var e in ExpenseId)
            {
                if(_Expenses.Any(x => x.Id == e.Id))
                {
                    var expense = _Expenses.Where(x => x.Id == e.Id).Select(x => x.Amount).FirstOrDefault();
                    totalExpenses += expense;
                }
            }
        }

        var getMonthlySale = MonthlySalesRepository.Get(x => x.Id, SelectedMonthlySale.Id);
        var contributions = ContributionRepository.Get(s => s.Status, Status.Active);

        var netSale = getMonthlySale.MonthlyNetIncome;
        var Tithes = ((decimal)contributions.Tithes / 100) * netSale;
        var Charity = ((decimal)contributions.Charity / 100) * netSale;
        var Car = ((decimal)contributions.CarExpense / 100) * netSale;
        var OtherContribution = ((decimal)contributions.Others / 100) * netSale;

        if (totalExpenses > 0 && totalPurchases > 0)
        {
            getMonthlySale.Purchases = totalPurchases;
            getMonthlySale.Expenses = totalExpenses;
            getMonthlySale.Profit = getMonthlySale.MonthlyNetIncome - (totalPurchases + totalExpenses + Tithes + Charity + Car + OtherContribution);
            getMonthlySale.Tithes = Tithes;
            getMonthlySale.Car = Car;
            getMonthlySale.Charity = Charity;
            getMonthlySale.OtherContribution = OtherContribution;
        }

        await MonthlySalesRepository.FlushAsync();
    }

}
