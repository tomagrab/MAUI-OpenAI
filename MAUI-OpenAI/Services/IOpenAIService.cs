using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public interface IOpenAIService
    {
        Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, EventCallback<string> onUpdate, EventCallback onComplete, EventCallback<string> onError);
        Task GenerateImageAsync(string prompt, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError);
        Task GenerateEmbeddingAsync(List<string> texts, EventCallback<List<ReadOnlyMemory<float>>> onEmbeddingsGenerated, EventCallback<string> onError);
        Task TranscribeAudioAsync(string audioFilePath, EventCallback<string> onTranscriptionCompleted, EventCallback<string> onError);
        Task GenerateSpeechAsync(string text, EventCallback<byte[]> onSpeechGenerated, EventCallback<string> onError);
    }
}