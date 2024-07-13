using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUI_OpenAI.Services
{
    public class ComboBoxService : BaseService, IComboBoxService
    {
        public List<string> FilterItems(string searchText, List<string> items)
        {
            try
            {
                return string.IsNullOrWhiteSpace(searchText)
                    ? items
                    : items.Where(item => item.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception ex)
            {
                HandleError($"Error filtering items: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
                return new List<string>();
            }
        }

        public async Task<bool> IsClickOutsideAsync(double clientX, double clientY, BoundingClientRectModel rect, IJSRuntime js, ElementReference comboBoxRef, EventCallback<string> onError)
        {
            try
            {
                if (comboBoxRef.Context == null)
                {
                    return true;
                }

                rect = await js.InvokeAsync<BoundingClientRectModel>("getBoundingClientRect", comboBoxRef);

                if (rect == null)
                {
                    return true;
                }

                return clientX < rect.Left || clientX > rect.Right || clientY < rect.Top || clientY > rect.Bottom;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error checking if click is outside: {ex.Message}", onError);
                return false;
            }
        }
    }
}