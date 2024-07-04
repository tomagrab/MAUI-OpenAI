using CommunityToolkit.Maui.Storage;

namespace MAUI_OpenAI.Services
{
    public class ImageSaveService : IImageSaveService
    {
        private readonly IFileSaver _fileSaver;

        public ImageSaveService(IFileSaver fileSaver)
        {
            _fileSaver = fileSaver;
        }

        public async Task SaveImageAsync(string base64Image, string fileName)
        {
            try
            {
                var base64Data = base64Image.Substring(base64Image.IndexOf(',') + 1);
                var imageBytes = Convert.FromBase64String(base64Data);

                using var stream = new MemoryStream(imageBytes);
                var result = await _fileSaver.SaveAsync(fileName, stream, default);

                if (!result.IsSuccessful)
                {
                    throw new Exception(result.Exception?.Message ?? "Unknown error");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving image: {ex.Message}");
            }
        }
    }
}
