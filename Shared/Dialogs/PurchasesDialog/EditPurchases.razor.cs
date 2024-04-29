using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeSparesInventorySystem.Shared.Dialogs.PurchasesDialog;

public partial class EditPurchases
{
    [Parameter]
    public Guid PurchaseId { get; set; }

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; } = default!;

    public Purchases purchases;
    public MudForm form;
    public bool isSaving;


    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        purchases = PurchasesRepository.Get(x => x.Id, PurchaseId);
    }

    protected async Task EditPurchasesAsync()
    {
        await form.Validate();

        if(form.IsValid)
        {
            isSaving = true;

            var purchaseExists = PurchasesRepository.Get(x => x.Id, PurchaseId);
            if(purchaseExists != null)
            {
                purchaseExists.Amount = purchases.Amount;
                purchaseExists.Name = purchases.Name;
                purchaseExists.Acquired = purchases.Acquired;

                await PurchasesRepository.FlushAsync();
            }

            await Task.Delay(300); 
            MudDialog.Close();
        }
    }

    protected void Cancel() => MudDialog.Cancel();
}
