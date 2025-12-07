using Markdig;
using System.Text.RegularExpressions;
using System.Net;

namespace MarkdownProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MarkdownProcessor <input-directory> <output-directory>");
                Environment.Exit(1);
            }

            var inputDir = args[0];
            var outputDir = args[1];

            if (!Directory.Exists(inputDir))
            {
                Console.WriteLine($"Input directory does not exist: {inputDir}");
                Environment.Exit(1);
            }

            Directory.CreateDirectory(outputDir);

            var markdownFiles = Directory.GetFiles(inputDir, "*.md", SearchOption.TopDirectoryOnly);

            Console.WriteLine($"Processing {markdownFiles.Length} markdown files...");

            foreach (var file in markdownFiles)
            {
                try
                {
                    ProcessFile(file, outputDir);
                    Console.WriteLine($"Processed: {Path.GetFileName(file)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {file}: {ex.Message}");
                }
            }

            Console.WriteLine("Markdown processing complete!");
        }

        static void ProcessFile(string filePath, string outputDir)
        {
            var content = File.ReadAllText(filePath);
            var (frontmatter, markdownContent) = ParseFrontmatter(content);

            var title = frontmatter?.GetValueOrDefault("title") ?? Path.GetFileNameWithoutExtension(filePath);
            var dateStr = frontmatter?.GetValueOrDefault("date") ?? File.GetCreationTime(filePath).ToString("yyyy-MM-dd");
            var author = frontmatter?.GetValueOrDefault("author") ?? "Admin";

            if (!DateTime.TryParse(dateStr, out var date))
            {
                date = File.GetCreationTime(filePath);
            }

            var slug = GenerateSlug(title);
            var htmlContent = ConvertMarkdownToHtml(markdownContent);

            // Generate TOC and include before content if present
            var toc = GenerateTocHtml(htmlContent);
            var contentWithToc = string.IsNullOrWhiteSpace(toc) ? htmlContent : (toc + "\n" + htmlContent);
            var excerpt = GenerateExcerpt(markdownContent);

            // Create HTML file with metadata
            var htmlFile = Path.Combine(outputDir, $"{slug}.html");
            var htmlOutput = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>{title}</title>
</head>
<body>
    <article>
        <header>
            <h1>{title}</h1>
            <p class=""meta"">Published on {date:MMMM dd, yyyy} by {author}</p>
        </header>
        <div class=""content"">
                {contentWithToc}
        </div>
    </article>
</body>
</html>";

            File.WriteAllText(htmlFile, htmlOutput);
        }

        static (Dictionary<string, string>?, string) ParseFrontmatter(string content)
        {
            var lines = content.Split('\n');
            if (lines.Length < 3 || !lines[0].Trim().Equals("---", StringComparison.Ordinal))
            {
                return (null, content);
            }

            var frontmatterEndIndex = -1;
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals("---", StringComparison.Ordinal))
                {
                    frontmatterEndIndex = i;
                    break;
                }
            }

            if (frontmatterEndIndex == -1)
            {
                return (null, content);
            }

            var frontmatterLines = lines.Skip(1).Take(frontmatterEndIndex - 1).ToList();
            var frontmatter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in frontmatterLines)
            {
                var colonIndex = line.IndexOf(':');
                if (colonIndex > 0)
                {
                    var key = line.Substring(0, colonIndex).Trim();
                    var value = line.Substring(colonIndex + 1).Trim().Trim('"', '\'');
                    frontmatter[key] = value;
                }
            }

            var markdownContent = string.Join('\n', lines.Skip(frontmatterEndIndex + 1));
            return (frontmatter, markdownContent);
        }

        static string ConvertMarkdownToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            return Markdown.ToHtml(markdown, pipeline);
        }

        static string GenerateExcerpt(string markdown, int maxLength = 200)
        {
            var plainText = Regex.Replace(markdown, @"#.*?\n|\*\*.*?\*\*|`.*?`|\[.*?\]\(.*?\)|!\[.*?\]\(.*?\)", "");
            plainText = Regex.Replace(plainText, @"<.*?>", "");
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            if (plainText.Length <= maxLength)
                return plainText;

            var excerpt = plainText.Substring(0, maxLength);
            var lastSpace = excerpt.LastIndexOf(' ');

            if (lastSpace > 0)
                excerpt = excerpt.Substring(0, lastSpace);

            return excerpt + "...";
        }

        static string GenerateSlug(string title)
        {
            var slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }

        static string GenerateTocHtml(string html)
        {
            var headingRegex = new Regex("<h([1-6])\\s+id=\"([^\"]+)\">(.+?)</h\\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = headingRegex.Matches(html);

            var headings = new List<(int level, string id, string text)>();

            foreach (Match m in matches)
            {
                if (!m.Success) continue;
                if (!int.TryParse(m.Groups[1].Value, out var level)) continue;
                var id = m.Groups[2].Value;
                var innerHtml = m.Groups[3].Value;
                var text = Regex.Replace(innerHtml, "<.*?>", "");
                text = WebUtility.HtmlDecode(text).Trim();

                if (level >= 2 && level <= 4)
                {
                    headings.Add((level, id, text));
                }
            }

            if (headings.Count == 0)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<nav class=\"toc-container bg-gray-50 border rounded-md p-4 mb-6\" aria-label=\"Table of contents\">");
            sb.AppendLine("  <p class=\"font-semibold mb-2\">Contents</p>");

            int baseLevel = headings.Min(h => h.level);
            int currentLevel = baseLevel;

            sb.AppendLine("  <ul class=\"toc-list\">");

            foreach (var h in headings)
            {
                while (h.level > currentLevel)
                {
                    sb.AppendLine("    <ul>");
                    currentLevel++;
                }

                while (h.level < currentLevel)
                {
                    sb.AppendLine("    </ul>");
                    currentLevel--;
                }

                sb.AppendLine($"    <li class=\"toc-item toc-h{h.level}\"><a class=\"toc-link text-sm text-blue-600 hover:underline\" href=\"#{h.id}\">{WebUtility.HtmlEncode(h.text)}</a></li>");
            }

            while (currentLevel > baseLevel)
            {
                sb.AppendLine("    </ul>");
                currentLevel--;
            }

            sb.AppendLine("  </ul>");
            sb.AppendLine("</nav>");

            return sb.ToString();
        }
    }
}
