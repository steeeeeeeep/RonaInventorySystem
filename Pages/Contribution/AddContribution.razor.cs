namespace BikeSparesInventorySystem.Pages.Contribution;

public partial class AddContribution
{

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public Action ChangeParentState { get; set; }

    protected MudForm Form = default!;
    protected bool IsSaving;
    protected bool Disable;

    public double ? Tithes;
    public double ? Charity;
    public double ? Car;
    public double ? Others;
    public Status Status;

    protected async Task SubmitContribution()
    {
        IsSaving = true;
        await Form.Validate();

        if(Form.IsValid)
        {
            try
            {
                Contributions contributions = new()
                {
                    Tithes = Tithes,
                    Charity = Charity,
                    CarExpense = Car,
                    Others = Others,
                    Status = Status
                };

                ContributionRepository.Add(contributions);
                await ContributionRepository.FlushAsync();
                await Task.Delay(300);
                ChangeParentState.Invoke();

                Snackbar.Add($"Contribution is added successfully");
                MudDialog.Close();
            }
            finally { IsSaving = false; }
        }
    }

    protected void Cancel() => MudDialog.Cancel();

}
