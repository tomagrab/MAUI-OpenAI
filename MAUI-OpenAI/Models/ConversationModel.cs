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
    }
}