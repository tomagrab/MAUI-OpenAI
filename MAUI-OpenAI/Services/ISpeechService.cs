using OpenAI.Audio;

namespace MAUI_OpenAI.Services
{
    public interface ISpeechService
    {
        event Action<bool>? OnSpeechStateChanged;
        bool IsSpeechEnabled { get; }
        GeneratedSpeechVoice SelectedVoice { get; set; }
        void ToggleSpeech();
        void SetSpeechEnabled(bool isEnabled);
        void SetSpeechDisabled();
    }
}