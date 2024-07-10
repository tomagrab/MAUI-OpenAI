using System.ClientModel;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class GetAppearanceService : BaseService, IGetAppearanceService
    {
        private readonly IConversationService _conversationService;

        public GetAppearanceService(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<string> GeneratePromptAsync(string inputPrompt, IOpenAIService openAIService, EventCallback<string> onError)
        {
            string result = "";

            try
            {
                var temporaryConversation = new List<ChatMessageModel>
                {
                    new ChatMessageModel(inputPrompt, "user")
                };

                await openAIService.GetChatCompletionStreamingAsync(
                    temporaryConversation,
                    EventCallback.Factory.Create<string>(this, (update) =>
                    {
                        result += update;
                    }),
                    EventCallback.Factory.Create(this, () =>
                    {
                        result = ExtractDallePrompt(result);
                        return Task.CompletedTask;
                    }),
                    onError,
                    () => Task.CompletedTask
                );
            }
            catch (ClientResultException cre)
            {
                await HandleErrorAsync($"ClientResultException: {cre.Message}", onError);
                result = cre.Message;
            }
            catch (TimeoutException te)
            {
                await HandleErrorAsync($"TimeoutException: {te.Message}", onError);
                result = te.Message;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Exception: {ex.Message}", onError);
                result = ex.Message;
            }

            return result;
        }

        public string ExtractDallePrompt(string response)
        {
            try
            {
                if (response.Contains("\""))
                {
                    var match = Regex.Match(response, "\"([^\"]*)\"");
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error extracting DALLE prompt: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
            }

            return response;
        }
    }
}