using Markdig;
using System.Text.RegularExpressions;

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
            {htmlContent}
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
    }
}
