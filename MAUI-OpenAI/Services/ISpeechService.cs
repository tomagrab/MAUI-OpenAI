namespace MAUI_OpenAI.Services
{
    public interface ISpeechService
    {
        event Action<bool>? OnSpeechStateChanged;
        bool IsSpeechEnabled { get; }
        void ToggleSpeech();
        void SetSpeechEnabled(bool isEnabled);
        void SetSpeechDisabled();
    }
}