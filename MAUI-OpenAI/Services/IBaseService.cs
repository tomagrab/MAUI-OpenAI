using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IBaseService
    {
        void HandleError(string message, EventCallback<string> onError);
        Task HandleErrorAsync(string message, EventCallback<string> onError);
        Task InvokeIfHasDelegateAsync<T>(EventCallback<T> callback, T arg);
    }
}