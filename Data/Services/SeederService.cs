namespace BikeSparesInventorySystem.Data.Services;

internal class SeederService
{
    internal int MinUsers { get; set; } = 30;
    internal int MaxUsers { get; set; } = 44;
    internal int MinCategories { get; set; } = 2;
    internal int MaxCategories { get; set; } = 5;
    internal int MinSpares { get; set; } = 50;
    internal int MaxSpares { get; set; } = 120;
    internal int MinActivityLogs { get; set; } = 1650;
    internal int MaxActivityLogs { get; set; } = 3200;
    internal int MinSales { get; set; } = 1500;
    internal int MaxSales { get; set; } = 1200;
    internal int MinExpenses { get; set; } = 150;
    internal int MaxExpenses { get; set; } = 120;
    internal int MinMiners { get; set; } = 5050;
    internal int MaxMiners { get; set; } = 10120;
    internal int MinOrders { get; set; } = 5050;
    internal int MaxOrders { get; set; } = 10120;
    internal int MinPurchases { get; set; } = 50;
    internal int MaxPurchases { get; set; } = 120;


    private readonly Repository<User> _userRepository;
    private readonly Repository<Spare> _spareRepository;
    private readonly Repository<Category> _categoryRepository;
    private readonly Repository<ActivityLog> _activityLogRepository;
    private readonly Repository<Sales> _sales;
    private readonly Repository<Expenses> _expenses;
    private readonly Repository<Miners> _miners;
    private readonly Repository<Orders> _orders;
    private readonly Repository<Purchases> _purchases;

    public SeederService(Repository<User> userRepository, 
                        Repository<Category> categoryRepository,
                        Repository<Spare> spareRepository, 
                        Repository<ActivityLog> activityLogRepository, 
                        Repository<Sales> sales,
                        Repository<Expenses> expenses,
                        Repository<Miners> miners,
                        Repository<Orders> orders,
                        Repository<Purchases> purchases )
    {
        _userRepository = userRepository;
        _spareRepository = spareRepository;
		_categoryRepository = categoryRepository;
        _activityLogRepository = activityLogRepository;
        _sales = sales;
        _expenses = expenses;
        _miners = miners;
        _orders = orders;
        _purchases = purchases;
    }

    public async Task SeedAsync()
    {
		ICollection<User> users = _userRepository._sourceData = Seeder.GenerateUsers(MinUsers, MaxUsers);
		ICollection<Category> categories = _categoryRepository._sourceData = Seeder.GenerateCategories(users, MinCategories, MaxCategories);
		ICollection<Spare> spares = _spareRepository._sourceData = Seeder.GenerateSpares(MinSpares, MaxSpares);
		//ICollection<Sales> sales = _sales._sourceData = Seeder.GenerateSales(MinSpares, MaxSpares);
		//ICollection<Expenses> expenses = _expenses._sourceData = Seeder.GenerateExpenses(MinSpares, MaxSpares);
		//ICollection<Miners> miners = _miners._sourceData = Seeder.GenerateMiners(MinSpares, MaxSpares);
        _activityLogRepository._sourceData = Seeder.GenerateActivityLogs(users, spares, categories, MinActivityLogs, MaxActivityLogs);
        await _userRepository.FlushAsync();
        await _spareRepository.FlushAsync();
        await _activityLogRepository.FlushAsync();
        await _categoryRepository.FlushAsync();
        //await _sales.FlushAsync();
        //await _expenses.FlushAsync();
        //await _miners.FlushAsync();
    }
}
