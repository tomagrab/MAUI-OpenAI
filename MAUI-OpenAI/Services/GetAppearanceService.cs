using System.Text.RegularExpressions;
using MAUI_OpenAI.Models;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class GetAppearanceService : BaseService, IGetAppearanceService
    {
        public async Task<string> GeneratePromptAsync(string inputPrompt, IOpenAIService openAIService, EventCallback<string> onError)
        {
            string result = "";
            var conversation = new List<ChatMessageModel>
            {
                new ChatMessageModel(inputPrompt, "user")
            };

            try
            {
                await openAIService.GetChatCompletionStreamingAsync(conversation, EventCallback.Factory.Create<string>(this, (update) =>
                {
                    result += update;
                }), EventCallback.Factory.Create(this, () =>
                {
                    result = ExtractDallePrompt(result);
                }), onError);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error generating prompt: {ex.Message}", onError);
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