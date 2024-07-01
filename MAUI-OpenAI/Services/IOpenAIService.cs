using MAUI_OpenAI.Models;

namespace MAUI_OpenAI
{
    public interface IOpenAIService
    {
        Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, string message, Action<string> onUpdate);
    }
}