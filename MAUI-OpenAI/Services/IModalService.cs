using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUI_OpenAI.Services
{
    public interface IModalService
    {
        Task<string> SaveImageAsync(string imageSrc, IImageSaveService imageSaveService, string platform);
        Task CopyImageAsync(string imageSrc, IJSRuntime jsRuntime, EventCallback<string> onError);
    }
}