using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeSparesInventorySystem.Pages.MinersPage;

public partial class MinersDetail
{
    private readonly bool Dense = true;
    //private readonly bool Fixed_header = true;
    //private readonly bool Fixed_footer = true;
    private readonly bool Hover = true;
    private bool ReadOnly = false;
    private readonly bool CanCancelEdit = true;
    private readonly bool BlockSwitch = true;
    private Miners SelectedItem;
    private Miners ElementBeforeEdit;
    private readonly TableApplyButtonPosition ApplyButtonPosition = TableApplyButtonPosition.End;
    private readonly TableEditButtonPosition EditButtonPosition = TableEditButtonPosition.End;
    private readonly TableEditTrigger EditTrigger = TableEditTrigger.EditButton;
    //private string SearchString;
    private IEnumerable<Miners> Miners;
    //private IEnumerable<Spare> Products;
    private readonly Dictionary<Guid, bool> OrderDescTrack = new();

    //[CascadingParameter] private Action<string> SetAppBarTitle { get; set; }
    [Parameter] public Miners MinersDetails { get; set; }

    protected sealed override async Task OnParametersSetAsync()
    {
        await Task.Delay(1000);
        Miners = MinersRepository.GetAll().Where(n => n.Name == MinersDetails.Name && n.CreatedAt == MinersDetails.CreatedAt);
    }

    //private async Task HandleInternalInputChange(ChangeEventArgs args)
    //{
    //    // Extract the current input value
    //    string inputValue = args?.Value?.ToString();

    //    // Perform the search in uniqueMiners based on the input value
    //    var result = MinersRepository.Get(x => x.Name, args.Value).Name;

    //    // If no match is found, add the input value to the result
    //    if (!result.Contains(inputValue))
    //    {
    //        result = inputValue;
    //    }

    //    // Invoke the event callback with the search result
    //    //await titleField.OnInternalInputChanged.InvokeAsync(new ChangeEventArgs { Value = result });
    //} 
    
    protected async Task Update()
    {
        await MinersRepository.FlushAsync();
        Snackbar.Add("Successfuly Updated!", Severity.Success);
    }


    private void BackupItem(object element)
    {
        ElementBeforeEdit = ((Miners)element).Clone() as Miners;
    }

    private void ResetItemToOriginalValues(object element)
    {
        ((Miners)element).Name = ElementBeforeEdit.Name;
        ((Miners)element).Code = ElementBeforeEdit.Code;
        ((Miners)element).Price = ElementBeforeEdit.Price;
    }
}
