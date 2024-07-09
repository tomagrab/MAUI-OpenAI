public interface IInputStateService
{
    bool IsInputDisabled(bool isSendingMessage, bool isImageLoading, bool isAppearanceLoading, int tokenCount, int maxTokens);
}