using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class PlatformService : BaseService, IPlatformService
    {
        public string GetPlatform(EventCallback<string> onError)
        {
            try
            {
                return DeviceInfo.Platform.ToString();
            }
            catch (Exception ex)
            {
                HandleError($"Unknown Platform (Error: {ex.Message})", onError);
                return "Unknown Platform";
            }
        }
    }
}