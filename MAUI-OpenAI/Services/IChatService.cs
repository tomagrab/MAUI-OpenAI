using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public interface IChatService
    {
        Task HandleSendMessageAsync(string message, List<ChatMessageModel> chatMessages, bool isImageGenerationMode, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated);

        void PrepareForMessageSend(Func<Task> onStateChange);

        Task GenerateImageResponseAsync(string message, List<ChatMessageModel> chatMessages, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated);

        Task GenerateChatResponseAsync(List<ChatMessageModel> conversation, List<ChatMessageModel> chatMessages, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange);

        void FinishMessageSend(Func<Task> onStateChange);

        void AddForgetPreviousRoleMessage(string currentRole, EventCallback<string> onError);

        void AddRoleMessage(string roleName, EventCallback<string> onError);
    }
}
