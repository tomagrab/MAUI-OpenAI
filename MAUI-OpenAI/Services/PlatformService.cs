namespace MAUI_OpenAI.Services
{
    public class PlatformService : IPlatformService
    {
        public string GetPlatform()
        {
            try
            {
                return DeviceInfo.Platform.ToString();
            }
            catch (Exception ex)
            {
                // Handle unexpected errors gracefully and provide a default value
                return $"Unknown Platform (Error: {ex.Message})";
            }
        }
    }
}
