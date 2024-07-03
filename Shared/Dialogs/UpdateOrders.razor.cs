using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeSparesInventorySystem.Shared.Dialogs;

public partial class UpdateOrders
{
    [Parameter] public Guid id { get; set; }
    [CascadingParameter] public MudDialogInstance MuddDialog { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    public Miners Orders;
    public MudForm form = new();
    public bool IsSaving;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Orders = OrdersRepository.Get(o => o.Id, id);
    }

    protected async Task Update()
    {
        await form.Validate();
        if (form.IsValid)
        {
            IsSaving = true;
            var existingOrders = OrdersRepository.Get(o => o.Id, id);
            var product = ProductRepository.Get(p => p.Id, existingOrders.ProductId);

			if (existingOrders != null)
			{

				if (Orders.Status == "Pending" && existingOrders.Status != "Pending")
				{
					product.AvailableQuantity++;
					Snackbar.Add("Order has been updated!", Severity.Success);
				}
				else if (Orders.Status == "Cancelled" && existingOrders.Status != "Cancelled")
				{
					product.AvailableQuantity--;
					Snackbar.Add("Order has been updated!", Severity.Success);
				}
				else
				{
                    Snackbar.Add("Order has been updated!", Severity.Success);
				}

				existingOrders.Status = Orders.Status;
                existingOrders.Price = Orders.Price;
            }

            await OrdersRepository.FlushAsync();
            await ProductRepository.FlushAsync();
            await Task.Delay(500);
            MuddDialog?.Close();
        }
    }

    protected void Cancel() => MuddDialog.Cancel();
}
