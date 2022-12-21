﻿using BikeSparesInventorySystem.Data.Models;
using BikeSparesInventorySystem.Data.Providers;
using BikeSparesInventorySystem.Data.Utils;
using BikeSparesInventorySystem.Shared;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BikeSparesInventorySystem.Pages;

public partial class Users
{       
    private bool dense = true;
    private bool fixed_header = true;
    private bool fixed_footer = true;
    private bool hover = true;
    private bool ronly = false;
    private bool canCancelEdit = true;
    private bool blockSwitch = true;
    private string searchString = "";
    private User selectedItem1 = null;
    private User elementBeforeEdit;
    private TableApplyButtonPosition applyButtonPosition = TableApplyButtonPosition.End;
    private TableEditButtonPosition editButtonPosition = TableEditButtonPosition.End;
    private TableEditTrigger editTrigger = TableEditTrigger.RowClick;
    private IEnumerable<User> Elements = new List<User>();    

    protected override void OnInitialized()
    {
        Elements = UserRepository.GetAll();
        MainLayout.Title = "Manage Users";
    }

    private void BackupItem(object element)
    {
        elementBeforeEdit = ((User)element).Clone() as User;            
    }

    private string getName(Guid id) => id.Equals(Guid.Empty) ? "N/A" : UserRepository.Get(x => x.Id, id).UserName;        

    private void ResetItemToOriginalValues(object element)
    {            
        ((User)element).UserName = elementBeforeEdit.UserName;
        ((User)element).Email = elementBeforeEdit.Email;
        ((User)element).FullName = elementBeforeEdit.FullName;
        ((User)element).Role = elementBeforeEdit.Role;           
    }

    private bool FilterFunc(User element)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.HasInitialPassword.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedAt.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;            
        return false;
    }

    private async Task AddDialog()
    {
        await DialogService.ShowAsync<Shared.Dialogs.AddUserDialog>("Add User");
    }

    private async Task UploadFile(IBrowserFile file)
    {
        try
        {
            var provider = Explorer.GetFileProvider<User>(file.Name);
            await UserRepository.LoadAsync(provider, file.OpenReadStream(Explorer.MAX_ALLOWED_IMPORT_SIZE));
            Snackbar.Add($"Imported {file.Name} File!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        //TODO upload the files to the server
    }
}