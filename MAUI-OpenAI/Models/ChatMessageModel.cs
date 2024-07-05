namespace MAUI_OpenAI.Models
{
    public class ChatMessageModel
    {
        public ChatMessageModel
        (
            string message,
            string role,
            bool isImage = false,
            bool isImageLoading = false
        )
        {
            Message = message;
            Role = role;
            IsImage = isImage;
            IsImageLoading = isImageLoading;
        }

        public string Message { get; set; }
        public string Role { get; set; }
        public bool IsImage { get; set; }
        public bool IsImageLoading { get; set; }
        public string HtmlContent { get; set; } = string.Empty;
    }
}
