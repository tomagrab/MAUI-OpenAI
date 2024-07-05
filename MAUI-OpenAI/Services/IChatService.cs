using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public interface IChatService
    {
        Task HandleSendMessageAsync(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation, bool isImageGenerationMode, IOpenAIService openAIService, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated);
        void PrepareForMessageSend(Func<Task> onStateChange);
        void AddUserMessage(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation);
        Task GenerateImageResponseAsync(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation, IOpenAIService openAIService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated);
        Task GenerateChatResponseAsync(string message, List<ChatMessageModel> conversation, List<ChatMessageModel> chatMessages, IOpenAIService openAIService, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange);
        void FinishMessageSend(Func<Task> onStateChange);
    }
}