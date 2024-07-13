using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IConversationService
    {
        List<ConversationModel> GetAllConversations();
        ConversationModel? GetConversationById(Guid id);
        Task<List<ChatMessageModel>> GetTrimmedConversationAsync(Guid conversationId, EventCallback<string> onError);
        ConversationModel AddConversation(string title);
        void RemoveConversation(Guid conversationId, EventCallback<string> onError);
    }
}