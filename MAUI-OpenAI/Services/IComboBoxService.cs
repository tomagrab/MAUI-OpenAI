using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUI_OpenAI.Services
{
    public interface IComboBoxService
    {
        List<string> FilterItems(string searchText, List<string> items);
        Task<bool> IsClickOutsideAsync(double clientX, double clientY, BoundingClientRectModel rect, IJSRuntime js, ElementReference comboBoxRef, EventCallback<string> onError);
    }
}