using Microsoft.Maui.Controls;
using System;

namespace MAUI_OpenAI.Services
{
    public class KeyboardService : IKeyboardService
    {
        public event EventHandler<bool>? KeyboardStateChanged;

        public KeyboardService()
        {
            RegisterForKeyboardNotifications();
        }

        private void RegisterForKeyboardNotifications()
        {
            // This needs to be implemented for each platform
#if ANDROID
            PlatformKeyboardService.KeyboardStateChanged += OnKeyboardStateChanged;
#elif IOS
            PlatformKeyboardService.KeyboardStateChanged += OnKeyboardStateChanged;
#endif
        }

        private void OnKeyboardStateChanged(object sender, bool isVisible)
        {
            KeyboardStateChanged?.Invoke(this, isVisible);
        }
    }
}
