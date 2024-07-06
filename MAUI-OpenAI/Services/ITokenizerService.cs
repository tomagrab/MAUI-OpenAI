using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface ITokenizerService
    {
        int EstimateTokenCount(string text, EventCallback<string> onError);
        int EstimateTokenCount(IEnumerable<string> texts, EventCallback<string> onError);
        List<ChatMessageModel> TrimConversationToTokenLimit(List<ChatMessageModel> conversation, int maxTokens, EventCallback<string> onError);
    }
}