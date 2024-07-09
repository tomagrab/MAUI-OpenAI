using OpenAI.Chat;
using System.ClientModel;
using OpenAI.Images;
using OpenAI.Embeddings;
using OpenAI.Audio;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace MAUI_OpenAI.Services
{
    public class OpenAIService : BaseService, IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly ImageClient _imageClient;
        private readonly EmbeddingClient _embeddingClient;
        private readonly AudioClient _transcribeClient;
        private readonly AudioClient _textToSpeechClient;
        private readonly IConversationService _conversationService;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(
            ChatClient chatClient,
            ImageClient imageClient,
            EmbeddingClient embeddingClient,
            AudioClient transcribeClient,
            AudioClient textToSpeechClient,
            IConversationService conversationService,
            ILogger<OpenAIService> logger)
        {
            _chatClient = chatClient;
            _imageClient = imageClient;
            _embeddingClient = embeddingClient;
            _transcribeClient = transcribeClient;
            _textToSpeechClient = textToSpeechClient;
            _conversationService = conversationService;
            _logger = logger;
        }

        public async Task GetChatCompletionStreamingAsync(List<ChatMessageModel> conversation, EventCallback<string> onUpdate, EventCallback onComplete, EventCallback<string> onError)
        {
            try
            {
                var messages = conversation.Select(c => new UserChatMessage(c.Message)).ToArray();
                AsyncResultCollection<StreamingChatCompletionUpdate> updates = _chatClient.CompleteChatStreamingAsync(messages);

                await foreach (StreamingChatCompletionUpdate update in updates)
                {
                    foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                    {
                        await onUpdate.InvokeAsync(updatePart.Text);
                    }
                }

                await onComplete.InvokeAsync();
            }
            catch (ClientResultException cre)
            {
                _logger.LogError(cre, "An error occurred while processing the request");
                await HandleErrorAsync($"An error occurred while processing the request: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                _logger.LogError(te, "Request timed out");
                await HandleErrorAsync("Request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request");
                await HandleErrorAsync($"An unexpected error occurred while processing the request: {ex.Message}", onError);
            }
        }

        public async Task GenerateImageAsync(string prompt, EventCallback<byte[]> onImageGenerated, EventCallback<string> onError)
        {
            try
            {
                var options = new ImageGenerationOptions
                {
                    Quality = GeneratedImageQuality.High,
                    Size = GeneratedImageSize.W1792xH1024,
                    Style = GeneratedImageStyle.Vivid,
                    ResponseFormat = GeneratedImageFormat.Bytes
                };

                GeneratedImage image = await _imageClient.GenerateImageAsync(prompt, options);
                await onImageGenerated.InvokeAsync(image.ImageBytes.ToArray());
            }
            catch (ClientResultException cre)
            {
                _logger.LogError(cre, "An error occurred while generating the image");
                await HandleErrorAsync($"An error occurred while generating the image: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                _logger.LogError(te, "Image generation request timed out");
                await HandleErrorAsync("Image generation request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while generating the image");
                await HandleErrorAsync($"An unexpected error occurred while generating the image: {ex.Message}", onError);
            }
        }

        public async Task GenerateEmbeddingAsync(List<string> texts, EventCallback<List<ReadOnlyMemory<float>>> onEmbeddingsGenerated, EventCallback<string> onError)
        {
            try
            {
                EmbeddingCollection embeddings = await _embeddingClient.GenerateEmbeddingsAsync(texts);
                var vectors = embeddings.Select(e => e.Vector).ToList();
                await onEmbeddingsGenerated.InvokeAsync(vectors);
            }
            catch (ClientResultException cre)
            {
                _logger.LogError(cre, "An error occurred while generating the embeddings");
                await HandleErrorAsync($"An error occurred while generating the embeddings: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                _logger.LogError(te, "Embedding generation request timed out");
                await HandleErrorAsync("Embedding generation request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while generating the embeddings");
                await HandleErrorAsync($"An unexpected error occurred while generating the embeddings: {ex.Message}", onError);
            }
        }

        public async Task TranscribeAudioAsync(string audioFilePath, EventCallback<string> onTranscriptionCompleted, EventCallback<string> onError)
        {
            try
            {
                AudioTranscriptionOptions options = new()
                {
                    ResponseFormat = AudioTranscriptionFormat.Verbose,
                    Granularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
                };

                AudioTranscription transcription = await _transcribeClient.TranscribeAudioAsync(audioFilePath, options);
                await onTranscriptionCompleted.InvokeAsync(transcription.Text);
            }
            catch (ClientResultException cre)
            {
                _logger.LogError(cre, "An error occurred while transcribing the audio");
                await HandleErrorAsync($"An error occurred while transcribing the audio: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                _logger.LogError(te, "Audio transcription request timed out");
                await HandleErrorAsync("Audio transcription request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while transcribing the audio");
                await HandleErrorAsync($"An unexpected error occurred while transcribing the audio: {ex.Message}", onError);
            }
        }

        public async Task GenerateSpeechAsync(string text, Func<byte[], Task> onSpeechGenerated, EventCallback<string> onError)
        {
            try
            {
                BinaryData speech = await _textToSpeechClient.GenerateSpeechFromTextAsync(text, GeneratedSpeechVoice.Alloy);
                await onSpeechGenerated.Invoke(speech.ToArray());
            }
            catch (ClientResultException cre)
            {
                _logger.LogError(cre, "An error occurred while generating the speech");
                await HandleErrorAsync($"An error occurred while generating the speech: {cre.Message}", onError);
            }
            catch (TimeoutException te)
            {
                _logger.LogError(te, "Text-to-speech request timed out");
                await HandleErrorAsync("Text-to-speech request timed out: " + te.Message, onError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while generating the speech");
                await HandleErrorAsync($"An unexpected error occurred while generating the speech: {ex.Message}", onError);
            }
        }
    }
}