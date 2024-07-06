using MAUI_OpenAI.Data;
using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ConversationService : BaseService, IConversationService
    {
        private List<ChatMessageModel> conversation = new List<ChatMessageModel>();
        private readonly ITokenizerService _tokenizerService;
        private const int MaxTokens = 80000;

        public ConversationService(ITokenizerService tokenizerService)
        {
            _tokenizerService = tokenizerService;
        }

        public List<ChatMessageModel> GetConversation()
        {
            return conversation;
        }

        public List<ChatMessageModel> GetTextConversation()
        {
            return conversation.Where(c => !c.IsImage).ToList();
        }

        public async Task<List<ChatMessageModel>> GetTrimmedConversationAsync(EventCallback<string> onError)
        {
            try
            {
                var textConversation = GetTextConversation();
                return _tokenizerService.TrimConversationToTokenLimit(textConversation, MaxTokens, onError);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"An error occurred while trimming the conversation: {ex.Message}", onError);
                return new List<ChatMessageModel>();
            }
        }

        public void AddMessage(ChatMessageModel message, EventCallback<string> onError)
        {
            try
            {
                conversation.Add(message);
            }
            catch (Exception ex)
            {
                HandleError($"Error adding message: {ex.Message}", onError);
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
                HandleError($"Error adding user message: {ex.Message}", onError);
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
                HandleError($"Error adding forget previous role message: {ex.Message}", onError);
            }
        }

        public void AddRoleMessage(string roleName, EventCallback<string> onError)
        {
            try
            {
                if (RolePrompts.Roles.TryGetValue(roleName, out var roleDescription))
                {
                    var roleMessage = new ChatMessageModel(roleDescription, "system") { DisplayInUI = false };
                    AddMessage(roleMessage, onError);
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error adding role message: {ex.Message}", onError);
            }
        }

        public void ClearConversation(EventCallback<string> onError)
        {
            try
            {
                conversation.Clear();
            }
            catch (Exception ex)
            {
                HandleError($"Error clearing conversation: {ex.Message}", onError);
            }
        }
    }
}