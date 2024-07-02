namespace MAUI_OpenAI.Models
{
    public class ChatMessageModel
    {
        public ChatMessageModel(string message, string role, bool isImage = false)
        {
            Message = message;
            Role = role;
            IsImage = isImage;
        }

        public string Message { get; set; }
        public string Role { get; set; }
        public bool IsImage { get; set; }
    }
}
