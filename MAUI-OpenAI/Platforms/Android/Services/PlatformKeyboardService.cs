using Android.App;

[assembly: Dependency(typeof(MAUI_OpenAI.Platforms.Android.Services.PlatformKeyboardService))]
namespace MAUI_OpenAI.Platforms.Android.Services
{
    public class PlatformKeyboardService
    {
        public static event EventHandler<bool>? KeyboardStateChanged;

        public PlatformKeyboardService()
        {
            var activity = (Activity)MauiApplication.Context!;
            activity.Window.DecorView.ViewTreeObserver.GlobalLayout += (sender, e) =>
            {
                var r = new Rect();
                activity.Window.DecorView.GetWindowVisibleDisplayFrame(r);
                var screenHeight = activity.Window.DecorView.RootView.Height;
                var keypadHeight = screenHeight - r.Bottom;
                var isVisible = keypadHeight > screenHeight * 0.15;
                KeyboardStateChanged?.Invoke(this, isVisible);
            };
        }
    }
}
