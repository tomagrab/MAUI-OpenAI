using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IPlatformService
    {
        string GetPlatform(EventCallback<string> onError);
    }
}
