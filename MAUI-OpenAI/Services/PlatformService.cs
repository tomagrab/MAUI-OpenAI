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
                return $"Unknown Platform (Error: {ex.Message})";
            }
        }
    }
}
