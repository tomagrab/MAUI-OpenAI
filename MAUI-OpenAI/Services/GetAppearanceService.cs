using System.Text.RegularExpressions;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public class GetAppearanceService : IGetAppearanceService
    {
        public async Task<string> GeneratePromptAsync(string inputPrompt, IOpenAIService openAIService)
        {
            string result = "";
            await openAIService.GetChatCompletionStreamingAsync(new List<ChatMessageModel>(), inputPrompt, (update) =>
            {
                result += update;
            }, () =>
            {
                result = ExtractDallePrompt(result);
            });

            return result;
        }

        public string ExtractDallePrompt(string response)
        {
            if (response.Contains("\""))
            {
                var match = Regex.Match(response, "\"([^\"]*)\"");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return response;
        }
    }
}