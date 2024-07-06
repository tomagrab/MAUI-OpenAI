using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class BaseService : IBaseService
    {
        public void HandleError(string message, EventCallback<string> onError)
        {
            if (onError.HasDelegate)
            {
                onError.InvokeAsync(message);
            }
        }

        public async Task HandleErrorAsync(string message, EventCallback<string> onError)
        {
            if (onError.HasDelegate)
            {
                await onError.InvokeAsync(message);
            }
        }

        public async Task InvokeIfHasDelegateAsync<T>(EventCallback<T> callback, T arg)
        {
            if (callback.HasDelegate)
            {
                await callback.InvokeAsync(arg);
            }
        }
    }
}