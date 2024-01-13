namespace BikeSparesInventorySystem.Shared.Dialogs;

public partial class AddFixedCost
{
    [CascadingParameter] public MudDialogInstance Dialog {  get; set; }
    [Parameter] public Action ChangeParentState { get; set; }

    private MudForm mudForm;
    private string Error { get; set; }
    private bool errorStatus;
    //private int numberValue;
    private string Name {  get; set; }
    private int FixedCost { get; set; }
    private int DirectCost { get; set; }
    private DateTime? DateOfExpense { get; set; }

    private async Task AddExpense()
    {
        await mudForm.Validate();
        if(mudForm.IsValid)
        {
            if (DirectCost == 0)
            {
                errorStatus = true;
                Error = "Expenses should not be 0 \uD83D\uDE01";
            }
            else
            {
                Expenses expenses = new()
                {
                    Name = Name,
                    FixedCostId = FixedCost,
                    DirectCostId = DirectCost,
                    DateOfExpense = DateOfExpense
                };

                ExpensesRepository.Add(expenses);
                ChangeParentState.Invoke();
                await ExpensesRepository.FlushAsync();

                Dialog.Close();
            }

        }
    }

    private void Cancel () => Dialog.Cancel();
}
