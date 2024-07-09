public class InputStateService : IInputStateService
{
    public bool IsInputDisabled(bool isSendingMessage, bool isImageLoading, bool isAppearanceLoading, int tokenCount, int maxTokens)
    {
        return isSendingMessage || isImageLoading || isAppearanceLoading || tokenCount > maxTokens;
    }
}