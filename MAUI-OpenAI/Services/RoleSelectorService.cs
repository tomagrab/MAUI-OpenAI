using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class RoleSelectorService : IRoleSelectorService
    {
        public async Task HandleRoleChangeAsync(string value, EventCallback<string> onRoleSelected)
        {
            if (onRoleSelected.HasDelegate)
            {
                await onRoleSelected.InvokeAsync(value);
            }
        }

        public async Task HandleImageGeneratedAsync(byte[] imageBytes, EventCallback<byte[]> onImageGenerated)
        {
            if (onImageGenerated.HasDelegate)
            {
                await onImageGenerated.InvokeAsync(imageBytes);
            }
        }

        public async Task HandleErrorAsync(string error, EventCallback<string> onError)
        {
            if (onError.HasDelegate)
            {
                await onError.InvokeAsync(error);
            }
        }

        public async Task HandleLoadingAsync(bool loading, EventCallback<bool> onLoading)
        {
            if (onLoading.HasDelegate)
            {
                await onLoading.InvokeAsync(loading);
            }
        }
    }
}