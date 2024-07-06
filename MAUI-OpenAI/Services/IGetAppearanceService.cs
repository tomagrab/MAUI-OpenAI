using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IGetAppearanceService
    {
        Task<string> GeneratePromptAsync(string inputPrompt, IOpenAIService openAIService, EventCallback<string> onError);
        string ExtractDallePrompt(string response);
    }
}