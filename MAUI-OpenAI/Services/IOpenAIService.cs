using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IOpenAIService
    {
        Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, EventCallback<string> onUpdate, EventCallback onComplete, EventCallback<string> onError);
        Task GenerateImageAsync(string prompt, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError);
    }
}