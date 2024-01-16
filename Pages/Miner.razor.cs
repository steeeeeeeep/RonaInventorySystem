using NPOI.SS.Formula.PTG;

namespace BikeSparesInventorySystem.Pages;

public partial class Miner
{
    public const string Route = "/miner";

    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private bool ReadOnly = false;
    private readonly bool CanCancelEdit = true;
    private readonly bool BlockSwitch = true;
    private Miners SelectedItem;
    private Miners ElementBeforeEdit;
    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    private readonly TableEditTrigger EditTrigger = TableEditTrigger.EditButton;
    private string SearchString;
    private IEnumerable<Miners> Miners;
    private IEnumerable<Category> Category;
    private readonly Dictionary<Guid, bool> OrderDescTrack = new();

    [CascadingParameter] private Action<string> SetAppBarTitle { get; set; }
    [Parameter] public Action ChangeParentState { get; set; }
    private MudForm form;
    public int? _orders = 0;

    private string MinersName;
    private string CodeName;
    private decimal Price;
    public DateTime ? CreatedAt;
    private bool IsSaving = false;

    private Dictionary<string, Miners> uniqueMiners = new Dictionary<string, Miners>();
    private Dictionary<string, int> orderCounts = new Dictionary<string, int>();
    private Dictionary<string, decimal> totalPrices = new Dictionary<string, decimal>();

    protected sealed override async Task OnParametersSetAsync()
    {
        SetAppBarTitle.Invoke("Today's Miners");
        await LoadData();
        await LoadCategory();

        if (!AuthService.IsUserAdmin())
        {
            ReadOnly = true;
        }
    }

    protected async Task LoadData()
    {
        await Task.Delay(1000);
        uniqueMiners.Clear();
        orderCounts.Clear();
        totalPrices.Clear();

        Miners = MinersRepository.GetAll();
        foreach (var miner in Miners)
        {
            var name = miner.Name;

            if (!uniqueMiners.ContainsKey(name))
            {
                uniqueMiners[name] = miner;
                totalPrices[name] = miner.Price;
                orderCounts[name] = 1;
            }
            else
            {
                orderCounts[name]++;
                totalPrices[name] += miner.Price;
            }
        }
    }

    protected async Task LoadCategory()
    {
        await Task.Delay(500);
        Category = CategoryRepository.GetAll();
    }

    private List<Miners> UniqueMiners => uniqueMiners.Values.Where(x => x.CreatedAt == CreatedAt).ToList();
    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        var result = uniqueMiners.Values.Where(x => x.CreatedAt == CreatedAt).Select(x=>x.Name).ToList();
        return result;
    }

    private async Task Submit()
    {
        await form.Validate();
        if (form.IsValid)
        {
            Miners Miner = new()
            {
                Name = MinersName,
                Code = CodeName,
                Price = Price,
                CreatedAt = CreatedAt
            };

            //var CheckMiners = MinersRepository.GetAll().Where(x => x.Name == MinersName).ToList();

            //if(CheckMiners.Any())
            //{
            //    _orders += 1;
            //}

            MinersRepository.Add(Miner);

            if (IsSaving)
            {
                return;
            }

            IsSaving = true;
            await MinersRepository.FlushAsync();
            await LoadData();
            IsSaving = false;
        }

        MinersName = "";
        CodeName = "";
        Price = 0;
    }

    protected async Task Update()
    {
        await MinersRepository.FlushAsync();
        Snackbar.Add("Successfuly Updated!", Severity.Success);
    }


    private void BackupItem(object element)
    {
        ElementBeforeEdit = ((Miners)element).Clone() as Miners;
    }

    private void ResetItemToOriginalValues(object element)
    {
        ((Miners)element).Name = ElementBeforeEdit.Name;
        ((Miners)element).Code = ElementBeforeEdit.Code;
        ((Miners)element).Price = ElementBeforeEdit.Price;
    }

    private bool FilterFunc(Miners miner)
    {
        return string.IsNullOrWhiteSpace(SearchString)
            || miner.Name.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
            || miner.Code.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
            || miner.Price.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)
            || miner.CreatedAt.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase);
    }

}
