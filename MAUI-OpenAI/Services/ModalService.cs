using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ModalService : BaseService, IModalService
    {
        public async Task<string> SaveImageAsync(string imageSrc, IImageSaveService imageSaveService, string platform)
        {
            try
            {
                if (!string.IsNullOrEmpty(imageSrc))
                {
                    var fileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await imageSaveService.SaveImageAsync(imageSrc, fileName);
                    return "Saved";
                }
                else
                {
                    return "No image to save.";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Operation cancelled"))
                {
                    return "Cancelled";
                }
                else
                {
                    return $"Error saving image: {ex.Message}";
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
    }
}