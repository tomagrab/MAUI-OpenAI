using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ModalService : IModalService
    {
        public async Task SaveImageAsync(string imageSrc, IImageSaveService imageSaveService, EventCallback<string> onError, string platform)
        {
            try
            {
                if (!string.IsNullOrEmpty(imageSrc))
                {
                    var fileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await imageSaveService.SaveImageAsync(imageSrc, fileName);
                }
                else
                {
                    await HandleErrorAsync("No image to save.", onError);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Operation cancelled"))
                {
                    await HandleErrorAsync("Save operation was cancelled.", onError);
                }
                else
                {
                    await HandleErrorAsync($"Error saving image: {ex.Message}", onError);
                }
            }
        }

        public async Task CopyImageAsync(string imageSrc, IJSRuntime jsRuntime, EventCallback<string> onError)
        {
            try
            {
                if (!string.IsNullOrEmpty(imageSrc))
                {
                    await jsRuntime.InvokeVoidAsync("copyImageToClipboard", imageSrc);
                }
                else
                {
                    await HandleErrorAsync("No image to copy.", onError);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error copying image: {ex.Message}", onError);
            }
        }

        private async Task HandleErrorAsync(string message, EventCallback<string> onError)
        {
            if (onError.HasDelegate)
            {
                await onError.InvokeAsync(message);
            }
        }
    }
}