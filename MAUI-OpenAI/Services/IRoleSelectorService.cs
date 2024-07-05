using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IRoleSelectorService
    {
        Task HandleRoleChangeAsync(string value, EventCallback<string> onRoleSelected);
        Task HandleImageGeneratedAsync(byte[] imageBytes, EventCallback<byte[]> onImageGenerated);
        Task HandleErrorAsync(string error, EventCallback<string> onError);
        Task HandleLoadingAsync(bool loading, EventCallback<bool> onLoading);
    }
}