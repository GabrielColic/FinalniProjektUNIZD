using DocumentFormat.OpenXml.Packaging;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components.Forms;
using OpenXmlPowerTools;
using System.Xml.Linq;

namespace WebNovels.Services.ChapterServices
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
            string rawHtml = htmlElement.ToString(SaveOptions.DisableFormatting);

            return CleanHtml(rawHtml);
        }


        private string CleanHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // 1. Remove <style> blocks
            var styleNodes = doc.DocumentNode.SelectNodes("//style");
            if (styleNodes != null)
            {
                foreach (var styleNode in styleNodes)
                    styleNode.Remove();
            }

            // 2. Remove font-size and font-family from inline styles
            foreach (var node in doc.DocumentNode.SelectNodes("//*[@style]") ?? Enumerable.Empty<HtmlNode>())
            {
                string style = node.GetAttributeValue("style", "");

                style = System.Text.RegularExpressions.Regex.Replace(
                    style,
                    @"font-(size|family)\s*:\s*[^;""']+;?",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );

                style = style.Trim().TrimEnd(';');

                if (string.IsNullOrWhiteSpace(style))
                    node.Attributes["style"].Remove();
                else
                    node.SetAttributeValue("style", style);
            }

            // 3. Convert known bold span classes to <strong>
            foreach (var span in doc.DocumentNode.SelectNodes("//span[contains(@class, 'docx-')]") ?? Enumerable.Empty<HtmlNode>())
            {
                var classAttr = span.GetAttributeValue("class", "");
                var isBold = classAttr.Contains("000000") || classAttr.Contains("000007");

                HtmlNode newNode = HtmlNode.CreateNode(isBold ? "<strong></strong>" : "<span></span>");
                newNode.InnerHtml = span.InnerHtml;
                span.ParentNode.ReplaceChild(newNode, span);
            }

            // 4. Remove all remaining class attributes
            foreach (var node in doc.DocumentNode.SelectNodes("//*[@class]") ?? Enumerable.Empty<HtmlNode>())
            {
                node.Attributes["class"].Remove();
            }

            return doc.DocumentNode.OuterHtml;
        }

    }
}
