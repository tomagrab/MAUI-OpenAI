using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public class ChatService : IChatService
    {
        public async Task HandleSendMessageAsync(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation, bool isImageGenerationMode, IOpenAIService openAIService, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            PrepareForMessageSend(onStateChange);
            AddUserMessage(message, chatMessages, conversation);

            try
            {
                if (isImageGenerationMode)
                {
                    await GenerateImageResponseAsync(message, chatMessages, conversation, openAIService, onError, onStateChange, onImageGenerated);
                }
                else
                {
                    await GenerateChatResponseAsync(message, conversation, chatMessages, openAIService, markdownService, onError, onStateChange);
                }
            }
            catch (Exception ex)
            {
                await onError.InvokeAsync($"Error sending message: {ex.Message}");
            }
            finally
            {
                FinishMessageSend(onStateChange);
            }
        }

        public void PrepareForMessageSend(Func<Task> onStateChange)
        {
            onStateChange();
        }

        public void AddUserMessage(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation)
        {
            var userChatMessage = new ChatMessageModel(message, "user");
            chatMessages.Add(userChatMessage);
            conversation.Add(userChatMessage);
        }

        public async Task GenerateImageResponseAsync(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation, IOpenAIService openAIService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated)
        {
            var loadingMessage = AddLoadingMessage(chatMessages, onStateChange);

            await openAIService.GenerateImageAsync(message, async (imageBytes) =>
            {
                try
                {
                    await UpdateImageResponseAsync(imageBytes, loadingMessage, conversation, onStateChange, onImageGenerated, onError);
                }
                catch (Exception ex)
                {
                    RemoveLoadingMessage(loadingMessage, chatMessages, onStateChange);
                    await onError.InvokeAsync($"Error updating image response: {ex.Message}");
                }
            }, async (error) =>
            {
                RemoveLoadingMessage(loadingMessage, chatMessages, onStateChange);
                await onError.InvokeAsync(error);
            });
        }

        private ChatMessageModel AddLoadingMessage(List<ChatMessageModel> chatMessages, Func<Task> onStateChange)
        {
            var loadingMessage = new ChatMessageModel("", "assistant", true, true);
            chatMessages.Add(loadingMessage);
            onStateChange();
            return loadingMessage;
        }

        public async Task GenerateChatResponseAsync(string message, List<ChatMessageModel> conversation, List<ChatMessageModel> chatMessages, IOpenAIService openAIService, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange)
        {
            await openAIService.GetChatCompletionStreamingAsync(conversation.Where(c => !c.IsImage).ToList(), message, async (update) =>
            {
                try
                {
                    UpdateResponse(update, chatMessages, onStateChange, onError);
                    await onStateChange();
                }
                catch (Exception ex)
                {
                    await onError.InvokeAsync($"Error updating chat response: {ex.Message}");
                }
            }, () =>
            {
                CompleteResponse(chatMessages.Last(), markdownService, onStateChange, onError);
            });
        }

        public void FinishMessageSend(Func<Task> onStateChange)
        {
            onStateChange();
        }

        private void CompleteResponse(ChatMessageModel chatMessage, IMarkdownService markdownService, Func<Task> onStateChange, EventCallback<string> onError)
        {
            try
            {
                chatMessage.HtmlContent = markdownService.ConvertToHtml(chatMessage.Message);
                onStateChange();
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error completing response: {ex.Message}");
            }
        }

        private void UpdateResponse(string update, List<ChatMessageModel> chatMessages, Func<Task> onStateChange, EventCallback<string> onError)
        {
            try
            {
                if (chatMessages.LastOrDefault()?.Role == "assistant")
                {
                    chatMessages.Last().Message += update;
                }
                else
                {
                    AddNewAssistantMessage(update, chatMessages);
                }
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error updating response: {ex.Message}");
            }
        }

        private void AddNewAssistantMessage(string update, List<ChatMessageModel> chatMessages)
        {
            var newMessage = new ChatMessageModel(update, "assistant");
            chatMessages.Add(newMessage);
        }

        private async Task UpdateImageResponseAsync(byte[] imageBytes, ChatMessageModel loadingMessage, List<ChatMessageModel> conversation, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
        {
            try
            {
                var base64Image = Convert.ToBase64String(imageBytes);
                loadingMessage.Message = base64Image;
                loadingMessage.IsImageLoading = false;
                conversation.Add(loadingMessage);
                await onImageGenerated.InvokeAsync(imageBytes);
                onStateChange();
            }
            catch (Exception ex)
            {
                await onError.InvokeAsync($"Error updating image response: {ex.Message}");
            }
        }

        private void RemoveLoadingMessage(ChatMessageModel loadingMessage, List<ChatMessageModel> chatMessages, Func<Task> onStateChange)
        {
            if (chatMessages.Contains(loadingMessage))
            {
                chatMessages.Remove(loadingMessage);
            }
            onStateChange();
        }
    }
}