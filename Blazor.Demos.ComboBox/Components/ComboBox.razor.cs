using Microsoft.AspNetCore.Components;

namespace Blazor.Demos.ComboBox.Components;
public partial class ComboBox<TItem>
{
    [Parameter] public IEnumerable<TItem> Items { get; set; }
    [Parameter] public Func<Task<IEnumerable<TItem>>> DataSource { get; set; }
    [Parameter] public Func<TItem, string> DisplayProperty { get; set; }
    [Parameter] public EventCallback<TItem> OnItemSelected { get; set; }

    private IEnumerable<TItem> _items;
    private List<TItem> FilteredItems { get; set; } = new();
    private string SearchText { get; set; }
    private bool IsOpen { get; set; }
    private bool _isItemSelected;
    private bool _isInitialized;

    protected override async Task OnInitializedAsync()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            await LoadItemAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isInitialized && DataSource != null)
        {
            await LoadItemAsync();
        }
        else if (_isInitialized && Items != _items)
        {
            _items = Items;
            FilteredItems = _items?.ToList() ?? [];
        }
    }

    private async Task LoadItemAsync()
    {
        if (DataSource != null)
        {
            _items = await DataSource();
        }
        else
        {
            _items = Items;
        }
        FilteredItems = _items?.ToList() ?? [];
    }

    private void FilterItems(ChangeEventArgs e = null)
    {
        SearchText = e?.Value?.ToString() ?? SearchText;
        FilteredItems = string.IsNullOrEmpty(SearchText)
            ? _items.ToList()
            : _items.Where(item => GetDisplayValue(item).Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private string GetDisplayValue(TItem item)
    {
        return DisplayProperty?.Invoke(item) ?? item.ToString();
    }

    private void SelectedItem(TItem item)
    {
        SearchText = GetDisplayValue(item);
        _isItemSelected = true;
        OnItemSelected.InvokeAsync(item);
        FilterItems();
        CloseDropdown();
    }

    private void CloseDropdown()
    {
        IsOpen = false;
    }

    private void HandleFocusOut()
    {
        if (ShouldSelectedSingleItem())
        {
            SelectedItem(FilteredItems.First());
        }
        else if (ShouldSelecteExactMatch())
        {
            SelectExtactMatch();
        }
        ResetItemSelection();
        CloseDropdown();

    }

    private void SelectExtactMatch()
    {
        TItem match = FilteredItems.FirstOrDefault(item=> GetDisplayValue(item).Equals(SearchText, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            SelectedItem(match);
        }
    }

    private bool ShouldSelectedSingleItem()
    {
        return !_isItemSelected && FilteredItems.Count == 1;
    }

    private bool ShouldSelecteExactMatch()
    {
        return !_isItemSelected && FilteredItems.Count > 1;
    }

    private void ResetItemSelection()
    {
        _isItemSelected = false;
    }

    private void OpenDropdown()
    {
        IsOpen = true;
    }
}