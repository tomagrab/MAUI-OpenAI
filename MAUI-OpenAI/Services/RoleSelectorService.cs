using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class RoleSelectorService : BaseService, IRoleSelectorService
    {
        public async Task HandleRoleChangeAsync(string value, EventCallback<string> onRoleSelected)
        {
            await InvokeIfHasDelegateAsync(onRoleSelected, value);
        }

        public async Task HandleImageGeneratedAsync(byte[] imageBytes, EventCallback<byte[]> onImageGenerated)
        {
            await InvokeIfHasDelegateAsync(onImageGenerated, imageBytes);
        }

        public new async Task HandleErrorAsync(string error, EventCallback<string> onError)
        {
            await base.HandleErrorAsync(error, onError);
        }

        public async Task HandleLoadingAsync(bool loading, EventCallback<bool> onLoading)
        {
            await InvokeIfHasDelegateAsync(onLoading, loading);
        }
    }
}