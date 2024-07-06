using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IMarkdownService
    {
        Task<string> ConvertToHtmlAsync(string markdown, EventCallback<string> onError);
    }
}
