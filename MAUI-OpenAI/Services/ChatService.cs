using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Models;
using MAUI_OpenAI.Data;

namespace MAUI_OpenAI.Services
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService openAIService;

        public ChatService(IOpenAIService openAIService)
        {
            this.openAIService = openAIService;
        }

        public async Task HandleSendMessageAsync(string message, List<ChatMessageModel> chatMessages, List<ChatMessageModel> conversation, bool isImageGenerationMode, IOpenAIService openAIService, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            PrepareForMessageSend(onStateChange);
            AddUserMessage(message, chatMessages, conversation);

            try
            {
                if (isImageGenerationMode)
                {
                    await GenerateImageResponseAsync(message, chatMessages, onError, onStateChange, onImageGenerated);
                }
                else
                {
                    await GenerateChatResponseAsync(conversation, chatMessages, openAIService, markdownService, onError, onStateChange);
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

        public async Task GenerateImageResponseAsync(string message, List<ChatMessageModel> chatMessages, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated)
        {
            var loadingMessage = AddLoadingMessage(chatMessages, onStateChange);

            await openAIService.GenerateImageAsync(message, async (imageBytes) =>
            {
                try
                {
                    RemoveLoadingMessage(loadingMessage, chatMessages, onStateChange);
                    await UpdateImageResponseAsync(imageBytes, chatMessages, onStateChange, onImageGenerated, onError);
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

        public async Task GenerateChatResponseAsync(List<ChatMessageModel> conversation, List<ChatMessageModel> chatMessages, IOpenAIService openAIService, IMarkdownService markdownService, EventCallback<string> onError, Func<Task> onStateChange)
        {
            var assistantMessage = new ChatMessageModel("", "assistant");
            bool isFirstUpdateReceived = false;

            await openAIService.GetChatCompletionStreamingAsync(conversation, async (update) =>
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
                    await onError.InvokeAsync($"Error updating chat response: {ex.Message}");
                }
            }, () =>
            {
                CompleteResponse(assistantMessage, markdownService, onStateChange, onError);
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

        private ChatMessageModel AddNewAssistantMessage(string update, List<ChatMessageModel> chatMessages)
        {
            var newMessage = new ChatMessageModel(update, "assistant");
            chatMessages.Add(newMessage);
            return newMessage;
        }

        private async Task UpdateImageResponseAsync(byte[] imageBytes, List<ChatMessageModel> chatMessages, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
        {
            try
            {
                var base64Image = Convert.ToBase64String(imageBytes);
                var imageMessage = new ChatMessageModel(base64Image, "assistant", isImage: true);
                chatMessages.Add(imageMessage);
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

        public void AddForgetPreviousRoleMessage(string currentRole, List<ChatMessageModel> conversation)
        {
            if (!string.IsNullOrEmpty(currentRole))
            {
                var forgetMessage = new ChatMessageModel("Forget the previous role.", "user");
                conversation.Add(forgetMessage);
            }
        }

        public void AddRoleMessage(string roleName, List<ChatMessageModel> conversation)
        {
            if (RolePrompts.Roles.TryGetValue(roleName, out var roleDescription))
            {
                var roleMessage = new ChatMessageModel(roleDescription, "system");
                conversation.Add(roleMessage);
            }
        }
    }
}