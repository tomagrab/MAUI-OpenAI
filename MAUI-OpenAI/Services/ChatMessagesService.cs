using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class ChatMessagesService : IChatMessagesService
    {
        public string GetMessageCssClass(string role, EventCallback<string> onError)
        {
            try
            {
                return role switch
                {
                    "user" => "bg-blue text-brightWhite self-end ml-auto text-right",
                    "assistant" => "bg-green text-brightWhite self-start mr-auto text-left",
                    "error" => "bg-red text-brightWhite self-start mr-auto text-left",
                    "success" => "bg-brightGreen text-brightWhite self-start mr-auto text-left",
                    _ => "bg-gray text-brightWhite self-start mr-auto text-left"
                };
            }
            catch (Exception ex)
            {
                HandleError($"Error generating CSS class: {ex.Message}", onError);
                return "bg-gray text-brightWhite self-start mr-auto text-left";
            }
        }

        public async Task HandleImageClickAsync(string imageSrc, EventCallback<string> onImageClick, EventCallback<string> onError)
        {
            try
            {
                if (onImageClick.HasDelegate)
                {
                    await onImageClick.InvokeAsync(imageSrc);
                }
                else
                {
                    HandleError("Image click handler is not configured.", onError);
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error handling image click: {ex.Message}", onError);
            }
        }

        public void HandleError(string message, EventCallback<string> onError)
        {
            if (onError.HasDelegate)
            {
                onError.InvokeAsync(message);
            }
        }
    }
}