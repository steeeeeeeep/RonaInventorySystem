namespace BikeSparesInventorySystem.Pages;

public partial class GetInvistors 
{
    public const string route = "/invistors";
    private readonly bool Dense = true;
    private readonly bool Fixed_header = true;
    private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private bool ReadOnly = false;
    private readonly bool CanCancelEdit = true;
    private readonly bool BlockSwitch = true;
    private string SearchString;
    private Spare SelectedItem;
    private Spare ElementBeforeEdit;

    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    private readonly TableEditTrigger EditTrigger = TableEditTrigger.RowClick;
    private IEnumerable<Invistor> Elements;
    private readonly Dictionary<Guid, bool> SpareDescTracks = new();


    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }
    protected sealed override void OnInitialized()
    {
        SetAppBarTitle.Invoke("Invistors");
        Elements = InvistorRepository.GetAll();
    }
}
