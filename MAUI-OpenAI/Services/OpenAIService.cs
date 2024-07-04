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
        private readonly ITokenizerService _tokenizerService;
        private const int MaxTokens = 80000;

        public OpenAIService(ChatClient chatClient, ImageClient imageClient, ITokenizerService tokenizerService)
        {
            _chatClient = chatClient;
            _imageClient = imageClient;
            _tokenizerService = tokenizerService;
        }

        public async Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, string message, Action<string> onUpdate, Action onComplete)
        {
            try
            {
                conversation.Add(new ChatMessageModel(message, "user"));

                var textConversation = conversation.Where(c => !c.IsImage).ToList();

                List<ChatMessageModel> trimmedConversation;
                try
                {
                    trimmedConversation = _tokenizerService.TrimConversationToTokenLimit(textConversation, MaxTokens);
                }
                catch (Exception tex)
                {
                    onUpdate($"An error occurred while trimming the conversation: {tex.Message}");
                    return;
                }

                var messages = trimmedConversation.Select(c => new UserChatMessage(c.Message)).ToArray();

                AsyncResultCollection<StreamingChatCompletionUpdate> updates = _chatClient.CompleteChatStreamingAsync(messages);

                await foreach (StreamingChatCompletionUpdate update in updates)
                {
                    foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                    {
                        onUpdate(updatePart.Text);
                    }
                }

                onComplete();
            }
            catch (ClientResultException cre)
            {
                onUpdate($"An error occurred while processing the request: {cre.Message}");
            }
            catch (TimeoutException te)
            {
                onUpdate("Request timed out: " + te.Message);
            }
            catch (Exception ex)
            {
                onUpdate($"An unexpected error occurred while processing the request: {ex.Message}");
            }
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
            catch (ClientResultException cre)
            {
                onError?.Invoke($"An error occurred while generating the image: {cre.Message}");
            }
            catch (TimeoutException te)
            {
                onError?.Invoke("Image generation request timed out: " + te.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke($"An unexpected error occurred while generating the image: {ex.Message}");
            }
        }
    }
}
