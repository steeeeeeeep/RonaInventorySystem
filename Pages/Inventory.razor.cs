﻿using BikeSparesInventorySystem.Data.Enums;
using BikeSparesInventorySystem.Data.Models;
using BikeSparesInventorySystem.Shared;
using MudBlazor;

namespace BikeSparesInventorySystem.Pages
{
    public partial class Inventory
    {
        private bool showDescription = false;
        private bool dense = true;
        private bool fixed_header = true;
        private bool fixed_footer = true;
        private bool hover = true;
        private bool ronly = false;
        private bool canCancelEdit = true;
        private bool blockSwitch = true;
        private string searchString = "";
        private Spare selectedItem1 = null;
        private Spare elementBeforeEdit;
        private TableApplyButtonPosition applyButtonPosition = TableApplyButtonPosition.End;
        private TableEditButtonPosition editButtonPosition = TableEditButtonPosition.End;
        private TableEditTrigger editTrigger = TableEditTrigger.RowClick;
        private IEnumerable<Spare> Elements = new List<Spare>();       

        protected override void OnInitialized()
        {
            Elements = SpareRepository.GetAll();
            MainLayout.Title = "Manage Bike Spares";
        }

        private void BackupItem(object element)
        {
            elementBeforeEdit = ((Spare)element).Clone() as Spare;
        }

        private void ResetItemToOriginalValues(object element)
        {
            ((Spare)element).Name = elementBeforeEdit.Name;
            ((Spare)element).Description = elementBeforeEdit.Description;
            ((Spare)element).Company = elementBeforeEdit.Company;
            ((Spare)element).Price = elementBeforeEdit.Price;
            ((Spare)element).AvailableQuantity = elementBeforeEdit.AvailableQuantity;
        }

        private bool FilterFunc(Spare element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Company.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Price.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.AvailableQuantity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private void ShowBtnPress() => showDescription = !showDescription;
    }
}