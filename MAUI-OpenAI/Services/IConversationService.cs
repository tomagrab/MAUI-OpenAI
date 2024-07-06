using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IConversationService
    {
        List<ChatMessageModel> GetConversation();
        List<ChatMessageModel> GetTextConversation();
        Task<List<ChatMessageModel>> GetTrimmedConversationAsync(EventCallback<string> onError);
        void AddMessage(ChatMessageModel message, EventCallback<string> onError);
        void AddUserMessage(string message, EventCallback<string> onError);
        void AddForgetPreviousRoleMessage(string currentRole, EventCallback<string> onError);
        void AddRoleMessage(string role, EventCallback<string> onError);
        void ClearConversation(EventCallback<string> onError);
    }
}