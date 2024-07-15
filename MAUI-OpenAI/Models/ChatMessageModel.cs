using MAUI_OpenAI.Services;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Models
{
    public class ChatMessageModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }
        public bool IsImage { get; set; }
        public bool IsImageLoading { get; set; }
        public string HtmlContent { get; set; }
        public bool DisplayInUI { get; set; } = true;
        public Guid ConversationId { get; set; }

        public ITokenizerService TokenizerService { get; set; } = Application.Current!.MainPage!.Handler!.MauiContext!.Services!.GetService<ITokenizerService>()!;

        public ChatMessageModel(string message, string role, bool isImage = false, bool isImageLoading = false)
        {
            Id = Guid.NewGuid();
            Message = message;
            Role = role;
            IsImage = isImage;
            IsImageLoading = isImageLoading;
            HtmlContent = string.Empty;
        }

        public void UpdateMessage(string newMessage)
        {
            Message = newMessage;
        }

        public void MarkAsImageLoading()
        {
            IsImage = true;
            IsImageLoading = true;
        }

        public async Task ConvertToHtmlAsync(EventCallback<string> onError)
        {
            try
            {
                var markdownService = Application.Current?.MainPage?.Handler?.MauiContext?.Services?.GetService<IMarkdownService>();
                if (markdownService != null)
                {
                    HtmlContent = await markdownService.ConvertToHtmlAsync(Message, onError);
                }
                else
                {
                    await onError.InvokeAsync("Markdown service not found.");
                }
            }
            catch (Exception ex)
            {
                await onError.InvokeAsync($"Error converting message to HTML: {ex.Message}");
            }
        }

        public int EstimateTokenCount(EventCallback<string> onError)
        {
            try
            {
                if (TokenizerService != null)
                {
                    return TokenizerService.EstimateTokenCount(Message, onError);
                }
                else
                {
                    onError.InvokeAsync("Tokenizer service is null.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                onError.InvokeAsync($"Error estimating token count: {ex.Message}");
                return 0;
            }
        }
    }
}