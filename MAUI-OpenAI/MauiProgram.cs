using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using MAUI_OpenAI.Services;
using OpenAI.Images;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;

namespace MAUI_OpenAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            var OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new Exception("OPENAI_API_KEY is not set");

            ChatClient chatClient = new (
                model: "gpt-4o",
                OPENAI_API_KEY
            );

            ImageClient imageClient = new (
                model: "dall-e-3",
                OPENAI_API_KEY
            );

            builder.Services.AddSingleton(chatClient);
            builder.Services.AddSingleton(imageClient);
            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            builder.Services.AddSingleton<ITokenizerService, TokenizerService>();
            builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
            builder.Services.AddSingleton<IPlatformService, PlatformService>();
            builder.Services.AddSingleton<IMarkdownService, MarkdownService>();
            builder.Services.AddSingleton<IImageSaveService, ImageSaveService>();
            builder.Services.AddSingleton<IChatInputService, ChatInputService>();
            builder.Services.AddSingleton<IChatMessagesService, ChatMessagesService>();
            builder.Services.AddSingleton<IComboBoxService, ComboBoxService>();
            builder.Services.AddSingleton<IGetAppearanceService, GetAppearanceService>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
