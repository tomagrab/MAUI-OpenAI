using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MAUI_OpenAI.Services
{
    public class ChatInputService : BaseService, IChatInputService
    {
        public async Task HandleSubmitAsync(string userMessage, EventCallback<string> onSendMessage, EventCallback<string> onError)
        {
            try
            {
                if (onSendMessage.HasDelegate)
                {
                    string tempMessage = userMessage ?? string.Empty;
                    await onSendMessage.InvokeAsync(tempMessage);
                }
                else
                {
                    await HandleErrorAsync("Message sending function is not configured.", onError);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error sending message: {ex.Message}", onError);
            }
        }

        public async Task ClearMessagesAsync(EventCallback onClearMessages, EventCallback<string> onError)
        {
            try
            {
                if (onClearMessages.HasDelegate)
                {
                    await onClearMessages.InvokeAsync();
                }
                else
                {
                    await HandleErrorAsync("Clear messages function is not configured.", onError);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error clearing messages: {ex.Message}", onError);
            }
        }

        public async Task ToggleImageGenerationAsync(bool isImageGenerationMode, EventCallback<bool> onToggleImageGeneration, EventCallback<string> onError)
        {
            try
            {
                if (onToggleImageGeneration.HasDelegate)
                {
                    await onToggleImageGeneration.InvokeAsync(!isImageGenerationMode);
                }
                else
                {
                    await HandleErrorAsync("Image generation toggle function is not configured.", onError);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error toggling image generation: {ex.Message}", onError);
            }
        }

        public string GetFormContainerCssClass(bool isFocused, string platform)
        {
            var baseClass = "p-4 shadow-md";
            try
            {
                return isFocused && (platform == "iOS" || platform == "Android") ? $"{baseClass} mb-64" : baseClass;
            }
            catch (Exception ex)
            {
                HandleError($"Error generating CSS class: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
                return baseClass;
            }
        }

        public async Task FocusTextAreaAsync(ElementReference textAreaRef, IJSRuntime js)
        {
            await js.InvokeVoidAsync("focusElement", textAreaRef);
        }
    }
}