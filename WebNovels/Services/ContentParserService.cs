using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Components.Forms;
using OpenXmlPowerTools;
using System.Xml.Linq;

namespace WebNovels.Services
{
    public class ContentParserService : IContentParserService
    {
        public async Task<string> ParseAsync(IBrowserFile file)
        {
            var extension = Path.GetExtension(file.Name).ToLower();

            using var stream = file.OpenReadStream(10 * 1024 * 1024);

            return extension switch
            {
                ".md" => await ParseMarkdownAsync(stream),
                ".docx" => await ParseDocxAsHtmlAsync(stream),
                _ => throw new NotSupportedException($"File type {extension} is not supported.")
            };
        }

        private async Task<string> ParseMarkdownAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var markdown = await reader.ReadToEndAsync();
            return Markdig.Markdown.ToHtml(markdown);
        }

        private async Task<string> ParseDocxAsHtmlAsync(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var doc = WordprocessingDocument.Open(memoryStream, true);

            var settings = new HtmlConverterSettings
            {
                PageTitle = "Chapter Content",
                FabricateCssClasses = true,
                CssClassPrefix = "docx-",
                RestrictToSupportedLanguages = false,
                RestrictToSupportedNumberingFormats = false
            };

            XElement htmlElement = HtmlConverter.ConvertToHtml(doc, settings);
            return htmlElement.ToString(SaveOptions.DisableFormatting);
        }
    }
}
