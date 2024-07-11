using Microsoft.AspNetCore.Components;
using OpenAI.Audio;

namespace MAUI_OpenAI.Services
{
    public class SpeechService : BaseService, ISpeechService
    {
        private bool _isSpeechEnabled;
        private GeneratedSpeechVoice _selectedVoice = GeneratedSpeechVoice.Alloy;

        public event Action<bool>? OnSpeechStateChanged;
        public event Func<Task>? OnStopAudioRequested;

        public bool IsSpeechEnabled => _isSpeechEnabled;

        public GeneratedSpeechVoice SelectedVoice
        {
            get => _selectedVoice;
            set => _selectedVoice = value;
        }

        public async Task ToggleSpeech()
        {
            try
            {
                _isSpeechEnabled = !_isSpeechEnabled;
                NotifyStateChanged();

                if (OnStopAudioRequested != null)
                {
                    await OnStopAudioRequested.Invoke();
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error toggling speech: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
            }
        }

        public Task SetSpeechEnabled(bool isEnabled)
        {
            try
            {
                _isSpeechEnabled = isEnabled;
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                HandleError($"Error setting speech enabled: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
            }

            return Task.CompletedTask;
        }

        public Task SetSpeechDisabled()
        {
            try
            {
                _isSpeechEnabled = false;
                NotifyStateChanged();
                if (OnStopAudioRequested != null)
                {
                    OnStopAudioRequested.Invoke();
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error setting speech disabled: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
            }

            return Task.CompletedTask;
        }

        private void NotifyStateChanged()
        {
            try
            {
                OnSpeechStateChanged?.Invoke(_isSpeechEnabled);
            }
            catch (Exception ex)
            {
                HandleError($"Error notifying state change: {ex.Message}", EventCallback.Factory.Create<string>(this, message => { }));
            }
        }
    }
}