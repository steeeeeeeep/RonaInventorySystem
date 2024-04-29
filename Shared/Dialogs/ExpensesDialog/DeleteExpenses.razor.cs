namespace BikeSparesInventorySystem.Shared.Dialogs.ExpensesDialog;

public partial class DeleteExpenses
{
    [Parameter]
    public Guid ExpenseId { get; set; }

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; } = default!;

    public bool isSaving;
    private async Task DeleteExpense()
    {
        isSaving = true;
        var idExists = ExpensesRepository.Get(s => s.Id, ExpenseId);
        if(idExists != null)
        {
            ExpensesRepository.Remove(idExists);
            await ExpensesRepository.FlushAsync();
        }

        await Task.Delay(300);
        MudDialog.Close();
    }

    private void cancel() => MudDialog.Close();

}
