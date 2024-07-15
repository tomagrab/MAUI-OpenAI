using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ConversationService : BaseService, IConversationService
    {
        private readonly List<ConversationModel> conversations = new List<ConversationModel>();

        public List<ConversationModel> GetAllConversations()
        {
            return conversations;
        }

        public ConversationModel? GetConversationById(Guid id)
        {
            return conversations.FirstOrDefault(c => c.Id == id);
        }

        public ConversationModel AddConversation(string title)
        {
            var conversation = new ConversationModel(title);
            conversations.Add(conversation);
            return conversation;
        }

        public void RemoveConversation(Guid conversationId, EventCallback<string> onError)
        {
            try
            {
                var conversation = GetConversationById(conversationId);
                if (conversation != null)
                {
                    conversations.Remove(conversation);
                }
                else
                {
                    onError.InvokeAsync("Conversation not found.");
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error removing conversation: {ex.Message}", onError);
            }
        }
    }
}