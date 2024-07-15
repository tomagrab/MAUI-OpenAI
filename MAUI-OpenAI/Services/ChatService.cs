using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public class ChatService : BaseService, IChatService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IConversationService _conversationService;
        private readonly ITokenizerService _tokenizerService;
        private ConversationModel _currentConversation;
        private const int MaxTokensPerRequest = 80000;

        public ChatService(IOpenAIService openAIService, IConversationService conversationService, ITokenizerService tokenizerService)
        {
            _openAIService = openAIService;
            _conversationService = conversationService;
            _tokenizerService = tokenizerService;
            _currentConversation = InitializeCurrentConversation() ?? new ConversationModel("Default");
        }

        private ConversationModel InitializeCurrentConversation()
        {
            var conversation = _conversationService.GetAllConversations().FirstOrDefault();
            if (conversation == null)
            {
                conversation = _conversationService.AddConversation("Default");
            }
            else
            {
                conversation.ClearMessages(EventCallback.Factory.Create<string>(this, async (error) =>
                {
                    await HandleErrorAsync(error, EventCallback<string>.Empty);
                }));
            }
            return conversation;
        }

        public async Task HandleSendMessageAsync(string message, bool isImageGenerationMode, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, Func<Task> onResponseComplete)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            PrepareForMessageSend(onStateChange);
            _currentConversation.AddUserMessage(message, onError);

            try
            {
                if (isImageGenerationMode)
                {
                    await GenerateImageResponseAsync(message, onError, onStateChange, onImageGenerated);
                }
                else
                {
                    var textConversation = _currentConversation.GetTextMessages();
                    textConversation = _tokenizerService.TrimConversationToTokenLimit(textConversation, MaxTokensPerRequest, onError);
                    await GenerateChatResponseAsync(textConversation, markdownService, onError, onStateChange, onResponseComplete);
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

        public async Task GenerateImageResponseAsync(string message, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated)
        {
            var loadingMessage = AddLoadingMessage(onStateChange);

            await _openAIService.GenerateImageAsync(message, EventCallback.Factory.Create<byte[]>(this, async (imageBytes) =>
            {
                try
                {
                    RemoveLoadingMessage(loadingMessage, onStateChange);
                    await UpdateImageResponseAsync(imageBytes, onStateChange, onImageGenerated, onError);
                }
                catch (Exception ex)
                {
                    RemoveLoadingMessage(loadingMessage, onStateChange);
                    await HandleErrorAsync($"Error updating image response: {ex.Message}", onError);
                }
            }), EventCallback.Factory.Create<string>(this, async (error) =>
            {
                RemoveLoadingMessage(loadingMessage, onStateChange);
                await HandleErrorAsync(error, onError);
            }));
        }

        private ChatMessageModel AddLoadingMessage(Func<Task> onStateChange)
        {
            var loadingMessage = new ChatMessageModel("", "assistant", true, true);
            _currentConversation.AddMessage(loadingMessage, EventCallback<string>.Empty);
            onStateChange();
            return loadingMessage;
        }

        public async Task GenerateChatResponseAsync(List<ChatMessageModel> conversation, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, Func<Task> onResponseComplete)
        {
            var assistantMessage = new ChatMessageModel("", "assistant");
            bool isFirstUpdateReceived = false;

            await _openAIService.GetChatCompletionStreamingAsync(_currentConversation, EventCallback.Factory.Create<string>(this, async (update) =>
            {
                try
                {
                    if (!isFirstUpdateReceived)
                    {
                        _currentConversation.AddMessage(assistantMessage, onError);
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
                await chatMessage.ConvertToHtmlAsync(markdownService, onError);
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

        private async Task UpdateImageResponseAsync(byte[] imageBytes, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
        {
            try
            {
                var base64Image = Convert.ToBase64String(imageBytes);
                var imageMessage = new ChatMessageModel(base64Image, "assistant", isImage: true);
                _currentConversation.AddMessage(imageMessage, onError);
                await InvokeIfHasDelegateAsync(onImageGenerated, imageBytes);
                await onStateChange();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error updating image response: {ex.Message}", onError);
            }
        }

        private void RemoveLoadingMessage(ChatMessageModel loadingMessage, Func<Task> onStateChange)
        {
            _currentConversation.Messages.Remove(loadingMessage);
            onStateChange();
        }

        public void AddForgetPreviousRoleMessage(string currentRole, EventCallback<string> onError)
        {
            _currentConversation.AddForgetPreviousRoleMessage(currentRole, onError);
        }

        public void AddRoleMessage(string roleName, EventCallback<string> onError)
        {
            _currentConversation.AddRoleMessage(roleName, onError);
        }
    }
}