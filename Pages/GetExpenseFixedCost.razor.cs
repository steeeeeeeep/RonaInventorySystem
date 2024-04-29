
using BikeSparesInventorySystem.Shared.Dialogs.ExpensesDialog;

namespace BikeSparesInventorySystem.Pages;

public partial class GetExpenseFixedCost
{
    public const string Route = "/expenses";
    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    private readonly TableEditTrigger EditTrigger = TableEditTrigger.EditButton;
    private string SearchString;

    private IEnumerable<Expenses> _expenses;

    [CascadingParameter]
    private Action<string> SetAppBarTitle { get; set; }
    [Parameter] public Action ChangeParentState { get; set; }
    [Inject] public IDialogService DialogService { get; set; }

    //private string Name {  get; set; }
    //private int fixedCost { get; set; }
    //private int directCost { get; set; }
    //private DateTime dateOfExpense { get; set; }


    protected sealed override void OnInitialized()
    {
        SetAppBarTitle.Invoke("Manage Expenses");
        _expenses = ExpensesRepository.GetAll();
    }

    protected async Task ShowAddExpenses()
    {
        DialogParameters parameters = new()
        {
            {"ChangeParentState", new Action(StateHasChanged) }
        };

        await DialogService.ShowAsync<AddExpenses>("Add Expenses", parameters);
    }

    protected async Task ShowDelete(Guid id)
    {
        var parameters = new DialogParameters<DeleteExpenses>();
        parameters.Add(x => x.ExpenseId, id);

        var options = new DialogOptions { DisableBackdropClick = true };

        var dialog = DialogService.Show<DeleteExpenses>("Delete expenses", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await ExpensesRepository.LoadAsync();
        }
    }

    protected async Task ShowEdit(Guid id)
    {
        var parameters = new DialogParameters<EditExpenses>();
        parameters.Add(x => x.ExpenseId, id);

        var options = new DialogOptions { DisableBackdropClick = true };

        var dialog = DialogService.Show<EditExpenses>("Edit expenses", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await ExpensesRepository.LoadAsync();
        }
    }

    //private void FilterByMonth(string a)
    //{
    //    ICollection<Expenses> expenses = ExpensesRepository.GetAll();
    //    if (string.IsNullOrEmpty(a))
    //    {
    //        _expenses = expenses;
    //        return;
    //    }
    //    string[] date = a.Split('-');
    //    _expenses = expenses.Where(d => d.CreatedAt.Year == int.Parse(date[0]) && d.CreatedAt.Month == int.Parse(date[1])).ToList();
    //}
}
