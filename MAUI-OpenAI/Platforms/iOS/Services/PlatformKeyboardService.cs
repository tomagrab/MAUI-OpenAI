using Foundation;
using UIKit;
using MAUI_OpenAI.Services;

[assembly: Dependency(typeof(MAUI_OpenAI.Platforms.iOS.Services.PlatformKeyboardService))]
namespace MAUI_OpenAI.Platforms.iOS.Services
{
    public class PlatformKeyboardService
    {
        public static event EventHandler<bool> KeyboardStateChanged;

        public PlatformKeyboardService()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyboardDidShow);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardWillHide);
        }

        private void KeyboardDidShow(NSNotification notification)
        {
            KeyboardStateChanged?.Invoke(this, true);
        }

        private void KeyboardWillHide(NSNotification notification)
        {
            KeyboardStateChanged?.Invoke(this, false);
        }
    }
}
