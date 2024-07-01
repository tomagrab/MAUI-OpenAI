namespace MAUI_OpenAI.Models
{
    public class ChatMessageModel
    {
        public ChatMessageModel(string message, string role)
        {
            Message = message;
            Role = role;
        }

        public string Message { get; set; }
        public string Role { get; set; }
    }
}
