using Markdig;
using Markdig.SyntaxHighlighting;

namespace MAUI_OpenAI.Services
{
    public class MarkdownService : IMarkdownService
    {
        public string ConvertToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();
            return Markdig.Markdown.ToHtml(markdown, pipeline);
        }
    }
}