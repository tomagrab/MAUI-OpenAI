using Microsoft.AspNetCore.Components;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public class ChatService : BaseService, IChatService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IConversationService _conversationService;
        private ConversationModel _currentConversation;

        public ChatService(IOpenAIService openAIService, IConversationService conversationService)
        {
            _openAIService = openAIService;
            _conversationService = conversationService;
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

        public async Task HandleSendMessageAsync(string message, bool isImageGenerationMode, EventCallback<string> onError, Func<Task> onStateChange, EventCallback<byte[]> onImageGenerated, Func<Task> onResponseComplete)
        {
            await _currentConversation.HandleSendMessageAsync(message, isImageGenerationMode, onError, onStateChange, onImageGenerated, onResponseComplete, _openAIService);
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