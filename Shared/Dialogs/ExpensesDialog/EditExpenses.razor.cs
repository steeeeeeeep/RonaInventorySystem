namespace BikeSparesInventorySystem.Shared.Dialogs.ExpensesDialog;

public partial class EditExpenses
{
    [Parameter]
    public Guid ExpenseId { get; set; }

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; } = default!;

    public Expenses expenses;
    public MudForm form;
    public bool isSaving;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        expenses = ExpensesRepository.Get(x => x.Id, ExpenseId);
    }
    private async Task EditExpensesAsync()
    {
        await form.Validate();

        if (form.IsValid)
        {
            isSaving = true;
            var expenseExists = ExpensesRepository.Get(s => s.Id, ExpenseId);
            if (expenseExists != null)
            {
                expenseExists.DateOfExpense = expenses.DateOfExpense;
                expenseExists.Amount = expenses.Amount;
                expenseExists.Name = expenses.Name;

                await ExpensesRepository.FlushAsync();
            }

            await Task.Delay(300);
            MudDialog.Close();
        }
    }

    private void cancel() => MudDialog.Close();
}
