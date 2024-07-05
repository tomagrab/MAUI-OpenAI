using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUI_OpenAI.Services
{
    public class ComboBoxService : IComboBoxService
    {
        public List<string> FilterItems(string searchText, List<string> items)
        {
            return string.IsNullOrWhiteSpace(searchText)
                ? items
                : items.Where(item => item.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<bool> IsClickOutsideAsync(double clientX, double clientY, BoundingClientRect rect, IJSRuntime js, ElementReference comboBoxRef)
        {
            rect = await js.InvokeAsync<BoundingClientRect>("getBoundingClientRect", comboBoxRef);
            return clientX < rect.Left || clientX > rect.Right || clientY < rect.Top || clientY > rect.Bottom;
        }
    }
}
