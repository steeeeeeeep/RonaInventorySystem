namespace BikeSparesInventorySystem.Shared.Dialogs.PurchasesDialog;

public partial class DeletePurchases
{
    [Parameter]
    public Guid PurchaseId { get; set; }

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; } = default!;

    public bool isSaving;

    protected async Task DeletePurchasesAsync()
    {
        var idExists = PurchasesRepository.Get(x => x.Id, PurchaseId);
        var product = ProductRepository.Get(x => x.Id, idExists.ProductId);

        if (idExists != null)
        {
            isSaving = true;
            PurchasesRepository.Remove(idExists);

            var result = PurchasesRepository.FlushAsync();
            if(result.IsCompleted)
            {
                product.AvailableQuantity -= idExists.Items;
                await ProductRepository.FlushAsync();
            }
        }

        await Task.Delay(300);
        MudDialog.Close();
    }

    protected void Cancel() => MudDialog.Close();
}
