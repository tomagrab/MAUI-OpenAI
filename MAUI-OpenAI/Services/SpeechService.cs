namespace MAUI_OpenAI.Services
{
    public class SpeechService : ISpeechService
    {
        private bool _isSpeechEnabled;

        public event Action<bool>? OnSpeechStateChanged;

        public bool IsSpeechEnabled => _isSpeechEnabled;

        public void ToggleSpeech()
        {
            _isSpeechEnabled = !_isSpeechEnabled;
            NotifyStateChanged();
        }

        public void SetSpeechEnabled(bool isEnabled)
        {
            _isSpeechEnabled = isEnabled;
            NotifyStateChanged();
        }

        public void SetSpeechDisabled()
        {
            _isSpeechEnabled = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnSpeechStateChanged?.Invoke(_isSpeechEnabled);
    }
}