namespace BikeSparesInventorySystem.Shared.Dialogs.ExpensesDialog;

public partial class AddExpenses
{
    [CascadingParameter] public MudDialogInstance Dialog { get; set; }
    [Parameter] public Action ChangeParentState { get; set; }

    private MudForm mudForm;
    private string Error { get; set; }
    private bool errorStatus;
    //private int numberValue;
    private string Name { get; set; }
    private int Amount { get; set; }
    private DateTime? DateOfExpense { get; set; }

    private async Task AddExpense()
    {
        await mudForm.Validate();
        if (mudForm.IsValid)
        {
            if (Amount == 0)
            {
                errorStatus = true;
                Error = "Expenses should not be 0 \uD83D\uDE01";
            }
            else
            {
                Expenses expenses = new()
                {
                    Name = Name,
                    Amount = Amount,
                    DateOfExpense = DateOfExpense
                };

                ExpensesRepository.Add(expenses);
                ChangeParentState.Invoke();
                await ExpensesRepository.FlushAsync();

                Dialog.Close();
            }

        }
    }

    private void Cancel() => Dialog.Cancel();
}
