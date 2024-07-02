using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public interface IOpenAIService
    {
        Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, string message, Action<string> onUpdate);
    }
}