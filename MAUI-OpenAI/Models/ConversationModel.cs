using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Services;

namespace MAUI_OpenAI.Models
{
    public class ConversationModel
    {
        public const int MaxTokens = 80000;
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<ChatMessageModel> Messages { get; set; } = new List<ChatMessageModel>();
        public IBaseService BaseService { get; set; } = Application.Current!.MainPage!.Handler!.MauiContext!.Services!.GetService<IBaseService>()!;
        public ITokenizerService TokenizerService { get; set; } = Application.Current!.MainPage!.Handler!.MauiContext!.Services!.GetService<ITokenizerService>()!;
        public ConversationModel(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
        }

        public void AddMessage(ChatMessageModel message, EventCallback<string> onError)
        {
            try
            {
                message.ConversationId = Id;
                Messages.Add(message);
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error adding message: {ex.Message}");
            }
        }

        public void AddUserMessage(string message, EventCallback<string> onError)
        {
            try
            {
                var userChatMessage = new ChatMessageModel(message, "user");
                AddMessage(userChatMessage, onError);
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error adding user message: {ex.Message}");
            }
        }

        public void AddForgetPreviousRoleMessage(string currentRole, EventCallback<string> onError)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentRole))
                {
                    var forgetMessage = new ChatMessageModel("Forget the previous role.", "user") { DisplayInUI = false };
                    AddMessage(forgetMessage, onError);
                }
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error adding forget previous role message: {ex.Message}");
            }
        }

        public void AddRoleMessage(string roleName, EventCallback<string> onError)
        {
            try
            {
                var role = RolePromptModel.RolePrompts().Find(r => r.RoleName == roleName);
                if (role != null)
                {
                    var roleMessage = new ChatMessageModel(role.Description, "system") { DisplayInUI = false };
                    AddMessage(roleMessage, onError);
                }
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error adding role message: {ex.Message}");
            }
        }

        public void ClearMessages(EventCallback<string> onError)
        {
            try
            {
                Messages.Clear();
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error clearing conversation: {ex.Message}");
            }
        }

        public List<ChatMessageModel> GetTextMessages()
        {
            return Messages.Where(m => !m.IsImage).ToList();
        }

        public List<ChatMessageModel> GetTrimmedTextMessages(EventCallback<string> onError)
        {
            try
            {
                var textMessages = GetTextMessages();
                if (TokenizerService != null)
                {
                    return TokenizerService.TrimConversationToTokenLimit(textMessages, MaxTokens, onError);
                }
                else
                {
                    onError.InvokeAsync("Tokenizer service is null.");
                    return new List<ChatMessageModel>();
                }
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error trimming conversation: {ex.Message}");
                return new List<ChatMessageModel>();
            }
        }

        public async Task HandleSendMessageAsync(string message, bool isImageGenerationMode, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, Func<Task> onResponseComplete, IOpenAIService openAIService)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            await onStateChange();
            AddUserMessage(message, onError);

            try
            {
                if (isImageGenerationMode)
                {
                    await GenerateImageResponseAsync(message, onError, onStateChange, onImageGenerated, openAIService);
                }
                else
                {
                    var trimmedMessages = GetTrimmedTextMessages(onError);
                    await GenerateChatResponseAsync(trimmedMessages, onError, onStateChange, onResponseComplete, openAIService);
                }
            }
            catch (Exception ex)
            {
                await BaseService.HandleErrorAsync($"Error sending message: {ex.Message}", onError);
            }
            finally
            {
                await onStateChange();
            }
        }

        private async Task GenerateImageResponseAsync(string message, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, IOpenAIService openAIService)
        {
            var loadingMessage = AddLoadingMessage(onStateChange);

            await openAIService.GenerateImageAsync(message, EventCallback.Factory.Create(this, async (byte[] imageBytes) =>
            {
                try
                {
                    RemoveLoadingMessage(loadingMessage, onStateChange);
                    await UpdateImageResponseAsync(imageBytes, onStateChange, onImageGenerated, onError);
                }
                catch (Exception ex)
                {
                    RemoveLoadingMessage(loadingMessage, onStateChange);
                    await BaseService.HandleErrorAsync($"Error updating image response: {ex.Message}", onError);
                }
            }), EventCallback.Factory.Create<string>(this, async (error) =>
            {
                RemoveLoadingMessage(loadingMessage, onStateChange);
                await BaseService.HandleErrorAsync(error, onError);
            }));
        }

        private async Task GenerateChatResponseAsync(List<ChatMessageModel> conversation, EventCallback<string> onError, Func<Task> onStateChange, Func<Task> onResponseComplete, IOpenAIService openAIService)
        {
            var assistantMessage = new ChatMessageModel("", "assistant");
            bool isFirstUpdateReceived = false;

            await openAIService.GetChatCompletionStreamingAsync(this, EventCallback.Factory.Create(this, async (string update) =>
            {
                try
                {
                    if (!isFirstUpdateReceived)
                    {
                        AddMessage(assistantMessage, onError);
                        isFirstUpdateReceived = true;
                    }
                    UpdateResponse(update, assistantMessage, onStateChange, onError);
                    await onStateChange();
                }
                catch (Exception ex)
                {
                    await BaseService.HandleErrorAsync($"Error updating chat response: {ex.Message}", onError);
                }
            }), EventCallback.Factory.Create(this, async () =>
            {
                await CompleteResponseAsync(assistantMessage, onStateChange, onError);
                await onResponseComplete();
            }), onError, onResponseComplete);
        }

        private void UpdateResponse(string update, ChatMessageModel assistantMessage, Func<Task> onStateChange, EventCallback<string> onError)
        {
            try
            {
                assistantMessage.Message += update;
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error updating response: {ex.Message}");
            }
        }

        private async Task CompleteResponseAsync(ChatMessageModel chatMessage, Func<Task> onStateChange, EventCallback<string> onError)
        {
            try
            {
                await chatMessage.ConvertToHtmlAsync(onError);
                await onStateChange();
            }
            catch (Exception ex)
            {
                await onError.InvokeAsync($"Error completing response: {ex.Message}");
            }
        }

        private async Task UpdateImageResponseAsync(byte[] imageBytes, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
        {
            try
            {
                var base64Image = Convert.ToBase64String(imageBytes);
                var imageMessage = new ChatMessageModel(base64Image, "assistant", isImage: true);
                AddMessage(imageMessage, onError);
                await onImageGenerated.InvokeAsync(imageBytes);
                await onStateChange();
            }
            catch (Exception ex)
            {
                await BaseService.HandleErrorAsync($"Error updating image response: {ex.Message}", onError);
            }
        }

        private ChatMessageModel AddLoadingMessage(Func<Task> onStateChange)
        {
            var loadingMessage = new ChatMessageModel("", "assistant", true, true);
            AddMessage(loadingMessage, EventCallback<string>.Empty);
            onStateChange();
            return loadingMessage;
        }

        private void RemoveLoadingMessage(ChatMessageModel loadingMessage, Func<Task> onStateChange)
        {
            Messages.Remove(loadingMessage);
            onStateChange();
        }
    }
}