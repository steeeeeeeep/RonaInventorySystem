namespace BikeSparesInventorySystem.Pages;

public partial class GetInvistors 
{
    public const string Route = "/invistors";
    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private readonly bool ReadOnly = false;
    private readonly bool CanCancelEdit = true;
    private string SearchString;

    private IEnumerable<Invistor> Elements;


    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }

    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    public DialogOptions _options = new() { MaxWidth = MaxWidth.Large };

    protected sealed override void OnInitialized()
    {
        SetAppBarTitle.Invoke("Invistors");
        Elements = InvistorRepository.GetAll();
    }

    protected async Task AddDialog()
    {
        DialogParameters parameters = new()
        {
            { "ChangeParentState", new Action(StateHasChanged) }
        };

        await Dialog.ShowAsync<AddInvistor>("Add Invistor", parameters, _options);
    }
}
