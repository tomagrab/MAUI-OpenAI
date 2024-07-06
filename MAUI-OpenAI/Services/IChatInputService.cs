using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MAUI_OpenAI.Services
{
    public interface IChatInputService
    {
        Task HandleSubmitAsync(string userMessage, EventCallback<string> onSendMessage, EventCallback<string> onError);
        Task ClearMessagesAsync(EventCallback onClearMessages, EventCallback<string> onError);
        Task ToggleImageGenerationAsync(bool isImageGenerationMode, EventCallback<bool> onToggleImageGeneration, EventCallback<string> onError);
        string GetFormContainerCssClass(bool isFocused, string platform);
        Task FocusTextAreaAsync(ElementReference textAreaRef, IJSRuntime js);
        void HandleError(string message, EventCallback<string> onError);
        int EstimateTokenCount(string userMessage, ITokenizerService tokenizerService);
    }
}