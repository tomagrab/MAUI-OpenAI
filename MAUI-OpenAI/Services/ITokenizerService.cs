using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public interface ITokenizerService
    {
        int EstimateTokenCount(string text);
        int EstimateTokenCount(IEnumerable<string> texts);
        List<ChatMessageModel> TrimConversationToTokenLimit(List<ChatMessageModel> conversation, int maxTokens);
    }
}
