using OpenAI.Chat;
using System.ClientModel;
using OpenAI.Images;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class OpenAIService : BaseService, IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly ImageClient _imageClient;
        private readonly IConversationService _conversationService;

        public OpenAIService(ChatClient chatClient, ImageClient imageClient, IConversationService conversationService)
        {
            _chatClient = chatClient;
            _imageClient = imageClient;
            _conversationService = conversationService;
        }

        public async Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, EventCallback<string> onUpdate, EventCallback onComplete, EventCallback<string> onError)
        {
            try
            {
                var messages = conversation.Select(c => new UserChatMessage(c.Message)).ToArray();

                AsyncResultCollection<StreamingChatCompletionUpdate> updates = _chatClient.CompleteChatStreamingAsync(messages);

                await foreach (StreamingChatCompletionUpdate update in updates)
                {
                    foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                    {
                        await onUpdate.InvokeAsync(updatePart.Text);
                    }
                }

                await onComplete.InvokeAsync();
            }
            catch (ClientResultException cre)
            {
                await HandleErrorAsync($"An error occurred while processing the request: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                await HandleErrorAsync("Request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"An unexpected error occurred while processing the request: {ex.Message}", onError);
            }
        }

        public async Task GenerateImageAsync(string prompt, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
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
                await onImageGenerated.InvokeAsync(image.ImageBytes.ToArray());
            }
            catch (ClientResultException cre)
            {
                await HandleErrorAsync($"An error occurred while generating the image: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                await HandleErrorAsync("Image generation request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"An unexpected error occurred while generating the image: {ex.Message}", onError);
            }
        }
    }
}