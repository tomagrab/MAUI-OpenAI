using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ConversationService : BaseService, IConversationService
    {
        private readonly List<ConversationModel> conversations = new List<ConversationModel>();
        private readonly ITokenizerService _tokenizerService;
        private const int MaxTokens = 80000;

        public ConversationService(ITokenizerService tokenizerService)
        {
            _tokenizerService = tokenizerService;
        }

        public List<ConversationModel> GetAllConversations()
        {
            return conversations;
        }

        public ConversationModel? GetConversationById(Guid id)
        {
            return conversations.FirstOrDefault(c => c.Id == id);
        }

        public async Task<List<ChatMessageModel>> GetTrimmedConversationAsync(Guid conversationId, EventCallback<string> onError)
        {
            try
            {
                var conversation = GetConversationById(conversationId);

                if (conversation != null)
                {
                    var textMessages = conversation.GetTextMessages();

                    return await Task.Run(() => conversation?.GetTrimmedTextMessages(MaxTokens, _tokenizerService, onError) ?? new List<ChatMessageModel>());
                }

                return await Task.Run(() => conversation?.GetTrimmedTextMessages(MaxTokens, _tokenizerService, onError) ?? new List<ChatMessageModel>());
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"An error occurred while trimming the conversation: {ex.Message}", onError);
                return new List<ChatMessageModel>();
            }
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