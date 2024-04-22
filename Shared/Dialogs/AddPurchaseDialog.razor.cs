namespace BikeSparesInventorySystem.Shared.Dialogs;

public partial class AddPurchaseDialog
{

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public Action ChangeParentState { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    private MudForm form;
    private IEnumerable<Spare> Products;

    private string Name;
    private Guid ProductId;
    private decimal Amount;
    private int Items;
    private DateTime ? Acquired;
    private bool IsSaving;

    protected sealed override void OnParametersSet()
    {
        Products = ProductRepository.GetAll();
        ProductId = Products.Select(x => x.Id).FirstOrDefault();
    }

    private async Task AddPurchase()
    {
        await form.Validate();
        if (form.IsValid)
        {
            try
            {
                IsSaving = true;
                var product = ProductRepository.Get(x => x.Id, ProductId);
                Purchases purchases = new()
                {
                    Name = Name,
                    ProductId = ProductId,
                    ProductName = product.Name,
                    Amount = Amount,
                    Items = Items,
                    Acquired = Acquired
                };

                PurchaseRepository.Add(purchases);
                var result = PurchaseRepository.FlushAsync();

                if(result.IsCompletedSuccessfully)
                {
                    product.AvailableQuantity += Items;
                    await ProductRepository.FlushAsync();

                    await Task.Delay(250);
                }

                ChangeParentState.Invoke();

                Snackbar.Add($"{Name} is added!", Severity.Success);
                MudDialog.Close();
            }
            finally 
            {
                IsSaving = false;
            }
        }
    }

    private void Cancel() => MudDialog.Cancel();

}
