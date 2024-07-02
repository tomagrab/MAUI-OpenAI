using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using MAUI_OpenAI.Services;

namespace MAUI_OpenAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            var OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new Exception("OPENAI_API_KEY is not set");

            ChatClient chatClient = new (
                model: "gpt-4o",
                OPENAI_API_KEY
            );

            builder.Services.AddSingleton(chatClient);
            builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
