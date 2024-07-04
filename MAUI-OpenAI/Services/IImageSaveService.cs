namespace MAUI_OpenAI.Services
{
    public interface IImageSaveService
    {
        Task SaveImageAsync(string base64Image, string fileName);
    }
}
