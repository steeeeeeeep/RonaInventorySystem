
namespace BikeSparesInventorySystem.Pages.Contribution;

public partial class GetContributions
{
    public const string Route = "/contributions";

    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private readonly bool ReadOnly = false;
    private readonly bool BlockSwitch = true;
    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    protected IEnumerable<Contributions> _Contributions;

    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    protected sealed override async Task OnInitializedAsync()
    {
        await Task.Delay(1000);
        SetAppBarTitle.Invoke("Contributions");
        _Contributions = ContributionRepository.GetAll();
    }

    protected async Task AddContribution()
    {
        DialogParameters parameters = new()
        {
            { "ChangeParentState", new Action(StateHasChanged) }
        };

        await DialogService.ShowAsync<AddContribution>("Add Contribution", parameters);
    }

    protected async Task Update()
    {
        await ContributionRepository.FlushAsync();
        Snackbar.Add("Successfuly Updated!", Severity.Success);
    }

    protected async Task Remove(Guid id)
    {
        var DataToRemove = ContributionRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
        ContributionRepository.Remove(DataToRemove);
        await ContributionRepository.FlushAsync();
        await Task.Delay(500);
        StateHasChanged();
    }

    protected MudBlazor.Color GetChipVariant(Status status)
    {
        switch(status)
        {
            case Status.Pending:
                return MudBlazor.Color.Info;
            case Status.Active:
                return MudBlazor.Color.Success;
            case Status.Inactive:
                return MudBlazor.Color.Warning;
            default:
                return MudBlazor.Color.Default;
        }
    }
}
