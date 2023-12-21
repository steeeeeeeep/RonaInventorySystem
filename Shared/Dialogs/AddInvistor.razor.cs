
namespace BikeSparesInventorySystem.Shared.Dialogs;

public partial class AddInvistor
{
    [CascadingParameter] public MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public Action ChangeParentState { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    private MudForm form;
    private string FirstName;
    private string LastName;
    private string Description;
    private decimal Amount;
    private decimal Percent;


    protected async Task Submit()
    {
        await form.Validate();
        if(form.IsValid)
        {
            try
            {
                Invistor invistor = new()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Description = Description,
                    Amount = Amount,
                    Percent = Percent,
                };

                InvistorRepository.Add(invistor);
                await InvistorRepository.FlushAsync();
                ChangeParentState.Invoke();

                Snackbar.Add("Invistor has been added", Severity.Success);
            }
            catch (Exception ex)
            {
                new Exception("CategoryId is null");
            }
        }
    }

    protected void Cancel() => MudDialog.Cancel();
}
