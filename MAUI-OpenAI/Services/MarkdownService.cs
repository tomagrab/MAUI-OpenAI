using Markdig;
using Markdig.SyntaxHighlighting;
using Microsoft.AspNetCore.Components;

namespace MAUI_OpenAI.Services
{
    public class MarkdownService : BaseService, IMarkdownService
    {
        private readonly MarkdownPipeline pipeline;

        public MarkdownService()
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();
        }

        public async Task<string> ConvertToHtmlAsync(string markdown, EventCallback<string> onError)
        {
            try
            {
                return await Task.Run(() => Markdig.Markdown.ToHtml(markdown, pipeline));
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Error converting markdown to HTML: {ex.Message}", onError);
                return string.Empty;
            }
        }
    }
}