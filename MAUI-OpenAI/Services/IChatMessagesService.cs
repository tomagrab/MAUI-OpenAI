using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IChatMessagesService
    {
        string GetMessageCssClass(string role, EventCallback<string> onError);
        Task HandleImageClickAsync(string imageSrc, EventCallback<string> onImageClick, EventCallback<string> onError);
        void HandleError(string message, EventCallback<string> onError);
    }
}