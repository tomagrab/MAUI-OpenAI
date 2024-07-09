﻿using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using MAUI_OpenAI.Services;
using OpenAI.Images;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using OpenAI.Embeddings;
using OpenAI.Audio;

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

            EmbeddingClient embeddingClient = new (
                model: "text-embedding-3-small",
                OPENAI_API_KEY
            );

            AudioClient transcribeClient = new (
                model: "whisper-1",
                OPENAI_API_KEY
            );

            AudioClient textToSpeechClient = new (
                model: "tts-1",
                OPENAI_API_KEY
            );

            builder.Services.AddSingleton(chatClient);
            builder.Services.AddSingleton(imageClient);
            builder.Services.AddSingleton(embeddingClient);
            builder.Services.AddSingleton(transcribeClient);
            builder.Services.AddSingleton(textToSpeechClient);
            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            builder.Services.AddSingleton<IBaseService, BaseService>();
            builder.Services.AddSingleton<IChatService, ChatService>();
            builder.Services.AddSingleton<IChatInputService, ChatInputService>();
            builder.Services.AddSingleton<IChatMessagesService, ChatMessagesService>();
            builder.Services.AddSingleton<IComboBoxService, ComboBoxService>();
            builder.Services.AddSingleton<IConversationService, ConversationService>();
            builder.Services.AddSingleton<IGetAppearanceService, GetAppearanceService>();
            builder.Services.AddSingleton<IImageSaveService, ImageSaveService>();
            builder.Services.AddSingleton<IInputStateService, InputStateService>();
            builder.Services.AddSingleton<IMarkdownService, MarkdownService>();
            builder.Services.AddSingleton<IModalService, ModalService>();
            builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
            builder.Services.AddSingleton<IPlatformService, PlatformService>();
            builder.Services.AddSingleton<IRoleSelectorService, RoleSelectorService>();
            builder.Services.AddSingleton<ISpeechService, SpeechService>();
            builder.Services.AddSingleton<ITokenizerService, TokenizerService>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
