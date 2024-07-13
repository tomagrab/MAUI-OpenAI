using Microsoft.AspNetCore.Components;
using OpenAI.Audio;
using MAUI_OpenAI.Models;

namespace MAUI_OpenAI.Services
{
    public interface IOpenAIService
    {
        Task GetChatCompletionStreamingAsync(ConversationModel conversation, EventCallback<string> onUpdate, EventCallback onComplete, EventCallback<string> onError, Func<Task> onResponseComplete);
        Task GenerateImageAsync(string prompt, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError);
        Task GenerateEmbeddingAsync(List<string> texts, EventCallback<List<ReadOnlyMemory<float>>> onEmbeddingsGenerated, EventCallback<string> onError);
        Task TranscribeAudioAsync(string audioFilePath, EventCallback<string> onTranscriptionCompleted, EventCallback<string> onError);
        Task GenerateSpeechAsync(string text, GeneratedSpeechVoice voice, Func<byte[], Task> onSpeechGenerated, EventCallback<string> onError);
    }
}