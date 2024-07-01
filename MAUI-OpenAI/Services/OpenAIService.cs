using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using MAUI_OpenAI.Models;
using System.ClientModel;

namespace MAUI_OpenAI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<OpenAIService> _logger;
        private const int MaxTokens = 4096; // Set the token limit according to your model

        public OpenAIService(ChatClient chatClient, ILogger<OpenAIService> logger)
        {
            _chatClient = chatClient;
            _logger = logger;
        }

        public async Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, string message, Action<string> onUpdate)
        {
            _logger.LogInformation("Sending message to OpenAI: {Message}", message);

            try
            {
                // Append the new message to the conversation
                conversation.Add(new ChatMessageModel(message, "user"));

                // Ensure the conversation stays within the token limit
                var trimmedConversation = TrimConversationToTokenLimit(conversation);

                var messages = trimmedConversation.Select(c => new UserChatMessage(c.Message)).ToArray();

                AsyncResultCollection<StreamingChatCompletionUpdate> updates = _chatClient.CompleteChatStreamingAsync(messages);

                await foreach (StreamingChatCompletionUpdate update in updates)
                {
                    foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                    {
                        onUpdate(updatePart.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing chat with OpenAI");
                onUpdate("Error completing chat with OpenAI");
            }
        }

        private List<ChatMessageModel> TrimConversationToTokenLimit(List<ChatMessageModel> conversation)
        {
            // This is a placeholder implementation. You will need to implement token counting
            // and trimming logic according to the tokenization method used by your model.
            // Here's a simplified version that keeps the last N messages.

            // Estimate the tokens and keep the last messages under the token limit
            var estimatedTokens = conversation.Sum(c => c.Message.Split(' ').Length);
            while (estimatedTokens > MaxTokens && conversation.Count > 0)
            {
                conversation.RemoveAt(0);
                estimatedTokens = conversation.Sum(c => c.Message.Split(' ').Length);
            }

            return conversation;
        }
    }
}
