namespace MAUI_OpenAI.Services
{
    public interface IGetAppearanceService
    {
        Task<string> GeneratePromptAsync(string inputPrompt, IOpenAIService openAIService);
        string ExtractDallePrompt(string response);
    }
}