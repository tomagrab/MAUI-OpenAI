using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IChatService
    {
        Task HandleSendMessageAsync(string message, bool isImageGenerationMode,EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, Func<Task> onResponseComplete);
        void AddForgetPreviousRoleMessage(string currentRole, EventCallback<string> onError);
        void AddRoleMessage(string roleName, EventCallback<string> onError);
    }
}
