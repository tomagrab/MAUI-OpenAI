using OpenAI.Chat;
using MAUI_OpenAI.Models;
using System.ClientModel;
using OpenAI.Images;

namespace MAUI_OpenAI.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly ImageClient _imageClient;
        private const int MaxTokens = 4096; // Set the token limit according to your model

        public OpenAIService(ChatClient chatClient, ImageClient imageClient)
        {
            _chatClient = chatClient;
            _imageClient = imageClient;
        }

        public async Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, string message, Action<string> onUpdate)
        {
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
                // Handle exceptions
                onUpdate("An error occurred while processing the request: " + ex.Message);
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

        public async Task GenerateImageAsync(string prompt, Action<byte[]> onImageGenerated, Action<string> onError)
        {
            try
            {
                var options = new ImageGenerationOptions
                {
                    Quality = GeneratedImageQuality.High,
                    Size = GeneratedImageSize.W1792xH1024,
                    Style = GeneratedImageStyle.Vivid,
                    ResponseFormat = GeneratedImageFormat.Bytes
                };

                GeneratedImage image = await _imageClient.GenerateImageAsync(prompt, options);
                onImageGenerated?.Invoke(image.ImageBytes.ToArray());
            }
            catch (Exception ex)
            {
                // Handle exceptions
                onError?.Invoke("An error occurred while generating the image: " + ex.Message);
            }
        }
    }
}
