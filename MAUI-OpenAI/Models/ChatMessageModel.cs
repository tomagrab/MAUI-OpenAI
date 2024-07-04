namespace MAUI_OpenAI.Models
{
    public class ChatMessageModel
    {
        public ChatMessageModel(string message, string role, bool isImage = false, bool isLoading = false)
        {
            Message = message;
            Role = role;
            IsImage = isImage;
            IsLoading = isLoading;
        }

        public string Message { get; set; }
        public string Role { get; set; }
        public bool IsImage { get; set; }
        public bool IsLoading { get; set; }
        public string HtmlContent { get; set; } = string.Empty;
    }
}
