namespace MAUI_OpenAI.Services
{
    public class PlatformService : IPlatformService
    {
        public string GetPlatform()
        {
            return DeviceInfo.Platform.ToString();
        }
    }
}