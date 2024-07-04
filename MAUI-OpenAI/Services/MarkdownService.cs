using Markdig;

namespace MAUI_OpenAI.Services
{
    public class MarkdownService : IMarkdownService
    {
        public string ConvertToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdig.Markdown.ToHtml(markdown, pipeline);
        }
    }
}