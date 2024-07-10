using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ChatService : BaseService, IChatService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IConversationService _conversationService;

        public ChatService(IOpenAIService openAIService, IConversationService conversationService)
        {
            _openAIService = openAIService;
            _conversationService = conversationService;
        }

        public async Task HandleSendMessageAsync(string message, List<ChatMessageModel> chatMessages, bool isImageGenerationMode, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, Func<Task> onResponseComplete)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            PrepareForMessageSend(onStateChange);
            _conversationService.AddUserMessage(message, onError);

            try
            {
                if (isImageGenerationMode)
                {
                    await GenerateImageResponseAsync(message, chatMessages, onError, onStateChange, onImageGenerated);
                }
                else
                {
                    var conversation = await _conversationService.GetTrimmedConversationAsync(onError);
                    await GenerateChatResponseAsync(conversation, chatMessages, markdownService, onError, onStateChange, onResponseComplete);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error sending message: {ex.Message}", onError);
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

        public async Task GenerateImageResponseAsync(string message, List<ChatMessageModel> chatMessages, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated)
        {
            var loadingMessage = AddLoadingMessage(chatMessages, onStateChange);

            await _openAIService.GenerateImageAsync(message, EventCallback.Factory.Create<byte[]>(this, async (imageBytes) =>
            {
                try
                {
                    RemoveLoadingMessage(loadingMessage, chatMessages, onStateChange);
                    await UpdateImageResponseAsync(imageBytes, chatMessages, onStateChange, onImageGenerated, onError);
                }
                catch (Exception ex)
                {
                    RemoveLoadingMessage(loadingMessage, chatMessages, onStateChange);
                    await HandleErrorAsync($"Error updating image response: {ex.Message}", onError);
                }
            }), EventCallback.Factory.Create<string>(this, async (error) =>
            {
                RemoveLoadingMessage(loadingMessage, chatMessages, onStateChange);
                await HandleErrorAsync(error, onError);
            }));
        }

        private ChatMessageModel AddLoadingMessage(List<ChatMessageModel> chatMessages, Func<Task> onStateChange)
        {
            var loadingMessage = new ChatMessageModel("", "assistant", true, true);
            chatMessages.Add(loadingMessage);
            onStateChange();
            return loadingMessage;
        }

        public async Task GenerateChatResponseAsync(List<ChatMessageModel> conversation, List<ChatMessageModel> chatMessages, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, Func<Task> onResponseComplete)
        {
            var assistantMessage = new ChatMessageModel("", "assistant");
            bool isFirstUpdateReceived = false;

            await _openAIService.GetChatCompletionStreamingAsync(conversation, EventCallback.Factory.Create<string>(this, async (update) =>
            {
                try
                {
                    if (!isFirstUpdateReceived)
                    {
                        chatMessages.Add(assistantMessage);
                        isFirstUpdateReceived = true;
                    }
                    UpdateResponse(update, assistantMessage, onStateChange, onError);
                    await onStateChange();
                }
                catch (Exception ex)
                {
                    await HandleErrorAsync($"Error updating chat response: {ex.Message}", onError);
                }
            }), EventCallback.Factory.Create(this, async () =>
            {
                await CompleteResponseAsync(assistantMessage, markdownService, onStateChange, onError);
                await onResponseComplete();
            }), onError, onResponseComplete);
        }

        public void FinishMessageSend(Func<Task> onStateChange)
        {
            onStateChange();
        }

        private async Task CompleteResponseAsync(ChatMessageModel chatMessage, IMarkdownService markdownService, Func<Task> onStateChange, EventCallback<string> onError)
        {
            try
            {
                chatMessage.HtmlContent = await markdownService.ConvertToHtmlAsync(chatMessage.Message, onError);
                await onStateChange();
            }
            catch (Exception ex)
            {
                HandleError($"Error completing response: {ex.Message}", onError);
            }
        }

        private void UpdateResponse(string update, ChatMessageModel assistantMessage, Func<Task> onStateChange, EventCallback<string> onError)
        {
            try
            {
                assistantMessage.Message += update;
            }
            catch (Exception ex)
            {
                HandleError($"Error updating response: {ex.Message}", onError);
            }
        }

        private async Task UpdateImageResponseAsync(byte[] imageBytes, List<ChatMessageModel> chatMessages, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
        {
            try
            {
                var base64Image = Convert.ToBase64String(imageBytes);
                var imageMessage = new ChatMessageModel(base64Image, "assistant", isImage: true);
                chatMessages.Add(imageMessage);
                await InvokeIfHasDelegateAsync(onImageGenerated, imageBytes);
                await onStateChange();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error updating image response: {ex.Message}", onError);
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

        public void AddForgetPreviousRoleMessage(string currentRole, EventCallback<string> onError)
        {
            _conversationService.AddForgetPreviousRoleMessage(currentRole, onError);
        }

        public void AddRoleMessage(string roleName, EventCallback<string> onError)
        {
            _conversationService.AddRoleMessage(roleName, onError);
        }
    }
}