using OpenAI.Audio;

namespace MAUI_OpenAI.Services
{
    public interface ISpeechService
    {
        bool IsSpeechEnabled { get; }
        GeneratedSpeechVoice SelectedVoice { get; set; }
        Task SetSpeechDisabled();
        Task SetSpeechEnabled(bool isEnabled);
        Task ToggleSpeech();
        event Func<Task>? OnStopAudioRequested;
    }
}