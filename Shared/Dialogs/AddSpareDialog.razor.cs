﻿namespace BikeSparesInventorySystem.Shared.Dialogs;

public partial class AddSpareDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter] public Action ChangeParentState { get; set; }

    private MudForm form;
    public bool _error;

    private string Name;
    private string Description;
    private Guid CategoryId;
    private decimal Price;
    private int AvailableQuantity;
    private int ActualQuantity;
    private IEnumerable<Category> Elements;

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    protected sealed override void OnParametersSet()
    {
        Elements = CategoryRepository.GetAll();
    }

    private async Task AddSpare()
    {
        await form.Validate();
        if (form.IsValid)
        {
            try
            {
                Spare spare = new()
                {
                    Name = Name,
                    Description = Description,
                    CategoryId = CategoryId,
                    Price = Price,
                    ActualQuantity = ActualQuantity,
                    AvailableQuantity = AvailableQuantity,
                };
                SpareRepository.Add(spare);
                await SpareRepository.FlushAsync();
                ChangeParentState.Invoke();

                Snackbar.Add($"Spare {Name} is Added!", Severity.Success);
                MudDialog.Close();
            }
            catch (Exception ex)
            {
				_ = new Exception($"{ex}CategoryId is null");
            }
        }
        else
        {
            _error = true;
        }
    }
}
