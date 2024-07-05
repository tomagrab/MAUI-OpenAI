using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUI_OpenAI.Services
{
    public interface IModalService
    {
        Task SaveImageAsync(string imageSrc, IImageSaveService imageSaveService, EventCallback<string> onError, string platform);
        Task CopyImageAsync(string imageSrc, IJSRuntime jsRuntime, EventCallback<string> onError);
    }
}