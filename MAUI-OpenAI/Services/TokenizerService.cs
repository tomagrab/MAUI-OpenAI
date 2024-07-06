using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class TokenizerService : BaseService, ITokenizerService
    {
        private const double CharactersPerToken = 3.5;

        public int EstimateTokenCount(string text, EventCallback<string> onError)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return 0;
                }

                return (int)Math.Ceiling(text.Length / CharactersPerToken);
            }
            catch (Exception ex)
            {
                HandleError($"An error occurred while estimating the token count for the provided text: {ex.Message}", onError);
                return 0;
            }
        }

        public int EstimateTokenCount(IEnumerable<string> texts, EventCallback<string> onError)
        {
            try
            {
                return texts.Sum(text => EstimateTokenCount(text, onError));
            }
            catch (Exception ex)
            {
                HandleError($"An error occurred while estimating the token count for multiple texts: {ex.Message}", onError);
                return 0;
            }
        }

        public List<ChatMessageModel> TrimConversationToTokenLimit(List<ChatMessageModel> conversation, int maxTokens, EventCallback<string> onError)
        {
            try
            {
                var estimatedTokens = EstimateTokenCount(conversation.Select(c => c.Message), onError);
                while (estimatedTokens > maxTokens && conversation.Count > 0)
                {
                    conversation.RemoveAt(0);
                    estimatedTokens = EstimateTokenCount(conversation.Select(c => c.Message), onError);
                }

                return conversation;
            }
            catch (Exception ex)
            {
                HandleError($"An error occurred while trimming the conversation to the token limit: {ex.Message}", onError);
                return conversation;
            }
        }
    }
}