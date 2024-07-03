using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public class TokenizerService : ITokenizerService
    {
        private const double CharactersPerToken = 3.5;

        public int EstimateTokenCount(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return 0;
                }

                return (int)Math.Ceiling(text.Length / CharactersPerToken);
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while estimating the token count for the provided text.");
            }
        }

        public int EstimateTokenCount(IEnumerable<string> texts)
        {
            try
            {
                return texts.Sum(text => EstimateTokenCount(text));
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while estimating the token count for multiple texts.");
            }
        }

        public List<ChatMessageModel> TrimConversationToTokenLimit(List<ChatMessageModel> conversation, int maxTokens)
        {
            try
            {
                var estimatedTokens = EstimateTokenCount(conversation.Select(c => c.Message));
                while (estimatedTokens > maxTokens && conversation.Count > 0)
                {
                    conversation.RemoveAt(0);
                    estimatedTokens = EstimateTokenCount(conversation.Select(c => c.Message));
                }

                return conversation;
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while trimming the conversation to the token limit.");
            }
        }
    }
}
